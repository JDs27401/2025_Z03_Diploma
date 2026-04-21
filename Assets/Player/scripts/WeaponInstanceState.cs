namespace Player.scripts
{
    [System.Serializable]
    public class WeaponInstanceState
    {
        public int currentMagazineAmmo;

        public WeaponInstanceState(int currentMagazineAmmo)
        {
            this.currentMagazineAmmo = currentMagazineAmmo;
        }

        public WeaponInstanceState Clone()
        {
            return new WeaponInstanceState(currentMagazineAmmo);
        }
    }
}

