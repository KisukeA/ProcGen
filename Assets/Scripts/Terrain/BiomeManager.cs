using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BiomeManager : MonoBehaviour
{
    [Header("Biomes")]
    public BiomeDefinition[] biomeDefinitions;

    [Header("Climate Noise")]
    public ClimateNoiseSettings climateSettings;

    [Header("Blending")]
    public float biomeBlendRange = 0.1f;

    public Biome GetBiome(int worldX, int worldZ)
    {
        float temperature = Noise.GenerateTemperature(
            worldX,
            worldZ,
            climateSettings
        );

        float humidity = Noise.GenerateHumidity(
            worldX,
            worldZ,
            climateSettings
        );

        // return SelectBiome(temperature, humidity); //below is seelct biome logic, if needed to be seperated, do so

        foreach (BiomeDefinition definition in biomeDefinitions)
        {
            bool temperatureMatch =
                temperature >= definition.minTemperature &&
                temperature <= definition.maxTemperature;

            bool humidityMatch =
                humidity >= definition.minHumidity &&
                humidity <= definition.maxHumidity;

            if (temperatureMatch && humidityMatch)
            {
                return definition.biome;
            }
        }

        Debug.LogWarning(
            $"No biome found for temperature {temperature:F2}, " +
            $"humidity {humidity:F2} at ({worldX}, {worldZ})"
        );

        return null;
    }

    public (Biome biome, float weight)[] GetBiomeWeights(int worldX, int worldZ)
    {
        float temperature = Noise.GenerateTemperature(worldX, worldZ, climateSettings);
        float humidity = Noise.GenerateHumidity(worldX, worldZ, climateSettings);

        List<(Biome biome, float weight)> weights = new List<(Biome biome, float weight)>();

        foreach (BiomeDefinition definition in biomeDefinitions)
        {
            float temperatureWeight = CalculateRangeWeight(temperature, definition.minTemperature, definition.maxTemperature, biomeBlendRange);
            float humidityWeight = CalculateRangeWeight(humidity, definition.minHumidity, definition.maxHumidity, biomeBlendRange);
            float weight = temperatureWeight * humidityWeight;

            if (weight > 0f)
            {
                weights.Add((definition.biome, weight));
            }
        }

        // make sure there is always at least one biome
        if (weights.Count == 0)
        {
            weights.Add((biomeDefinitions[0].biome, 1f));
        }
        NormalizeWeights(weights);
        return weights.ToArray();
    }


    private float CalculateRangeWeight(float value, float min, float max, float blendRange)
    {
        if (value >= min && value <= max)
            return 1f;

        if (value < min)
        {
            float distance = min - value;

            if (distance >= blendRange)
                return 0f;

            float t = 1f - distance / blendRange;

            return t * t * (3f - 2f * t);
        }

        float upperDistance = value - max;
        if (upperDistance >= blendRange)
            return 0f;
        float upperT = 1f -  upperDistance / blendRange;
        return upperT * upperT * (3f - 2f * upperT);
    }


    private void NormalizeWeights(List<(Biome biome, float weight)> weights)
    {
        float totalWeight = 0f;

        foreach (var entry in weights)
        {
            totalWeight += entry.weight;
        }
        if (totalWeight <= 0f)
            return;
        for (int i = 0; i < weights.Count; i++)
        {
            var entry = weights[i];
            weights[i] = (entry.biome, entry.weight / totalWeight);
        }
    }
}

[System.Serializable]
public class ClimateNoiseSettings
{
    public float temperatureScale = 0.001f;
    public float temperatureOffset = 50f;

    public float humidityScale = 0.001f;
    public float humidityOffset = 1000f;
}