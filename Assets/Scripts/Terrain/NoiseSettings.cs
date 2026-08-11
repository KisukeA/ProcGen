using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    [Range(0.001f, 1f)] public float scale = 0.01f;
    [Range(1, 8)] public int octaves = 4;
    [Range(0, 1000)] public int offset = 0;
    [Range(0.1f, 4f)] public float lacunarity = 2f;
    [Range(0f, 1f)] public float persistence = 0.5f;
    // public AnimationCurve heightCurve;

    public float amplitude = 50f;
    public int baseLevel = 0;
}