namespace Player.scripts
{
    [System.Serializable]
    public class WeaponModInstanceState
    {
        public WeaponModItemData itemData;
        public WeaponModData modData => itemData != null ? itemData.weaponModData : null;
        public bool isActive = false;

        public WeaponModInstanceState(WeaponModItemData itemData)
        {
            this.itemData = itemData;
            this.isActive = false;
        }

        public WeaponModInstanceState Clone()
        {
            return new WeaponModInstanceState(itemData)
            {
                isActive = false
            };
        }
    }
}


