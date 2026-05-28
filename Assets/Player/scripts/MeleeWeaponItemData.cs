using UnityEngine;
using Player.scripts;

[CreateAssetMenu(fileName = "New Melee Weapon Item", menuName = "Item/Melee Weapon Item")]
public class MeleeWeaponItemData : ItemData
{
    [Header("Melee Weapon Link")]
    public MeleeWeaponData meleeWeaponData;
}

