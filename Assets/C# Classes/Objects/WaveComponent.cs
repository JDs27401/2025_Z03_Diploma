using System;
using UnityEngine;

namespace C__Classes.Objects
{
    public class WaveComponent : MonoBehaviour
    {
        public event Action<GameObject> OnDeath;

        private void OnDestroy()
        {
            OnDeath?.Invoke(gameObject);
        }
    }
}