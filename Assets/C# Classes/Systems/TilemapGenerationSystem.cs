using System.Collections.Generic;
using C__Classes.Systems;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace C__Classes.Systems
{
    public class TilemapGenerationSystem : MonoBehaviour
    {
        [SerializeField] 
        private string hash;
        [SerializeField]
        private Tilemap tilemap;
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

        [SerializeField] 
        private int mapSize = 1024;
        [SerializeField]
        private int chunkSize = 128;
        

        private void Start()
        {
            RunProceduralGeneration();
        }
        
        private void RunProceduralGeneration()
        {
            GenerateTilemap();
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
                return waterTile;
            }
            if (value < 0.45f)
            {
                return GenerateRandomTile(groundTilesLight, x, y);
            }
            if (value < 0.75f)
            {
                return GenerateRandomTile(groundTilesMed, x, y);
            }
            if (value < 0.95f)
            {
                return GenerateRandomTile(groundTilesDark, x, y);
            }
            return GenerateRandomTile(cropTiles, x, y);
            
            /*return value switch
            {
                < 0.2f => waterTile,
                < 0.9f => groundTile,
                _      => highValueTile
            };*/
        }

        private TileBase GenerateRandomTile(TileBase[] tiles, int x, int y)
        {
            int tileSeed = MapGenerationSystem.GenerateSeed(hash);
            int tileHash = x * 73856093 ^ y * 19349663 ^ tileSeed;
            Random.InitState(tileHash);
            return tiles[Random.Range(0, tiles.Length)];
        }
    }
}
