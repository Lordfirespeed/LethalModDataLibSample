using System;
using JetBrains.Annotations;
using LethalModDataLib.Attributes;
using LethalModDataLib.Enums;
using LethalModDataLib.Features;
using Unity.Collections;
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
    public string? LastVisitedPlanet {
        get => _lastVisitedPlanetHasValue.Value ? _lastVisitedPlanet.Value.Value : null;
        internal set {
            if (value is null) {
                _lastVisitedPlanetHasValue.Value = false;
                return;
            }

            _lastVisitedPlanetHasValue.Value = true;
            _lastVisitedPlanet.Value = new FixedString64Bytes(value);
        }
    }

    private readonly NetworkVariable<bool> _lastVisitedPlanetHasValue = new() { Value = false, };

    private readonly NetworkVariable<FixedString64Bytes> _lastVisitedPlanet = new() { Value = "" };

    public override void OnNetworkSpawn()
    {
        var thisIsInstance = ReferenceEquals(this, Instance);
        var instanceIsNull = ReferenceEquals(Instance, null);

        if (instanceIsNull && !thisIsInstance) {
            Instance = this;
            if (IsOwner) ModDataHandler.RegisterInstance(this);
        }

        if (!instanceIsNull && !thisIsInstance) {
            throw new InvalidOperationException($"{nameof(LastVisitedPlanetBehaviour)} has been instantiated more than once!");
        }

        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        if (Instance == this) Instance = null;
        if (IsOwner) ModDataHandler.DeRegisterInstance(this);
        base.OnNetworkDespawn();
    }
}
