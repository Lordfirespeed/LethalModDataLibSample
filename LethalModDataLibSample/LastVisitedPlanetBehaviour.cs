using System;
using JetBrains.Annotations;
using LethalModDataLib.Attributes;
using LethalModDataLib.Enums;
using LethalModDataLib.Features;
using Unity.Netcode;

namespace LethalModDataLibSample;

public class LastVisitedPlanetBehaviour : NetworkBehaviour
{
    public static LastVisitedPlanetBehaviour? Instance { get; private set; }
    
    // It feels like this is going to be the 'de facto' default - maybe some default parameters for the ctor would be nice?
    // Maybe consider an overload using a 'SaveConfiguration' object so people can construct the settings once and re-use them. That pattern also makes adding optional parameters to the API without breaking changes much easier!
    // also, please add XML doc comments. Set `<GenerateDocumentationFile>true</GenerateDocumentationFile>` in MSBuild to enforce their existence for public types and members
    [UsedImplicitly]
    [ModData(SaveWhen.OnAutoSave, LoadWhen.OnLoad, SaveLocation.CurrentSave)]
    private string? LastVisitedPlanet {
        get => SyncedLastVisitedPlanet.Value;
        set => SyncedLastVisitedPlanet.Value = value;
    }

    public NetworkVariable<string?> SyncedLastVisitedPlanet { get; } = new();

    public override void OnNetworkSpawn()
    {
        if (Instance is not null)
            throw new InvalidOperationException($"{nameof(LastVisitedPlanetBehaviour)} has already been instantiated!");
        
        Instance = this;
        if (IsOwner) ModDataHandler.RegisterInstance(this);
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        if (Instance == this) Instance = null;
        if (IsOwner) ModDataHandler.DeRegisterInstance(this);
        base.OnNetworkDespawn();
    }
}