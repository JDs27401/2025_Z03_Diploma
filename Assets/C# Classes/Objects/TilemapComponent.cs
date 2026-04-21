using System;
using System.Collections.Generic;
using C__Classes.Systems;
using UnityEngine;

namespace C__Classes.Objects
{
    public class TilemapComponent : MonoBehaviour
    {
        private string _ownTag;
        private Dictionary<string, TileType> _dict = new Dictionary<string, TileType>
        {
            {"groundTilemap", TileType.Ground},
            {"waterTilemap", TileType.Water },
            {"cropTilemap", TileType.CropField},
            {"interiorTilemap", TileType.Interior },
            {"roadTilemap", TileType.Road },
        };

        private void Awake()
        {
            _ownTag = tag;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("projectile"))
            {
                return;
            }
            Actor actor = other.GetComponent<Actor>();
            if (actor == null)
            {
                return;
            }
            actor.TileType = _dict[_ownTag];
            print(actor.TileType);
        }
    }
}