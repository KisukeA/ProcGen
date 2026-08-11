using UnityEngine;

[CreateAssetMenu(menuName = "World/Biome")]
public class Biome : ScriptableObject
{
    public string biomeName;

    [Header("Terrain")]
    public float baseHeight;
    public float amplitude;
    public float scale;

    [Header("Surface")]
    public int surfaceBlock;
    public int subsurfaceBlock;
};

[System.Serializable]
public class BiomeDefinition
{
    public Biome biome;

    [Header("Temperature Range")]
    [Range(0f, 1f)]
    public float minTemperature;
    [Range(0f, 1f)]
    public float maxTemperature;

    [Header("Humidity Range")]
    [Range(0f, 1f)]
    public float minHumidity;
    [Range(0f, 1f)]
    public float maxHumidity;
}