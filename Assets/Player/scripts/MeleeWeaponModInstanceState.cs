namespace Player.scripts
{
    [System.Serializable]
    public class MeleeWeaponModInstanceState
    {
        public MeleeWeaponModItemData itemData;
        public MeleeWeaponModData modData => itemData != null ? itemData.meleeWeaponModData : null;
        public bool isActive = false;

        public MeleeWeaponModInstanceState(MeleeWeaponModItemData itemData)
        {
            this.itemData = itemData;
            this.isActive = false;
        }

        public MeleeWeaponModInstanceState Clone()
        {
            return new MeleeWeaponModInstanceState(itemData)
            {
                isActive = false
            };
        }
    }
}

