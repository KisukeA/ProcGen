using UnityEngine;

public static class Noise
{
    // Simple Perlin height generator
    public static float GetHeight(float x, float z, float scale, float amplitude, int octaves, float persistence, float lacunarity)
    {
        float height = 0f;
        float freq = scale;
        float amp = amplitude;

        for (int i = 0; i < octaves; i++)
        {
            float n = Mathf.PerlinNoise(x * freq, z * freq);
            height += n * amp;

            amp *= persistence;
            freq *= lacunarity;
        }

        return height;
    }
    
    public static float GenerateHeight(float x, float z, NoiseSettings s)
    {
        float noiseValue = 0f;
        float amplitude = 1f;
        float frequency = s.scale;

        for (int i = 0; i < s.octaves; i++)
        {
            float perlin = Mathf.PerlinNoise(
                (x + 0.1f) / VoxelData.ChunkWidth * frequency + s.offset,
                (z + 0.1f) / VoxelData.ChunkWidth * frequency + s.offset
            );
            // return perlin;
            noiseValue += perlin * amplitude;

            amplitude *= s.persistence;
            frequency *= s.lacunarity;
        }
        // return noiseValue;
        return (noiseValue * s.amplitude) + s.baseLevel;
    }
    
    // public static float GenerateHeight(float x, float z, NoiseSettings s)
    // {
    //     float noiseValue = 0f;
    //     float amplitude = 1f;
    //     float frequency = s.scale;
    //     float maxPossible = 0f;

    //     for (int i = 0; i < s.octaves; i++)
    //     {
    //         float perlin = Mathf.PerlinNoise(
    //             x * frequency,
    //             z * frequency
    //         );

    //         noiseValue += perlin * amplitude;
    //         maxPossible += amplitude;

    //         amplitude *= s.persistence;
    //         frequency *= s.lacunarity;
    //     }

    //     // --- NEW: normalize ----------
    //     float rawNormalized = noiseValue / maxPossible; // 0..1

    //     // --- NEW: curve shaping -------
    //     float curved = s.heightCurve.Evaluate(rawNormalized); // 0..1

    //     // --- finish normally ------
    //     return (curved * s.amplitude) + s.baseLevel;
    // }

}
