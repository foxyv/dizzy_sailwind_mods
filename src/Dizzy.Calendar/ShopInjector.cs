using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dizzy.Calendar
{
    /// <summary>
    /// Injects a shop spawner for the calendar at Dragon Cliffs (same port Climate uses for instruments).
    /// </summary>
    internal static class ShopInjector
    {
        private const string DragonCliffsScene = "island 9 E Dragon Cliffs";
        private const string DragonCliffsScenery = "island 9 E (dragon cliffs) scenery";
        private const string SpawnerName = "dizzy calendar shop spawner";

        private static bool _injected;

        public static void Init()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            TryInject();
        }

        public static void Shutdown()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == DragonCliffsScene)
            {
                _injected = false;
                TryInject();
            }
        }

        public static void TryInject()
        {
            if (_injected || Plugin.CalendarPrefab == null)
                return;

            GameObject scenery = GameObject.Find(DragonCliffsScenery);
            if (scenery == null)
                return;

            if (scenery.transform.Find(SpawnerName) != null)
            {
                _injected = true;
                return;
            }

            // Near Climate instrument stall coords on Dragon Cliffs.
            var spawnerGo = new GameObject(SpawnerName);
            spawnerGo.transform.SetParent(scenery.transform, false);
            spawnerGo.transform.localPosition = new Vector3(-73.35f, 4.55f, -552.25f);
            spawnerGo.transform.localRotation = Quaternion.Euler(76f, 140f, 0f);

            var filter = spawnerGo.AddComponent<MeshFilter>();
            var prefabFilter = Plugin.CalendarPrefab.GetComponent<MeshFilter>();
            if (prefabFilter != null)
                filter.sharedMesh = prefabFilter.sharedMesh;

            spawnerGo.AddComponent<MeshRenderer>();

            var spawner = spawnerGo.AddComponent<ShopItemSpawner>();
            spawner.itemPrefab = Plugin.CalendarPrefab;

            _injected = true;
            Plugin.Log.LogInfo("Injected calendar ShopItemSpawner at Dragon Cliffs.");
        }
    }
}
