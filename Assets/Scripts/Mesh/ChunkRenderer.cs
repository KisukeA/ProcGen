using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkRenderer : MonoBehaviour
{
    private Chunk chunk;
    private World world;
    private NoiseSettings settings;

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

    public void Initialize(Chunk newChunk, World newWorld, NoiseSettings newSettings)
    {
        this.chunk = newChunk;
        this.world = newWorld;
        this.settings = newSettings;
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

        GetComponent<MeshFilter>().mesh = mesh;
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
        // get direction to check for each face
        Vector3Int dir = VoxelData.faceDirection[faceIndex];

        int nx = x + dir.x;
        int ny = y + dir.y;
        int nz = z + dir.z;

        // check whether above-mentioned face is at the edge of this chunk(visible),
        // meaning it has no neighbor in this chunk
        if (!chunk.isBlockInChunk(nx,ny,nz)){
            // //if it is, we check the neighboring chunk
            // to check for neighboring chunk, we only render if the block is on the edge of the world, for now
            int worldX = Mathf.FloorToInt(worldPosition.x) + nx;
            int worldY = ny;
            int worldZ = Mathf.FloorToInt(worldPosition.z) + nz;

            if (!world.isBlockInWorld(worldX, worldY, worldZ)){
                return true;
            }
            // // Find which chunk the neighbor block belongs to
            // ChunkCoord neighborChunkCoord = world.GetChunkFromPosition(new Vector3(worldX,0,worldZ));

            // // If chunk not loaded → treat as air → render face
            // if (!world.chunks.TryGetValue(neighborChunkCoord, out Chunk neighborChunk)){
            //     // if(neighborChunkCoord.Equals(new ChunkCoord(21,21))){
            //     //     Debug.Log("we are here");
            //     // }
            //     return false;
            //     // return true;
            // }
            // // Convert world position to neighbor local coords
            // int localX = worldX - neighborChunkCoord.x * VoxelData.ChunkWidth;
            // int localZ = worldZ - neighborChunkCoord.z * VoxelData.ChunkDepth;

            // // Fetch neighbor block
            // int neighborId = neighborChunk.blocks[localX, worldY, localZ];

            // // Render face only if neighbor is NON-solid
            // return !world.blockTypes[neighborId].isSolid;

            //test logic
            // int worldX = Mathf.FloorToInt(worldPosition.x) + nx;
            // int worldY = ny;
            // int worldZ = Mathf.FloorToInt(worldPosition.z) + nz;

            int heightInt = Mathf.FloorToInt(Noise.GenerateHeight(worldX, worldZ, settings));

            int neighborId = chunk.GetBlock(worldY,heightInt);
            return !world.blockTypes[neighborId].isSolid;
        }

        // if not, meaning it's somewhere inside the chunk,
        // we check whether the neighbor of that face is solid, and if it is we dont render the face
        int neighborKey = chunk.blocks[nx, ny, nz];
            // check if we even have the neighboring block saved in our block types
        if (!world.blockTypes.TryGetValue(neighborKey, out BlockType neighborBlock)){
            return true; // if not, treat unknown as air and render this face
        }
        return !neighborBlock.isSolid; // if neighbor is solid we don't render this face
    }

    void AddFace(int x, int y, int z, int[] face, int blockType, int faceIndex)
    {
        int vertIndex = vertices.Count;
        // adding vertices
        foreach (int i in face)
            vertices.Add(VoxelData.cubeVertices[i] + new Vector3(x, y, z));

        // adding uvs
        // Determine UVs based on atlas, mapped to face vertex order so orientation is correct
        if (!world.blockTypes.TryGetValue(blockType, out BlockType block))
            return; // air or invalid

        Vector2[] faceUVs = GetUVsFromAtlas(
            block.GetTextureCoord(faceIndex)
        );
        // Vector2[] faceUVs = GetUVsFromAtlas(Array.Find(world.blockTypes, el => el.mapKey == blockType).GetTextureCoord(faceIndex));
        uvs.AddRange(faceUVs);

        // adding triangles (single mesh now)
        triangles.Add(vertIndex);
        triangles.Add(vertIndex + 1);
        triangles.Add(vertIndex + 2);

        triangles.Add(vertIndex);
        triangles.Add(vertIndex + 2);
        triangles.Add(vertIndex + 3);
    }

    // string GetTextureName(int blockType, int faceIndex)
    // {
    //     return blockType switch
    //     {
    //         1 => "dirt",
    //         2 => (faceIndex == 4) ? "grasstop" : (faceIndex == 5) ? "dirt" : "grassside",
    //         3 => "stone",
    //         4 => "water",
    //         5 =>(faceIndex == 4 || faceIndex == 5) ? "furnacevert" : (faceIndex == 0) ? "furnacefront" : "furnaceside",
    //         _ => "air"
    //     };
    // }

    // Returns the 4 UVs for a tile in canonical corner order: BL, BR, TR, TL
    Vector2[] GetUVsFromAtlas(Vector2Int textureAtlasCoords)
    {
        float cellWidth = 1f / atlasColumns;
        float cellHeight = 1f / atlasRows;

        // // Map names to atlas (column, row) (starting bottom left in Unity)
        // Dictionary<string, Vector2Int> atlasPos = new Dictionary<string, Vector2Int>()
        // {
        //     {"grasstop",  new Vector2Int(3, 2)},
        //     {"grassside", new Vector2Int(2, 3)},
        //     {"stone",     new Vector2Int(0, 3)},
        //     {"dirt",      new Vector2Int(1, 3)},
        //     {"water",     new Vector2Int(0, 0)},
        //     {"furnaceside", new Vector2Int(1, 0)},
        //     {"furnacefront", new Vector2Int(0, 0)},
        //     {"furnacevert", new Vector2Int(3, 0)}
        // };

        Vector2Int cell = textureAtlasCoords;

        float xMin = cell.x * cellWidth;
        float yMin = cell.y * cellHeight;
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
