// © SS220, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/CLA.txt

namespace Content.Server.SS220.AntagItem;

[RegisterComponent]
public sealed partial class AntagBackpackComponent : Component
{
    [DataField]
    public bool Unequipble = true;
    [DataField]
    public string UnquipText = "Невозможно снять. Вещь, кажется приросла к вашему телу намертво.";
    [DataField]
    public string Slot = "back";
    [DataField]
    public string PopupText = "Невозможно. Рюкзак сопротивляется вашей воле.";
}
