namespace Player.scripts
{
	using UnityEngine;

	public enum WeaponModType
	{
		Standard = 0,
		Flashlight = 1,
		LaserSight = 2
	}

	[CreateAssetMenu(fileName = "New Weapon Mod", menuName = "Weapons/Weapon Mod")]
	public class WeaponModData : ScriptableObject
	{
		public string modName = "New Weapon Mod";
		public Sprite icon;
		public WeaponModType modType = WeaponModType.Standard;

		[TextArea]
		public string description;

		[Header("Shooting Stats")]
		public float fireRateBonus = 0f;
		public float damageBonus = 0f;
		public float projectileSpeedBonus = 0f;
		public float spreadBonus = 0f;
		public int projectilesPerShotBonus = 0;

		[Header("Ammo & Reloading")]
		public int magazineSizeBonus = 0;
		public float reloadTimeBonus = 0f;

		[Header("Behaviour Overrides")]
		public bool overrideIsAutomatic = false;
		public bool isAutomaticValue = false;

		public bool overrideIsExplosive = false;
		public bool isExplosiveValue = false;
		public float explosionRadiusBonus = 0f;

		public bool overrideIsMolotov = false;
		public bool isMolotovValue = false;
		public float dotAreaRadiusBonus = 0f;
		public float dotDamageBonus = 0f;
		public float dotDurationBonus = 0f;
		public float dotIntervalBonus = 0f;
		public float dotAreaLifetimeBonus = 0f;

		[Header("Weight")]
		// Percent change to weapon weight (e.g. 0.2 = +20%, -0.15 = -15%)
		public float weightPercentBonus = 0f;
	}
}

