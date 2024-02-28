using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using LethalAPI.LibTerminal;
using LethalAPI.LibTerminal.Attributes;
using LethalAPI.LibTerminal.Models;
using UnityEngine;

namespace LethalModDataLibSample;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(LethalModDataLib.PluginInfo.PLUGIN_GUID, LethalModDataLib.PluginInfo.PLUGIN_VERSION)]
[BepInDependency(LethalLib.MyPluginInfo.PLUGIN_GUID, LethalLib.MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(LethalAPI.LibTerminal.PluginInfo.PLUGIN_GUID, LethalAPI.LibTerminal.PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static Plugin Instance { get; private set; } = null!;
    internal static new ManualLogSource Logger { get; private set; } = null!;
    private static Harmony? _harmony;
    private static TerminalModRegistry? _commandRegistry;

    public static GameObject LastVisitedPlanetManagerPrefab = null!;

    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;

        InitializePrefabs();
        InitializeCommandRegistry();
        Patch();

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    [UsedImplicitly]
    [TerminalCommand("LastPlanet")]
    [CommandInfo("See the name of the last planet the ship visited.")]
    public string LastPlanetCommand()
    {
        if (LastVisitedPlanetBehaviour.Instance is not { } lastVisitedPlanetBehaviourInstance)
            throw new InvalidOperationException($"{nameof(LastVisitedPlanetBehaviour)} has not been instantiated.", new NullReferenceException());

        var lastVisitedPlanet = lastVisitedPlanetBehaviourInstance.SyncedLastVisitedPlanet.Value;

        if (lastVisitedPlanet is null) return "The ship hasn't landed anywhere yet!\n\n";
        return $"The ship last landed on {lastVisitedPlanet}.\n\n";
    }

    private void InitializePrefabs()
    {
        var inactiveContainer = new GameObject("Inactive Prefab Container");
        inactiveContainer.SetActive(false);

        LastVisitedPlanetManagerPrefab = LethalLib.Modules.NetworkPrefabs.CreateNetworkPrefab("Last Visited Planet Manager");
        Logger.LogWarning("Ignore the following error r.e. 'NetworkManager is not listening'. NetCode is a big whiny baby.");
        LastVisitedPlanetManagerPrefab.transform.SetParent(inactiveContainer.transform);
        LastVisitedPlanetManagerPrefab.AddComponent<LastVisitedPlanetBehaviour>();
    }

    private void InitializeCommandRegistry()
    {
        _commandRegistry = TerminalRegistry.CreateTerminalRegistry();
        _commandRegistry.RegisterFrom(this);
    }

    private void Patch()
    {
        _harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

        Logger.LogDebug("Patching...");

        _harmony.PatchAll();

        Logger.LogDebug("Finished Patching!");
    }

    private void Unpatch()
    {
        if (_harmony is null) {
            Logger.LogDebug("Nothing to unpatch.");
            return;
        }

        Logger.LogDebug("Unpatching...");

        _harmony.UnpatchSelf();

        Logger.LogDebug("Finished Unpatching!");
    }

}
