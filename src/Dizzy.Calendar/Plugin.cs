using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace Dizzy.Calendar
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInProcess("Sailwind.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.dizzy.sailwind.calendar";
        public const string PluginName = "Dizzy Calendar";
        public const string PluginVersion = "0.1.0";

        internal static ManualLogSource Log;
        internal static GameObject CalendarPrefab;

        private ConfigEntry<KeyboardShortcut> _debugSpawnKey;
        private Harmony _harmony;

        private void Awake()
        {
            Log = Logger;

            _debugSpawnKey = Config.Bind(
                "Debug",
                "SpawnCalendar",
                new KeyboardShortcut(KeyCode.F8),
                "Spawn a sold calendar in front of the player for hang testing.");

            CalendarPrefab = CalendarPrefabFactory.Create();

            _harmony = new Harmony(PluginGuid);
            _harmony.PatchAll(typeof(Plugin).Assembly);

            ShopInjector.Init();

            Log.LogInfo($"{PluginName} v{PluginVersion} loaded. Prefab index {CalendarItem.PrefabIndex}. Debug spawn: {_debugSpawnKey.Value}");
        }

        private void Update()
        {
            ShopInjector.TryInject();

            if (_debugSpawnKey.Value.IsDown())
                SpawnDebugCalendar();
        }

        private void OnDestroy()
        {
            ShopInjector.Shutdown();
            _harmony?.UnpatchSelf();

            if (CalendarPrefab != null)
            {
                Object.Destroy(CalendarPrefab);
                CalendarPrefab = null;
            }
        }

        private void SpawnDebugCalendar()
        {
            if (CalendarPrefab == null)
            {
                Log.LogError("Cannot spawn calendar: prefab is null.");
                return;
            }

            Transform cam = null;
            if (Refs.ovrCameraRig != null)
                cam = Refs.ovrCameraRig;
            else if (Camera.main != null)
                cam = Camera.main.transform;

            if (cam == null)
            {
                Log.LogWarning("Cannot spawn calendar: no camera/player reference yet.");
                return;
            }

            Vector3 pos = cam.position + cam.forward * 1.2f + Vector3.up * 0.2f;
            GameObject instance = Object.Instantiate(CalendarPrefab, pos, Quaternion.LookRotation(cam.forward, Vector3.up));
            instance.SetActive(true);
            instance.name = "calendar";

            var item = instance.GetComponent<CalendarItem>();
            if (item != null)
            {
                item.sold = true;
                item.RefreshDateDisplay(force: true);
            }

            var saveable = instance.GetComponent<SaveablePrefab>();
            if (saveable != null)
                saveable.RegisterToSave();

            Log.LogInfo($"Debug-spawned calendar at {pos} ({CalendarItem.FormatDayLabel()}).");
        }
    }
}
