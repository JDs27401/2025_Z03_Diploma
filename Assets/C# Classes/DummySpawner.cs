using System;
using UnityEngine;

namespace C__Classes
{
    public class DummySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        
        private void Start()
        {
            for (int i = 0; i < 500; i++)
            {
                Instantiate(prefab, transform.position, Quaternion.identity);
            }
        }
    }
}