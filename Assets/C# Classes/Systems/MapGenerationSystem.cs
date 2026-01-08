using System;

namespace C__Classes.Systems
{
    public class MapGenerationSystem
    {
        private static readonly int[,] vectors =
            { { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, 1 }, { 1, 1 }
                , { 2, 1 }, { -2, 1 }, { 1, 2 }, { -1, -2 } //dodatkowe wektory
                , { 2, -1 }, { 1, -2 }, { -2, -1 }, { -1, 2 } //dodatkowe wektory
            };

    //chunk corner values
    private static readonly int[] tl = { 0, 0 };
    private static readonly int[] tr = { 1, 0 };
    private static readonly int[] bl = { 0, 1 };
    private static readonly int[] br = { 1, 1 };

    // public float[,] GenerateNoiseChunk(int size, int chunkSize)
    private static float
        GenerateNoiseInPoint(int x, int y, int chunkSize, Vec vec1, Vec vec2, Vec vec3, Vec vec4) //test only
    {
        //vector placement in 2D                        this one <-|
        // [1,2]
        // [3,4]

        float vec1dotProd, vec2dotProd, vec3dotProd, vec4dotProd;

        float interpolatedTop; //top horizontal interpolated value of dot products
        float interpolatedBottom; //bottom horizontal interpolated value of dot products

        float finalInterpolated; //final interpolation of the value in point
        
        int chunkX = x / chunkSize;
        int chunkY = y / chunkSize;

        float cellUVx = (x - chunkX * chunkSize) / (float) chunkSize;
        float cellUVy = (y - chunkY * chunkSize) / (float) chunkSize;


        //UV calculation
        //first vector calculations related to the point in the grid from the corner of the chunk
        //-- this also applies to the usage of the function, where vec1 = 1, vec2 = 2, vec3 = 3 and vec4 = 4
        float[] cornerUV1 = { cellUVx - tl[0], cellUVy - tl[1] };
        float[] cornerUV2 = { cellUVx - tr[0], cellUVy - tr[1] };
        float[] cornerUV3 = { cellUVx - bl[0], cellUVy - bl[1] };
        float[] cornerUV4 = { cellUVx - br[0], cellUVy - br[1] };

        //dot product calculation for each corner -- keeping the vector placement in mind -^
        vec1dotProd = cornerUV1[0] * vec1.x + cornerUV1[1] * vec1.y;
        vec2dotProd = cornerUV2[0] * vec2.x + cornerUV2[1] * vec2.y;
        vec3dotProd = cornerUV3[0] * vec3.x + cornerUV3[1] * vec3.y;
        vec4dotProd = cornerUV4[0] * vec4.x + cornerUV4[1] * vec4.y;

        //horizontal interpolation for top and bottom dot products
        interpolatedTop = Interpolate(vec1dotProd, vec2dotProd, cellUVx);
        interpolatedBottom = Interpolate(vec3dotProd, vec4dotProd, cellUVx);

        finalInterpolated = Interpolate(interpolatedTop, interpolatedBottom, cellUVy);

        return finalInterpolated * 0.5f + 0.5f; //'flattening' the value to be contained between 0 and 1
        // return finalInterpolated; //'flattening' the value to be contained between 0 and 1
    }
    
    public static float[,] GenerateMap(int size, int chunkSize, string hash)
    {
        if (size % chunkSize != 0)
        {
            throw new ArgumentException("Size must be divisible by chunkSize");
        }
            
        // Random rng = new Random(GenerateSeed(hash));
            
        float[,] grid = new float[size, size];

        int chunkCount = size / chunkSize;
        Vec[,] randomVectors = new Vec[chunkCount + 1, chunkCount + 1];

        for (int y = 0; y < randomVectors.GetLength(0); y++)
        {
            for (int x = 0; x < randomVectors.GetLength(1); x++)
            {
                //tu implementacja losowania vectorów
                randomVectors[x, y] = GenerateDeterministicVec(x, y, hash);
            }
        }
        
        for (int y = 0; y < size; y++)
        {
            int iY = y / chunkSize;

            for (int x = 0; x < size; x++)
            {
                int iX = x / chunkSize; 

                //wywołanie metody generującej wartość szumu w [j, i]
                grid[y, x] = GenerateNoiseInPoint(x, y, chunkSize, 
                    randomVectors[iX, iY], 
                    randomVectors[iX + 1, iY], 
                    randomVectors[iX, iY + 1], 
                    randomVectors[iX + 1, iY + 1]);
            }
        }

        return grid;
    }
    
    private static float Interpolate(float a, float b, float t)
    {
        return b * Smooth(t) + a * (1 - Smooth(t));
    }

    private static float Smooth(float t)
    {
        return 6 * t * t * t * t * t - 15 * t * t * t * t + 10 * t * t * t;
    }
        
    private static int GenerateSeed(String hash)
    {
        var sha1 = System.Security.Cryptography.SHA256.Create();
        var bytes = sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(hash));
        return BitConverter.ToInt32(bytes, 0);
    }
    
    private static Vec GenerateDeterministicVec(int x, int y, string hash)
    {
        int seed = GenerateSeed(hash + "_" + x + "_" + y); // <- zmiana: zależne od x,y + hash
        Random rng = new Random(seed);
        int index = rng.Next(vectors.GetLength(0));
        return Vec.NewVec(vectors[index, 0], vectors[index, 1]);
    }
    
    private struct Vec
    {
        public int x;
        public int y;

        private Vec(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vec NewVec(int x, int y)
        {
            return new Vec(x, y);
        }
    }
    }
}