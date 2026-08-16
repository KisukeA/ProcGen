using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour
{
    [Header("Chunk Game Object")]
    public GameObject chunkPrefab;
    // how many chunks to generate in X and Z directions
    [Header("Maximum World Size")]
    public int WorldChunkSize = 1;
    [Header("Noise Settings")]
    public NoiseSettings noiseSettings;
    [Header("Block Types")]
    public BlockType[] blockTypeAssets;
    [Header("Render Distance")]
    public int RenderChunkDistance = 1;
    [Header("Player")]
    public Transform player;
    // [Header("Biomes")]
    // public BiomeDefinition[] biomeDefinitions;
    [Header("Biome Manager")]
    public BiomeManager biomeManager;

    private Vector3 spawnPosition;
    ChunkCoord currentPlayerChunk;

    private ChunkGenerator chunkGenerator;

    public Dictionary<int, BlockType> blockTypes { get; private set; }  
    public Dictionary<ChunkCoord, Chunk> chunks = new();
    public Dictionary<ChunkCoord, ChunkRenderer> renderers = new();

    public int seed;

    public int WorldBlockSize {
        get { return WorldChunkSize * VoxelData.ChunkWidth;}
    }

    void Awake()
    {
        chunkGenerator = new ChunkGenerator(noiseSettings, biomeManager);

        blockTypes = new Dictionary<int, BlockType>();

        foreach (var bt in blockTypeAssets)
        {
            if (blockTypes.ContainsKey(bt.mapKey))
            {
                Debug.LogError($"Duplicate BlockType key {bt.mapKey}");
                continue;
            }

            blockTypes.Add(bt.mapKey, bt);
        }
    }

    void Start()
    {
        Random.InitState(seed);
        spawnPosition = new Vector3((WorldChunkSize * VoxelData.ChunkWidth) / 2f, VoxelData.ChunkHeight / 2f, (WorldChunkSize * VoxelData.ChunkDepth) / 2f);
        player.position = spawnPosition;
        currentPlayerChunk = GetChunkFromPosition(player.position);
        GenerateWorld();
    }

    void Update()
    {
        ChunkCoord newChunk = GetChunkFromPosition(player.position);

        if (!newChunk.IsEqual(currentPlayerChunk))
        {
            currentPlayerChunk = newChunk;
            // UpdateVisibleChunks(currentPlayerChunk);
            GenerateWorld();
        }
    }

    public void GenerateWorld()
    {
        ChunkCoord center = GetChunkFromPosition(player.position);

        // first load all chunk data
        LoadChunkData(center);

        // then build all meshes
        BuildChunkMeshes(center);

        // lastly update visibility 
        UpdateVisibleChunks(center);

    }

    void LoadChunkData(ChunkCoord center)
    {
        for (int cx = center.x - RenderChunkDistance; cx <= center.x + RenderChunkDistance; cx++)
        {
            for (int cz = center.z - RenderChunkDistance; cz <= center.z + RenderChunkDistance; cz++)
            {
                ChunkCoord coord = new ChunkCoord(cx, cz);

                if (!isChunkInWorld(coord))
                    continue;

                if (!chunks.ContainsKey(coord))
                {
                    CreateChunkData(coord);
                }
            }
        }
    }

    void BuildChunkMeshes(ChunkCoord center)
    {
        for (int cx = center.x - RenderChunkDistance; cx <= center.x + RenderChunkDistance; cx++)
        {
            for (int cz = center.z - RenderChunkDistance; cz <= center.z + RenderChunkDistance; cz++)
            {
                ChunkCoord coord = new ChunkCoord(cx, cz);

                if (!chunks.ContainsKey(coord))
                    continue;

                if (!renderers.ContainsKey(coord))
                {
                    CreateChunkRenderer(coord);
                }
                // else
                // {
                //     renderers[coord].gameObject.SetActive(true);
                // }
            }
        }
    }

    void CreateChunkData(ChunkCoord coord)
    {
        Chunk chunk = new Chunk(coord);
        chunkGenerator.GenerateChunk(chunk, coord);
        chunks.Add(coord, chunk);
    }

    void CreateChunkRenderer(ChunkCoord coord){
        Vector3 pos = new Vector3(
            coord.x * VoxelData.ChunkWidth,
            0,
            coord.z * VoxelData.ChunkDepth
        );

        GameObject chunkObj = Instantiate(chunkPrefab, pos, Quaternion.identity, transform);
        ChunkRenderer cr = chunkObj.GetComponent<ChunkRenderer>();

        cr.Initialize(chunks[coord], this, chunkObj);
        cr.GenerateChunk();

        renderers.Add(coord, cr);
    }

    void UpdateVisibleChunks(ChunkCoord center)
    {
        // Disable out-of-range
        foreach (var cr in renderers)
        {
            int dx = Mathf.Abs(cr.Key.x - center.x);
            int dz = Mathf.Abs(cr.Key.z - center.z);

            bool visible = dx <= RenderChunkDistance && dz <= RenderChunkDistance;
            cr.Value.gameObject.SetActive(visible);
        }
    }

    public bool IsBlockSolid(int worldX, int worldY, int worldZ)
    {
        int blockID = GetBlock(worldX, worldY, worldZ);

        if (!blockTypes.TryGetValue(blockID, out BlockType block))
            return false;

        return block.isSolid;
    }

    public int GetBlock(int worldX, int worldY, int worldZ)
    {
        // outside the world = air
        if (!IsBlockInWorld(worldX, worldY, worldZ))
            return 3; // block id 3 is air

        ChunkCoord coord = GetChunkFromPosition(new Vector3(worldX, 0, worldZ));

        if (!chunks.TryGetValue(coord, out Chunk chunk))
            return 3; // treat unloaded chunk as air

        int localX = worldX - coord.x * VoxelData.ChunkWidth;
        int localZ = worldZ - coord.z * VoxelData.ChunkDepth;

        return chunk.blocks[localX, worldY, localZ];
    }

    //helpers

    public bool IsBlockInWorld(int x, int y, int z){
        if(
            x >= 0 && x < WorldBlockSize &&
            y >= 0 && y < VoxelData.ChunkHeight &&
            z >= 0 && z < WorldBlockSize
        ){
            return true;
        }
        else{
            return false;
        }
    }

    bool isChunkInWorld(ChunkCoord coord){
        if(
            coord.x >= 0 && coord.x < WorldChunkSize &&
            coord.z >= 0 && coord.z < WorldChunkSize 
        ){
            return true;
        }
        else{
            return false;
        }
    }

    public ChunkCoord GetChunkFromPosition (Vector3 pos){
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkDepth);

        return new ChunkCoord(x,z);
    }

}