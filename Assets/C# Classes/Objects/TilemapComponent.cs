using System;
using System.Collections.Generic;
using C__Classes.Systems;
using Enemy.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace C__Classes.Objects
{
    public class TilemapComponent : MonoBehaviour
    {
        [SerializeField] private float groundMult = 1.0f;
        [SerializeField] private float waterMult = 0.5f;
        [SerializeField] private float cropFieldMult = 0.8f;
        [SerializeField] private float interiorMult = 1.0f;
        [SerializeField] private float roadMult = 1.0f;
        
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
            switch (_ownTag)
            {
                case "groundTilemap":
                    actor.SetSpeed(groundMult);
                    break;
                case "waterTilemap":
                    actor.SetSpeed(waterMult);
                    break;
                case "cropTilemap":
                    actor.SetSpeed(cropFieldMult);
                    break;
                case "interiorTilemap":
                    actor.SetSpeed(interiorMult);
                    break;
                case "roadTilemap":
                    actor.SetSpeed(roadMult);
                    break;
            }
            print(actor.TileType);
        }

        private void OnTriggerExit2D(Collider2D other)
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
            actor.TileType = _dict["groundTilemap"];
            actor.SetSpeed(groundMult);
        }
    }
}