// © SS220, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/CLA.txt
using Content.Server.Database;
using Content.Shared.Damage;
using Content.Shared.Roles;
using Content.Shared.Whitelist;

namespace Content.Server.SS220.AntagItem;

[RegisterComponent]
public sealed partial class AntagItemComponent : Component
{
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public DamageSpecifier DamageOnDrop = new DamageSpecifier();
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public string TextOnDrop = string.Empty;
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public bool StunOnDrop = true;
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan StunTime = TimeSpan.FromSeconds(3);
}
