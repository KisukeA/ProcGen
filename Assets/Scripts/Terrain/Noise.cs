using UnityEngine;

public static class Noise
{  
    public static float GenerateNoise(float x, float z, NoiseSettings settings, Biome biome)
    {
        float noiseValue = 0f;
        float octaveAmplitude = 1f;
        float frequency = biome.scale;

        for (int i = 0; i < settings.octaves; i++)
        {
            float perlin = Mathf.PerlinNoise(
                (x + 0.1f) / VoxelData.ChunkWidth * frequency + settings.offset,
                (z + 0.1f) / VoxelData.ChunkWidth * frequency + settings.offset
            );

            noiseValue += perlin * octaveAmplitude;

            octaveAmplitude *= settings.persistence;
            frequency *= settings.lacunarity;
        }
        return (noiseValue * biome.amplitude) + biome.baseHeight;
    }

    public static float GenerateTemperature(float x, float z, ClimateNoiseSettings settings)
    {
        return Mathf.PerlinNoise(x * settings.temperatureScale + settings.temperatureOffset,z * settings.temperatureScale + settings.temperatureOffset);
    }

    public static float GenerateHumidity(float x, float z, ClimateNoiseSettings settings)
    {
        return Mathf.PerlinNoise( x * settings.humidityScale + settings.humidityOffset,z * settings.humidityScale + settings.humidityOffset);
    }
}
