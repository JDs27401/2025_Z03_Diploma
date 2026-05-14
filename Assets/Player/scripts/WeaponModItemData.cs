using UnityEngine;
using Player.scripts;

[CreateAssetMenu(fileName = "New Weapon Mod Item", menuName = "Item/Weapon Mod Item")]
public class WeaponModItemData : ItemData
{
    [Header("Weapon Mod Link")]
    public WeaponModData weaponModData;

    private void OnValidate()
    {
        itemType = ItemType.WeaponMod;
        isStackable = false;
        maxStackSize = 1;
    }
}

