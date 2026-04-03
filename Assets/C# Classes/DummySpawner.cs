using System;
using UnityEngine;

namespace C__Classes
{
    public class DummySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int num;
        
        private void Start()
        {
            for (int i = 0; i < num; i++)
            {
                Instantiate(prefab, transform.position, Quaternion.identity);
            }
        }
    }
}