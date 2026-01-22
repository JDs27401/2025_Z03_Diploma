using System.Collections.Generic;
using C__Classes.Systems;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace C__Classes.Systems
{
    public class TilemapGenerationSystem : MonoBehaviour
    {
        [Header("Tilemap Generation System")]
        [SerializeField] 
        private string hash;
        [SerializeField] 
        private int mapSize = 1024;
        [SerializeField]
        private int chunkSize = 128;
        [SerializeField]
        private Tilemap tilemap;
        private Rigidbody2D rigidBody2D;
        private EdgeCollider2D edgeCollider2D;
        [Header("Tiles")]
        [SerializeField]
        private TileBase waterTile;
        [SerializeField]
        private TileBase[] groundTilesLight;
        [SerializeField]
        private TileBase[] groundTilesMed;
        [SerializeField]
        private TileBase[] groundTilesDark;
        [SerializeField] 
        private TileBase[] cropTiles;
        [SerializeField]
        private TileBase highValueTile;
        
        [Header("Buildings")]
        [SerializeField]
        private GameObject[] buildingPrefabs;
        // [SerializeField] 
        // private Vector2 circleCenter;
        [SerializeField]
        private int numberOfCircles;
        [SerializeField]
        private int radius;
        [SerializeField]
        private int radiusIncrease;
        
        private TileProperties[,] tileProperties;

        private void Awake()
        {
            tileProperties = new TileProperties[mapSize, mapSize];
            RunProceduralGeneration();
        }
        
        private void Start()
        {
            
        }
        
        private void RunProceduralGeneration() //maybe this whole shit ass method should be removed
        {
            GenerateTilemap();
            GenerateMapBoundaries(); //dunno, this has to be b4 RunBuildingGeneration() method for some reason 
            RunBuildingGeneration();
        }

        private void GenerateMapBoundaries()
        {
            /*rigidBody2D = tilemap.GetComponent<Rigidbody2D>();
            if (ReferenceEquals(rigidBody2D, null))
            {
                return;
            }*/

            rigidBody2D = tilemap.AddComponent<Rigidbody2D>();
            rigidBody2D.bodyType = RigidbodyType2D.Static;

            edgeCollider2D = tilemap.AddComponent<EdgeCollider2D>();
            
            tilemap.CompressBounds();
            
            BoundsInt bounds = tilemap.cellBounds;
            Vector3 min = tilemap.CellToWorld(bounds.min);
            Vector3 max = tilemap.CellToWorld(bounds.max);

            //Punkty ramki (zgodnie z ruchem wskazówek)
            Vector2[] points =
            {
                new Vector2(min.x, min.y),
                new Vector2(min.x, max.y),
                new Vector2(max.x, max.y),
                new Vector2(max.x, min.y),
                new Vector2(min.x, min.y) // zamknięcie
            };

            edgeCollider2D.points = points;
        }

        private void GenerateTilemap()
        {
            float[,] tileValues = MapGenerationSystem.GenerateMap(mapSize, chunkSize, hash);

            for (int y = 0; y < tileValues.GetLength(1); y++)
            {
                for (int x = 0; x < tileValues.GetLength(0); x++)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), SelectTile(tileValues[x,y], x, y));
                }    
            }
        }

        private TileBase SelectTile(float value, int x, int y)
        {
            if (value < 0.15f)
            {
                tileProperties[x,y] = new TileProperties(x, y, TileType.Water);
                return waterTile;
            }
            if (value < 0.45f)
            {
                tileProperties[x,y] = new TileProperties(x, y, TileType.Ground);
                return GenerateRandomTile(groundTilesLight, x, y);
            }
            if (value < 0.75f)
            {
                tileProperties[x,y] = new TileProperties(x, y, TileType.Ground);
                return GenerateRandomTile(groundTilesMed, x, y);
            }
            if (value < 0.95f)
            {
                tileProperties[x,y] = new TileProperties(x, y, TileType.Ground);
                return GenerateRandomTile(groundTilesDark, x, y);
            }
            tileProperties[x,y] = new TileProperties(x, y, TileType.CropField);
            return GenerateRandomTile(cropTiles, x, y);
        }

        private TileBase GenerateRandomTile(TileBase[] tiles, int x, int y)
        {
            int tileSeed = MapGenerationSystem.GenerateSeed(hash);
            int tileHash = x * 73856093 ^ y * 19349663 ^ tileSeed;
            Random.InitState(tileHash);
            return tiles[Random.Range(0, tiles.Length)];
        }

        private void RunBuildingGeneration()
        {
            if (buildingPrefabs.Length == 0)
                return;

            Vector2Int center = new Vector2Int(mapSize / 2, mapSize / 2);

            for (int i = 1; i <= numberOfCircles; i++)
            {
                float currentRadius = radius + i * radiusIncrease;
                float startDeg = GenerateBuildingAngle(center, currentRadius, i);

                float stepDeg = 360f / i;
                
                for (int j = 0; j < i; j++)
                {
                    float angleDeg = startDeg + j * stepDeg;
                    float angleRad = angleDeg * Mathf.Deg2Rad;

                    int x = Mathf.RoundToInt(center.x + Mathf.Cos(angleRad) * currentRadius);
                    int y = Mathf.RoundToInt(center.y + Mathf.Sin(angleRad) * currentRadius);

                    int prefabIndex = GetDeterministicPrefabIndex(x, y, j);

                    Instantiate(
                        buildingPrefabs[prefabIndex],
                        new Vector3(x, y, 0),
                        Quaternion.identity
                    );    
                }
            }
        }

        private float GenerateBuildingAngle(Vector2Int center, float localRadius, int index)
        {
            int seed = MapGenerationSystem.GenerateSeed(hash);

            int h =
                seed ^
                (center.x * 73856093) ^
                (center.y * 19349663) ^
                ((int)localRadius * 83492791) ^
                (index * unchecked((int)2654435761));

            uint u = (uint)h;
            return (u / (float)uint.MaxValue) * 360f;
        }
        
        private int GetDeterministicPrefabIndex(int x, int y, int index)
        {
            int seed = MapGenerationSystem.GenerateSeed(hash);

            int h =
                seed ^
                (x * 374761393) ^
                (y * 668265263) ^
                (index * 2147483647);

            h = Mathf.Abs(h);
            return h % buildingPrefabs.Length;
        }


        
        public TileProperties GetTileProperties(int x, int y)
        {
            return tileProperties[x, y];    
        }

        public struct TileProperties
        {
            public int x;
            public int y;
            public TileType type;

            public TileProperties(int x, int y, TileType tileType)
            {
                this.x = x;
                this.y = y;
                this.type = tileType;
            }
        }
    }
}
