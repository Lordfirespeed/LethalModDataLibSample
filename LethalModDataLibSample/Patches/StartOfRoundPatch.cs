using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LethalModDataLibSample.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[HarmonyPatch(typeof(StartOfRound))]
public static class StartOfRoundPatch
{
    [HarmonyPatch(nameof(StartOfRound.Start))]
    [HarmonyPostfix]
    public static void OnSessionStart(StartOfRound __instance)
    {
        if (!__instance.IsOwner) return;

        try {
            var lastVisitedPlanetManager = Object.Instantiate(Plugin.LastVisitedPlanetManagerPrefab, __instance.transform);
            lastVisitedPlanetManager.hideFlags = HideFlags.None;
            lastVisitedPlanetManager.GetComponent<NetworkObject>().Spawn();
        }
        catch (Exception exc) {
            Plugin.Logger.LogError($"Failed to instantiate last visited planet manager:\n{exc}");
        }
    }
}
