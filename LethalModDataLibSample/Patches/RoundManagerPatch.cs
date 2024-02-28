using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace LethalModDataLibSample.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[HarmonyPatch(typeof(RoundManager))]
public class RoundManagerPatch
{
    [HarmonyPatch(nameof(RoundManager.LoadNewLevel))]
    [HarmonyPostfix]
    public static void OnNewLevelLoaded(RoundManager __instance)
    {
        if (!__instance.IsOwner) return;

        if (LastVisitedPlanetBehaviour.Instance is not { } lastVisitedPlanetBehaviourInstance) {
            var exc = new InvalidOperationException($"{nameof(LastVisitedPlanetBehaviour)} has not been instantiated.", new NullReferenceException());
            Plugin.Logger.LogError($"Failed to set last visited level:\n{exc}");
            return;
        }

        lastVisitedPlanetBehaviourInstance.LastVisitedPlanet = __instance.currentLevel.PlanetName;
    }
}
