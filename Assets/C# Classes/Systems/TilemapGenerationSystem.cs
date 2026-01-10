using System.Collections.Generic;
using C__Classes.Systems;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGenerationSystem : MonoBehaviour
{
    [SerializeField] 
    private string hash;
    [SerializeField]
    private Tilemap tilemap;
    [SerializeField]
    private TileBase waterTile;
    [SerializeField]
    private TileBase groundTile;
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
                tilemap.SetTile(new Vector3Int(x, y, 0), SelectTile(tileValues[x,y]));
            }    
        }
    }

    private TileBase SelectTile(float value)
    {
        if (value < 0.2f)
        {
            return waterTile;
        }
        if (value < 0.9f)
        {
            return groundTile;
        }
        return highValueTile;
        
        /*return value switch
        {
            < 0.2f => waterTile,
            < 0.9f => groundTile,
            _      => highValueTile
        };*/
    }
}
