using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkRenderer : MonoBehaviour
{
    private Chunk chunk;
    private World world;
    private GameObject chunkObj;

    List<Vector3> vertices = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();
    List<int> triangles = new List<int>(); // Single triangle list for atlas

    Mesh mesh;
    Vector3 worldPosition;

    public Material atlasMaterial; // The material that uses the texture atlas

    // Atlas configuration
    private int atlasColumns = 4;
    private int atlasRows = 4;
    // private int tilePixelSize = 16; // each cell is 16x16 pixels

    void Start()
    {
        // assign atlas material
        var mr = GetComponent<MeshRenderer>();
        mr.material = atlasMaterial;

        // MeshCollider mc = GetComponent<MeshCollider>();
        // mc.sharedMesh = null;     // clear old one
        // mc.sharedMesh = mesh;
    }

    public void Initialize(Chunk newChunk, World newWorld, GameObject newChunkObj)
    {
        this.chunk = newChunk;
        this.world = newWorld;
        this.chunkObj = newChunkObj;
        worldPosition = transform.position;
    }

    public void GenerateChunk()
    {
        GenerateMeshData();
        GenerateMesh();
    }

    public void GenerateMeshData()
    {
        vertices.Clear();
        uvs.Clear();
        triangles.Clear();

        for (int x = 0; x < VoxelData.ChunkWidth; x++)
        {
            for (int y = 0; y < VoxelData.ChunkHeight; y++)
            {
                for (int z = 0; z < VoxelData.ChunkDepth; z++)
                {
                    if(world.blockTypes[chunk.blocks[x,y,z]].isSolid){
                        CreateBlock(x, y, z);
                    }
                }
            }
        }

    }

    public void GenerateMesh()
    {
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.SetTriangles(triangles.ToArray(), 0);

        mesh.RecalculateNormals();

        MeshFilter mf = GetComponent<MeshFilter>();
        mf.mesh = mesh;
        MeshCollider mc = chunkObj.AddComponent<MeshCollider>();
        mc.sharedMesh = mf.mesh;
    }

    void CreateBlock(int x, int y, int z)
    {
        for (int i = 0; i < 6; i++)
        {
            if (ShouldRenderFace(x, y, z, i))
                AddFace(x, y, z, VoxelData.faces[i], chunk.blocks[x,y,z], i);
        }
    }

    bool ShouldRenderFace(int x, int y, int z, int faceIndex)
    {
        Vector3Int dir = VoxelData.faceDirection[faceIndex];

        int nx = x + dir.x;
        int ny = y + dir.y;
        int nz = z + dir.z;

        // if neighbour is inside this chunk
        if (chunk.IsBlockInChunk(nx, ny, nz))
        {
            int neighborID = chunk.blocks[nx, ny, nz];
            if (!world.blockTypes.TryGetValue(neighborID, out BlockType neighborBlock))
            {
                return true;
            }

            return !neighborBlock.isSolid;
        }
        // if neighbour is outside this chunk
        int worldX = Mathf.FloorToInt(worldPosition.x) + nx;
        int worldY = ny;
        int worldZ = Mathf.FloorToInt(worldPosition.z) + nz;

        return !world.IsBlockSolid(worldX, worldY, worldZ);
    }

    void AddFace(int x, int y, int z, int[] face, int blockType, int faceIndex)
    {
        int vertIndex = vertices.Count;
        // adding vertices
        foreach (int i in face)
            vertices.Add(VoxelData.cubeVertices[i] + new Vector3(x, y, z));

        // adding uvs
        // determine UVs based on atlas, mapped to face vertex order so orientation is correct
        if (!world.blockTypes.TryGetValue(blockType, out BlockType block))
            return; // air or invalid

        Vector2[] faceUVs = GetUVsFromAtlas(block.GetTextureID(faceIndex));
        uvs.AddRange(faceUVs);

        // adding triangles (single mesh now)
        triangles.Add(vertIndex);
        triangles.Add(vertIndex + 1);
        triangles.Add(vertIndex + 2);

        triangles.Add(vertIndex);
        triangles.Add(vertIndex + 2);
        triangles.Add(vertIndex + 3);
    }

    //returns UVs for a tile in order: BL, BR, TR, TL
    Vector2[] GetUVsFromAtlas(int textureID)
    {
        float cellWidth = 1f / atlasColumns;
        float cellHeight = 1f / atlasRows;

        // ID 0 -> (0,0)
        // ID 1 -> (1,0)
        // ID 2 -> (2,0)
        // ID 3 -> (3,0)
        // ID 4 -> (0,1)
        // ID 5 -> (1,1)
        
        int column = textureID % atlasColumns;
        int row = textureID / atlasColumns;

        float xMin = column * cellWidth;
        float yMin = row * cellHeight;

        float xMax = xMin + cellWidth;
        float yMax = yMin + cellHeight;

        return new Vector2[]
        {
            new Vector2(xMin, yMin), // bottom-left
            new Vector2(xMin, yMax), // top-left
            new Vector2(xMax, yMax), // top-right
            new Vector2(xMax, yMin)  // bottom-right
        };
    }
}
