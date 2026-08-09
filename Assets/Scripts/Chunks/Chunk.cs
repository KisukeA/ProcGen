using UnityEngine;

public class Chunk
{
    public int[,,] blocks = new int[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkDepth];
    
    public void FillTest()
    {
        for (int x = 0; x < VoxelData.ChunkWidth; x++)
        {
            for (int z = 0; z < VoxelData.ChunkDepth; z++)
            {
                for (int y = 0; y < VoxelData.ChunkHeight; y++)
                {
                    blocks[x, y, z] = (byte)BlockMap.Grass;
                }
            }
        }
    }

    public void GenerateTerrain(int worldX, int worldZ, NoiseSettings settings)
    {
        for (int x = 0; x < VoxelData.ChunkWidth; x++)
        {
            for (int z = 0; z < VoxelData.ChunkDepth; z++)
            {
                float noiseX = worldX + x;
                float noiseZ = worldZ + z;

                int height = Mathf.FloorToInt(Noise.GenerateHeight(noiseX, noiseZ, settings));

                for (int y = 0; y < VoxelData.ChunkHeight; y++)
                {
                    blocks[x, y, z] = GetBlock(y, height);
                }
            }
        }
    }

    public byte GetBlock(int y, int perlinHeight){
        // edge cases
        if (y < 0 || y > VoxelData.ChunkHeight)
            return (byte)BlockMap.Air;
        if (y == 0)
            return (byte)BlockMap.Bedrock;

        // generation logic
        if (y == perlinHeight){
            return (byte)BlockMap.Grass;
        }
        else if (y > perlinHeight) {
            return (byte)BlockMap.Air;
        }
        else if (y > perlinHeight - 4 && y < perlinHeight){
            return (byte)BlockMap.Dirt;
        }
        // else if (y > perlinHeight - 8 && y <= perlinHeight - 4){
        //     return (byte)BlockMap.Furnace;
        // }
        // else if (y > perlinHeight - 12 && y <= perlinHeight - 8){
        //     return (byte)BlockMap.Sand;
        // }
        else return (byte)BlockMap.Stone;        
    }

    public bool isBlockInChunk(int x, int y, int z){
        return 
            !(x < 0 || x >= VoxelData.ChunkWidth ||
            y < 0 || y >= VoxelData.ChunkHeight ||
            z < 0 || z >= VoxelData.ChunkDepth);
    }

}

public enum BlockMap
{
    Grass = 0,
    Stone = 1,
    Dirt = 2,
    Air = 3,
    Water = 4,
    Furnace = 5,
    Sand = 6,
    Bedrock = 7
}
