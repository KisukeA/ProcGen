// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;

// public class World : MonoBehaviour
// {
//     [Header("Chunk Game Object")]
//     public GameObject chunkPrefab;
//     // how many chunks to generate in X and Z directions
//     [Header("Maximum World Size")]
//     public int WorldChunkSize = 1;
//     [Header("Noise Settings")]
//     public NoiseSettings noiseSettings;
//     [Header("Block Types")]
//     public BlockType[] blockTypeAssets;
//     [Header("Render Distance")]
//     public int RenderChunkDistance = 1;
//     [Header("Player")]
//     public Transform player;


//     public Dictionary<int, BlockType> blockTypes { get; private set; }  
//     private Dictionary<ChunkCoord, Chunk> chunks = new();
//     private Dictionary<ChunkCoord, ChunkRenderer> renderers = new();

//     ChunkCoord currentPlayerChunk;

//     public int WorldBlockSize {
//         get { return WorldChunkSize * VoxelData.ChunkWidth;}
//     }

//     void Awake()
//     {
//         blockTypes = new Dictionary<int, BlockType>();

//         foreach (var bt in blockTypeAssets)
//         {
//             if (blockTypes.ContainsKey(bt.mapKey))
//             {
//                 Debug.LogError($"Duplicate BlockType key {bt.mapKey}");
//                 continue;
//             }

//             blockTypes.Add(bt.mapKey, bt);
//         }
//     }

//     void Start()
//     {
//         GenerateWorld();
//     }

//     public void GenerateWorld()
//     {
//         // delete old chunks
//         // foreach (Transform obj in this.transform){
//         //     Destroy(obj);
//         // }

//         // generate new chunks
//         for (int cx = Mathf.Max((WorldChunkSize / 2) - RenderChunkDistance, 0); cx < Mathf.Min((WorldChunkSize / 2) + RenderChunkDistance, WorldChunkSize); cx++)
//         {
//             for (int cz = Mathf.Max((WorldChunkSize / 2) - RenderChunkDistance, 0); cz < Mathf.Min((WorldChunkSize / 2) + RenderChunkDistance, WorldChunkSize); cz++)
//             {
//                 Vector3 pos = new Vector3(
//                     cx * VoxelData.ChunkWidth,
//                     0,
//                     cz * VoxelData.ChunkDepth
//                 );

//                 ChunkCoord coord = new ChunkCoord(cx,cz);
                
//                 // create chunk data
//                 Chunk chunk = new Chunk();
//                 // fill block array using Perlin noise + settings
//                 // cr.chunk.GenerateTerrain(
//                 //     cx * VoxelData.ChunkWidth,
//                 //     cz * VoxelData.ChunkDepth,
//                 //     noiseSettings
//                 // );
//                 chunk.FillTest();
//                 chunks.Add(coord, chunk);

//                 // create chunk renderer
//                 GameObject chunkObj = Instantiate(chunkPrefab, pos, Quaternion.identity);
//                 chunkObj.transform.SetParent(this.transform);
//                 ChunkRenderer cr = chunkObj.GetComponent<ChunkRenderer>();

//                 cr.Initialize(chunk,this);
//                 cr.GenerateChunk();
//                 renderers.Add(coord, cr);
//             }
//         }
//     }

//     void CreateChunk(ChunkCoord coord)
//     {
//         Vector3 pos = new Vector3(
//             coord.x * VoxelData.ChunkWidth,
//             0,
//             coord.z * VoxelData.ChunkDepth
//         );

//         Chunk chunk = new Chunk();
//         chunk.FillTest();
//         chunks.Add(coord, chunk);

//         GameObject chunkObj = Instantiate(chunkPrefab, pos, Quaternion.identity, transform);
//         ChunkRenderer cr = chunkObj.GetComponent<ChunkRenderer>();

//         cr.Initialize(chunk, this);
//         cr.GenerateChunk();

//         renderers.Add(coord, cr);
//     }

//     public bool isBlockInWorld(int x, int y, int z){
//         if(
//             x >= 0 && x < WorldBlockSize &&
//             y >= 0 && y < VoxelData.ChunkHeight &&
//             z >= 0 && z < WorldBlockSize
//         ){
//             return true;
//         }
//         else{
//             return false;
//         }

//     }

//     bool isChunkInWorld(ChunkCoord coord){
//         if(
//             coord.x >= 0 && coord.x <= WorldChunkSize &&
//             coord.z >= 0 && coord.z <= WorldChunkSize 
//         ){
//             return true;
//         }
//         else{
//             return false;
//         }
//     }

//     ChunkCoord GetChunkFromPosition (Vector3 pos){
//         int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
//         int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkDepth);

//         return new ChunkCoord(x,z);
//     }

// #if UNITY_EDITOR
//     // regenerate only DURING PLAY MODE
//     // private bool needsRebuild = false;

//     // private void OnValidate()
//     // {
//     //     if (Application.isPlaying)
//     //         needsRebuild = true;
//     // }

//     // private void Update()
//     // {
//     //     if (Application.isPlaying && needsRebuild)
//     //     {
//     //         needsRebuild = false;
//     //         GenerateWorld();
//     //     }
//     // }
// #endif
// }

// [System.Serializable]
// public class BlockType
// {
//     public string name;
//     public bool isSolid;
//     public int mapKey;

//     public Vector2Int frontTexture;
//     public Vector2Int rightTexture;
//     public Vector2Int backTexture;
//     public Vector2Int leftTexture;
//     public Vector2Int topTexture;
//     public Vector2Int bottomTexture;

//     // indexes order is: front, right, back, left, top, bottom

//     public Vector2Int GetTextureCoord (int faceIndex){
//         switch(faceIndex){
//             case 0:
//                 return frontTexture;
//             case 1:
//                 return rightTexture;
//             case 2:
//                 return backTexture;
//             case 3:
//                 return leftTexture;
//             case 4:
//                 return topTexture;
//             case 5:
//                 return bottomTexture;
//             default: 
//                 return new Vector2Int(0,0);
//         }
//     }
// }

// public class ChunkCoord {

//     public int x;
//     public int z;

//     public ChunkCoord (int newX, int newZ) {

//         x = newX;
//         z = newZ;

//     }

//     public bool IsEqual(ChunkCoord coord) {

//         if (coord == null)
//             return false;
//         else if (coord.x == x && coord.z == z)
//             return true;
//         else
//             return false;

//     }

// }
