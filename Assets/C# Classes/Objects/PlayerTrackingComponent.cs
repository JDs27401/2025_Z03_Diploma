using System;
using Enemy.Scripts;
using UnityEngine;

namespace C__Classes.Objects
{
    public class PlayerTrackingComponent : MonoBehaviour
    {
        private NpcBase _base;

        private void Start()
        {
            _base = GetComponent<NpcBase>();
            if (_base == null)
            {
                #if UNITY_EDITOR
                print("No NPC Base component found");
                #endif
                return;
            }
            _base.Aggravate(GameObject.FindWithTag("player").transform);
        }
    }
}