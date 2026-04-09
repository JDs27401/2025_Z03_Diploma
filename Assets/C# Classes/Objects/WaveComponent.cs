using System;
using UnityEngine;

namespace C__Classes.Objects
{
    public class WaveComponent : MonoBehaviour
    {
        public event Action OnDeath;

        private void OnDestroy()
        {
            OnDeath?.Invoke();
        }
    }
}