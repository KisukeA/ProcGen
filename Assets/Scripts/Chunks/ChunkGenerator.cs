using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator
{
    private NoiseSettings noiseSettings;
    private BiomeManager biomeManager;

    public ChunkGenerator(NoiseSettings noiseSettings, BiomeManager biomeManager)
    {
        this.noiseSettings = noiseSettings;
        this.biomeManager = biomeManager;
    }

    public void GenerateChunk(Chunk chunk, ChunkCoord coord)
    {
        for (int x = 0; x < VoxelData.ChunkWidth; x++)
        {
            for (int z = 0; z < VoxelData.ChunkDepth; z++)
            {
                int worldX = coord.x * VoxelData.ChunkWidth + x;
                int worldZ = coord.z * VoxelData.ChunkDepth + z;
                // BIOME SELECTION
                var biomeWeights = biomeManager.GetBiomeWeights(worldX, worldZ);
                // BLEND TERRAIN HEIGHTS
                float height = 0f;
                Biome dominantBiome = null;
                float highestWeight = 0f;

                foreach (var entry in biomeWeights)
                {
                    float biomeHeight = Noise.GenerateNoise(worldX, worldZ, noiseSettings, entry.biome);
                    height += biomeHeight * entry.weight;

                    if (entry.weight > highestWeight)
                    {
                        highestWeight = entry.weight;
                        dominantBiome = entry.biome;
                    }
                }
                // CREATE BLOCKS
                for (int y = 0; y < VoxelData.ChunkHeight; y++)
                {
                    chunk.blocks[x, y, z] = GetBlock(worldX, y, worldZ, height, dominantBiome);
                }
            }
        }
    }

    private byte GetBlock(int worldX, int y, int worldZ, float terrainHeight, Biome biome)
    {
        // TODO make it better
        // edge cases
        if (y > terrainHeight)
            return (byte)BlockMap.Air;
        if (y == 0)
            return (byte)BlockMap.Bedrock;
        //surface
        if (y >= terrainHeight - 1)
            return (byte)biome.surfaceBlock;
        //subsurface
        if (y >= terrainHeight - 4)
            return (byte)biome.subsurfaceBlock;
        //underground
        return (byte)BlockMap.Stone;
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