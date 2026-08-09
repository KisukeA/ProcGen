using UnityEngine;

public static class VoxelData
{
    public const int ChunkWidth = 16;
    public const int ChunkHeight = 128;
    public const int ChunkDepth = 16;

    // Define cube face vertices
    public static readonly Vector3[] cubeVertices = new Vector3[8]
    {
        new Vector3(0,0,0),
        new Vector3(1,0,0),
        new Vector3(1,1,0),
        new Vector3(0,1,0),
        new Vector3(0,1,1),
        new Vector3(1,1,1),
        new Vector3(1,0,1),
        new Vector3(0,0,1)
    };

    // Which vertices compose each face
    public static readonly int[][] faces = new int[6][]
    {
        new int[]{0, 3, 2, 1}, // front
        new int[]{1, 2, 5, 6}, // right
        new int[]{6, 5, 4, 7}, // back
        new int[]{7, 4, 3, 0}, // left
        new int[]{3, 4, 5, 2}, // top
        new int[]{7, 0, 1, 6}  // bottom
    };

    public static readonly Vector3Int[] faceDirection = new Vector3Int[6]
    {
        new Vector3Int(0, 0, -1), // front
        new Vector3Int(1, 0, 0),  // right
        new Vector3Int(0, 0, 1),  // back
        new Vector3Int(-1, 0, 0), // left
        new Vector3Int(0, 1, 0),  // top
        new Vector3Int(0, -1, 0), // bottom
    };

}