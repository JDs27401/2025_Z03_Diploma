using UnityEngine;
using Player.scripts;

[CreateAssetMenu(fileName = "New Weapon Item", menuName = "Item/Weapon Item")]
public class WeaponItemData : ItemData
{
    [Header("Weapon Link")]
    public WeaponData weaponData;
}

