using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;

namespace Dizzy.Gamma
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInProcess("Sailwind.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.dizzy.sailwind.gamma";
        public const string PluginName = "Dizzy Gamma";
        public const string PluginVersion = "0.1.2";

        public const float MinGamma = 0.5f;
        public const float MaxGamma = 2.5f;

        private const float DialogWidth = 340f;
        private const float DialogHeight = 200f;

        // How hard to lift near-black night ambient when gamma > 1 (multiply alone is a no-op on ~0).
        private const float AmbientLiftStrength = 0.4f;

        internal static ManualLogSource Log;
        internal static Plugin Instance;

        private ConfigEntry<float> _gamma;
        private ConfigEntry<float> _step;
        private ConfigEntry<KeyboardShortcut> _menuKey;
        private ConfigEntry<KeyboardShortcut> _gammaUpKey;
        private ConfigEntry<KeyboardShortcut> _gammaDownKey;

        private bool _showPanel;
        private bool _panelCentered;
        private Rect _panelRect;

        private bool _savedCursorVisible;
        private CursorLockMode _savedCursorLock;

        // Camera.onPreRender can fire for several cameras per frame; only boost ambient once.
        private int _lastAmbientBoostFrame = -1;

        public static float CurrentGamma => Instance != null ? Instance._gamma.Value : 1f;

        private void Awake()
        {
            Instance = this;
            Log = Logger;

            _gamma = Config.Bind(
                "General",
                "Gamma",
                1f,
                new ConfigDescription(
                    "Scene ambient lighting boost. 1.0 is vanilla; higher values brighten dark scenes without washing out UI.",
                    new AcceptableValueRange<float>(MinGamma, MaxGamma)));

            _step = Config.Bind(
                "General",
                "Step",
                0.1f,
                new ConfigDescription(
                    "Amount to change gamma when using Up/Down buttons or hotkeys.",
                    new AcceptableValueRange<float>(0.05f, 0.5f)));

            _menuKey = Config.Bind(
                "Hotkeys",
                "TogglePanel",
                new KeyboardShortcut(KeyCode.F7),
                "Toggle the Gamma settings panel.");

            _gammaUpKey = Config.Bind(
                "Hotkeys",
                "GammaUp",
                new KeyboardShortcut(KeyCode.Equals, KeyCode.RightControl),
                "Increase gamma.");

            _gammaDownKey = Config.Bind(
                "Hotkeys",
                "GammaDown",
                new KeyboardShortcut(KeyCode.Minus, KeyCode.RightControl),
                "Decrease gamma.");

            Camera.onPreRender += ApplyAmbientBoost;

            Log.LogInfo($"{PluginName} v{PluginVersion} loaded (gamma={_gamma.Value:0.00}). Press {_menuKey.Value} for settings.");
        }

        private void OnDestroy()
        {
            Camera.onPreRender -= ApplyAmbientBoost;

            if (_showPanel)
                RestoreCursor();

            if (Instance == this)
                Instance = null;
        }

        private void ApplyAmbientBoost(Camera cam)
        {
            float gamma = CurrentGamma;
            if (Mathf.Approximately(gamma, 1f) || gamma <= 0f)
                return;

            int frame = Time.frameCount;
            if (_lastAmbientBoostFrame == frame)
                return;

            _lastAmbientBoostFrame = frame;
            ApplyLightingBoost(gamma);
        }

        private static void ApplyLightingBoost(float gamma)
        {
            if (gamma > 1f)
            {
                // Night ambient is often ~black; multiply alone barely changes anything.
                float excess = gamma - 1f;
                Color lift = new Color(0.32f, 0.36f, 0.45f) * (AmbientLiftStrength * excess);

                RenderSettings.ambientLight = RenderSettings.ambientLight * gamma + lift;
                RenderSettings.ambientIntensity = RenderSettings.ambientIntensity * gamma + AmbientLiftStrength * excess;
                RenderSettings.ambientSkyColor = RenderSettings.ambientSkyColor * gamma + lift;
                RenderSettings.ambientEquatorColor = RenderSettings.ambientEquatorColor * gamma + lift * 0.75f;
                RenderSettings.ambientGroundColor = RenderSettings.ambientGroundColor * gamma + lift * 0.5f;

                if (RenderSettings.fog)
                    RenderSettings.fogColor = RenderSettings.fogColor * gamma + lift * 0.5f;
            }
            else
            {
                RenderSettings.ambientLight *= gamma;
                RenderSettings.ambientIntensity *= gamma;
                RenderSettings.ambientSkyColor *= gamma;
                RenderSettings.ambientEquatorColor *= gamma;
                RenderSettings.ambientGroundColor *= gamma;

                if (RenderSettings.fog)
                    RenderSettings.fogColor *= gamma;
            }
        }

        private void Update()
        {
            if (_menuKey.Value.IsDown())
                SetPanelVisible(!_showPanel);

            if (_gammaUpKey.Value.IsDown())
                AdjustGamma(_step.Value);

            if (_gammaDownKey.Value.IsDown())
                AdjustGamma(-_step.Value);
        }

        private void LateUpdate()
        {
            // Run after other LateUpdate lighting writers, and re-subscribe last so our
            // onPreRender runs after mods like RealisticSkies that also hook PreRender.
            Camera.onPreRender -= ApplyAmbientBoost;
            Camera.onPreRender += ApplyAmbientBoost;

            // Sailwind re-hides/locks the cursor each frame while playing; keep ours usable over the panel.
            if (_showPanel)
                ApplyMenuCursor();
        }

        private void SetPanelVisible(bool visible)
        {
            if (_showPanel == visible)
                return;

            if (visible)
            {
                _savedCursorVisible = Cursor.visible;
                _savedCursorLock = Cursor.lockState;
                _showPanel = true;
                ApplyMenuCursor();
            }
            else
            {
                _showPanel = false;
                RestoreCursor();
            }
        }

        private static void ApplyMenuCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void RestoreCursor()
        {
            Cursor.visible = _savedCursorVisible;
            Cursor.lockState = _savedCursorLock;
        }

        private void OnGUI()
        {
            if (!_showPanel)
                return;

            if (!_panelCentered)
            {
                CenterPanel();
                _panelCentered = true;
            }

            _panelRect = GUILayout.Window(
                GetHashCode(),
                _panelRect,
                DrawPanel,
                PluginName,
                GUILayout.Width(DialogWidth),
                GUILayout.Height(DialogHeight));
        }

        private void DrawPanel(int windowId)
        {
            GUILayout.Space(6f);
            GUILayout.Label($"Gamma: {_gamma.Value:0.00}  (1.00 = vanilla)");

            float slider = GUILayout.HorizontalSlider(_gamma.Value, MinGamma, MaxGamma);
            if (!Mathf.Approximately(slider, _gamma.Value))
                SetGamma(slider);

            GUILayout.Space(8f);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Gamma Down", GUILayout.Height(28f)))
                AdjustGamma(-_step.Value);
            if (GUILayout.Button("Gamma Up", GUILayout.Height(28f)))
                AdjustGamma(_step.Value);
            GUILayout.EndHorizontal();

            GUILayout.Space(4f);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset (1.0)", GUILayout.Height(28f)))
                SetGamma(1f);
            if (GUILayout.Button("Close", GUILayout.Height(28f)))
                SetPanelVisible(false);
            GUILayout.EndHorizontal();

            GUILayout.Space(4f);
            GUILayout.Label($"Hotkeys: {_menuKey.Value} menu · {_gammaUpKey.Value} up · {_gammaDownKey.Value} down");

            GUI.DragWindow();
        }

        private void AdjustGamma(float delta)
        {
            SetGamma(_gamma.Value + delta);
        }

        private void SetGamma(float value)
        {
            float clamped = Mathf.Clamp(value, MinGamma, MaxGamma);
            // Round to hundredths so slider/hotkeys stay tidy in the cfg.
            clamped = Mathf.Round(clamped * 100f) / 100f;
            if (Mathf.Approximately(clamped, _gamma.Value))
                return;

            _gamma.Value = clamped;
            Log.LogInfo($"Gamma set to {_gamma.Value:0.00}");
        }

        private void CenterPanel()
        {
            _panelRect = new Rect(
                (Screen.width - DialogWidth) * 0.5f,
                (Screen.height - DialogHeight) * 0.5f,
                DialogWidth,
                DialogHeight);
        }
    }
}
