// © SS220, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/CLA.txt
using Content.Server.Mind;
using Content.Server.Popups;
using Content.Server.Roles;
using Content.Server.Storage.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Storage;
using Content.Shared.Storage.Components;
using Robust.Server.Containers;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;

namespace Content.Server.SS220.AntagItem;

public sealed partial class AntagBacpackSystem : EntitySystem
{
    [Dependency] private readonly MindSystem _mind = default!;
    [Dependency] private readonly RoleSystem _role = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly MobStateSystem _mob = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly ContainerSystem _container = default!;
    [Dependency] private readonly StorageSystem _storage = default!;
    [Dependency] private readonly SharedUserInterfaceSystem _ui = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<AntagBackpackComponent, BoundUIOpenedEvent>(OnOpenAttempt);
        SubscribeLocalEvent<AntagBackpackComponent, ContainerIsInsertingAttemptEvent>(OnInsertAttempt);
        SubscribeLocalEvent<AntagBackpackComponent, ContainerIsRemovingAttemptEvent>(OnRemoveAttempt);
        SubscribeLocalEvent<AntagBackpackComponent, BeingUnequippedAttemptEvent>(OnUnequipAttempt);
        SubscribeLocalEvent<MobStateChangedEvent>(OnMobStateChanged);
    }

    private void OnRemoveAttempt(EntityUid uid, AntagBackpackComponent component, ContainerIsRemovingAttemptEvent ev)
    {
        if (!HasAntagRole(ev.EntityUid, ev.Container))
            ev.Cancel();
    }

    private void OnInsertAttempt(EntityUid uid, AntagBackpackComponent component, ContainerIsInsertingAttemptEvent ev)
    {
        if (!HasAntagRole(ev.EntityUid, ev.Container))
            ev.Cancel();
    }

    private void OnUnequipAttempt(EntityUid uid, AntagBackpackComponent component, BeingUnequippedAttemptEvent ev)
    {
        if (!component.Unequipble)
            return;
        _popup.PopupEntity(component.UnquipText, uid, type: Shared.Popups.PopupType.Medium);
        ev.Cancel();
    }

    private void OnMobStateChanged(MobStateChangedEvent ev)
    {
        if (!_inventory.TryGetSlotEntity(ev.Target, "back", out var backpack))
            return;
        if (!TryComp<AntagBackpackComponent>(backpack, out var comp))
            return;
        if (ev.NewMobState == MobState.Dead || ev.OldMobState == MobState.Dead)
            _inventory.TryUnequip(ev.Target, comp.Slot, true, true, false);

    }

    private void OnOpenAttempt(EntityUid uid, AntagBackpackComponent component, BoundUIOpenedEvent ev)
    {
        if (!TryComp<StorageComponent>(uid, out var comp))
            return;
        if (!_mind.TryGetMind(ev.Actor, out var mindId, out var _) || !_role.MindHasRole<TraitorRoleComponent>(mindId))
            _ui.CloseUserUis(ev.Actor);
    }

    public bool HasAntagRole(EntityUid item, BaseContainer storage)
    {
        if (!TryComp<AntagBackpackComponent>(storage.Owner, out var component))
            return false;
        var intruder = Transform(item).ParentUid;
        if (!_mind.TryGetMind(intruder, out var mindId, out var _) || !_role.MindHasRole<TraitorRoleComponent>(mindId))
        {
            _popup.PopupEntity(component.PopupText, intruder, type: Shared.Popups.PopupType.Medium);
            return false;
        }
        return true;
    }

}
