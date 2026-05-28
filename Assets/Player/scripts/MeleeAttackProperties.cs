using UnityEngine;

namespace Player.scripts
{
    public class MeleeAttackProperties : MonoBehaviour
    {
        public bool isMolotov = false;
        public float dotAreaRadius = 2.5f;
        public float dotDamage = 4f;
        public float dotDuration = 4f;
        public float dotInterval = 1f;
        public float dotAreaLifetime = 5f;
    }
}
