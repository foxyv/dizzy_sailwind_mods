using System;
using HarmonyLib;
using UnityEngine;

namespace Dizzy.Calendar
{
    [HarmonyPatch(typeof(PrefabsDirectory), "PopulateShipItems")]
    internal static class PrefabsDirectoryPatch
    {
        private static void Prefix(PrefabsDirectory __instance)
        {
            if (Plugin.CalendarPrefab == null)
            {
                Plugin.Log.LogError("Calendar prefab missing during PrefabsDirectory populate.");
                return;
            }

            int index = CalendarItem.PrefabIndex;
            if (__instance.directory == null)
            {
                Plugin.Log.LogError("PrefabsDirectory.directory is null.");
                return;
            }

            if (__instance.directory.Length <= index)
            {
                Array.Resize(ref __instance.directory, index + 1);
                Plugin.Log.LogInfo($"Resized PrefabsDirectory.directory to {__instance.directory.Length}.");
            }

            __instance.directory[index] = Plugin.CalendarPrefab;
            Plugin.Log.LogInfo($"Registered calendar prefab at directory[{index}].");
        }
    }
}
