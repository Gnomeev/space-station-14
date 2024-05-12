// © SS220, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/CLA.txt
using System.Linq;
using Content.Server.Antag;
using Content.Server.Mind;
using Content.Server.NPC.Queries.Queries;
using Content.Server.Popups;
using Content.Server.Roles;
using Content.Server.Stunnable;
using Content.Shared.Damage;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Roles;
using Microsoft.CodeAnalysis.CSharp;

namespace Content.Server.SS220.AntagItem;
public sealed class AntagItemSystem : EntitySystem
{
    [Dependency] private readonly MindSystem _mind = default!;
    [Dependency] private readonly RoleSystem _role = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly StunSystem _stun = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<AntagItemComponent, BeingEquippedAttemptEvent>(OnEquipAttempt);
        SubscribeLocalEvent<AntagItemComponent, GettingPickedUpAttemptEvent>(OnPickupAttempt);
    }

    private void OnEquipAttempt(EntityUid uid, AntagItemComponent component, BeingEquippedAttemptEvent ev)
    {
        if (!HasValidRoles(ev.EquipTarget, uid))
        {
            if (ev.EquipTarget != ev.Equipee)
                ev.Cancel();
            UseAntagItem(ev.EquipTarget, uid);
            ev.Cancel();
        }
    }

    private void OnPickupAttempt(EntityUid uid, AntagItemComponent component, GettingPickedUpAttemptEvent ev)
    {
        if (!HasValidRoles(ev.User, uid))
        {
            UseAntagItem(ev.User, uid);
            ev.Cancel();
        }
    }

    public void UseAntagItem(EntityUid target, EntityUid item)
    {
        if (!TryComp<AntagItemComponent>(item, out var component) || !_mind.TryGetMind(target, out var mindId, out var _))
            return;
        if (component.StunOnDrop == true)
        {
            _stun.TryKnockdown(target, component.StunTime, false);
            _stun.TryStun(target, component.StunTime, false);
        }
        _damageable.TryChangeDamage(target, component.DamageOnDrop);
        _popup.PopupEntity(component.TextOnDrop, target, type: Shared.Popups.PopupType.MediumCaution);
    }
    public bool HasValidRoles(EntityUid targetToCheck, EntityUid item)
    {
        if (!TryComp<AntagItemComponent>(item, out var comp))
            return false;
        if (!_mind.TryGetMind(targetToCheck, out var mindId, out var _) || !_role.MindHasRole<TraitorRoleComponent>(mindId))
            return false;
        return true;
    }
}
