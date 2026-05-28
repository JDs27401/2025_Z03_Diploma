using UnityEngine;
using Player.scripts;

[CreateAssetMenu(fileName = "New Melee Weapon Mod Item", menuName = "Item/Melee Weapon Mod Item")]
public class MeleeWeaponModItemData : ItemData
{
    [Header("Melee Weapon Mod Link")]
    public MeleeWeaponModData meleeWeaponModData;

    private void OnValidate()
    {
        itemType = ItemType.MeleeWeaponMod;
        isStackable = false;
        maxStackSize = 1;
    }
}

