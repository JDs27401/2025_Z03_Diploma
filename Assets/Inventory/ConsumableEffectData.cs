using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable Effect", menuName = "Item/Consumable Effect")]
public class ConsumableEffectData : ScriptableObject
{
    [Header("Identity")]
    public string effectId;
    public string effectName = "New Consumable Effect";
    public Sprite icon;

    [Header("Duration")]
    public float duration = 5f;

    [Header("Player Stat Modifiers")]
    public float speedMultiplier = 1f;
    public float accelerationMultiplier = 1f;
    public float maxHealthBonus = 0f;
    public float healthPerSecond = 0f;
    // Stamina
    public float maxStaminaBonus = 0f;
    public float staminaPerSecond = 0f;

    // Damage taken multiplier: values < 1 reduce incoming damage
    public float damageTakenMultiplier = 1f;

    // Noise multiplier: values < 1 make player quieter
    public float noiseMultiplier = 1f;

    // Weapon spread (accuracy) multiplier: values < 1 improve accuracy
    public float weaponSpreadMultiplier = 1f;

    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(effectId))
        {
            effectId = name;
        }

        duration = Mathf.Max(0.01f, duration);
        speedMultiplier = Mathf.Max(0f, speedMultiplier);
        accelerationMultiplier = Mathf.Max(0f, accelerationMultiplier);
        healthPerSecond = Mathf.Max(0f, healthPerSecond);
        maxStaminaBonus = Mathf.Max(0f, maxStaminaBonus);
        staminaPerSecond = Mathf.Max(0f, staminaPerSecond);
        damageTakenMultiplier = Mathf.Max(0f, damageTakenMultiplier);
        noiseMultiplier = Mathf.Max(0f, noiseMultiplier);
        weaponSpreadMultiplier = Mathf.Max(0f, weaponSpreadMultiplier);
    }
}

