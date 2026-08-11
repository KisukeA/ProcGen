using UnityEngine;

public class Chunk
{
    public int[,,] blocks;
    public ChunkCoord chunkCoord;

    public Chunk(ChunkCoord chunkCoord)
    {
        this.chunkCoord = chunkCoord;
        this.blocks = new int[
            VoxelData.ChunkWidth,
            VoxelData.ChunkHeight,
            VoxelData.ChunkDepth
        ];
    }

    //helpers

    public bool IsBlockInChunk(int x, int y, int z)
    {
        return 
            !(x < 0 || x >= VoxelData.ChunkWidth ||
            y < 0 || y >= VoxelData.ChunkHeight ||
            z < 0 || z >= VoxelData.ChunkDepth);
    }
}

