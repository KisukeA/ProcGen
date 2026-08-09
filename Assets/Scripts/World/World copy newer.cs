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

//     public Vector3 spawnPosition;

//     public Dictionary<int, BlockType> blockTypes { get; private set; }  
//     public Dictionary<ChunkCoord, Chunk> chunks = new();
//     public Dictionary<ChunkCoord, ChunkRenderer> renderers = new();

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
//         spawnPosition = new Vector3((WorldChunkSize * VoxelData.ChunkWidth) / 2f, VoxelData.ChunkHeight + 3, (WorldChunkSize * VoxelData.ChunkDepth) / 2f);
//         currentPlayerChunk = GetChunkFromPosition(player.position);
//         GenerateWorld();
//     }

//     void Update()
//     {
//         ChunkCoord newChunk = GetChunkFromPosition(player.position);

//         if (!newChunk.IsEqual(currentPlayerChunk))
//         {
//             currentPlayerChunk = newChunk;
//             UpdateVisibleChunks(currentPlayerChunk);
//         }
//     }

//     public void GenerateWorld()
//     {
//         player.position = spawnPosition;

//         ChunkCoord center = GetChunkFromPosition(player.position);

//         UpdateVisibleChunks(center);

//     }

//     void CreateChunk(ChunkCoord coord)
//     {
//         Vector3 pos = new Vector3(
//             coord.x * VoxelData.ChunkWidth,
//             0,
//             coord.z * VoxelData.ChunkDepth
//         );

//         Chunk chunk = new Chunk();
//         // chunk.FillTest();
//         chunk.GenerateTerrain(
//             coord.x * VoxelData.ChunkWidth,
//             coord.z * VoxelData.ChunkDepth,
//             noiseSettings
//         );
//         chunks.Add(coord, chunk);

//         GameObject chunkObj = Instantiate(chunkPrefab, pos, Quaternion.identity, transform);
//         ChunkRenderer cr = chunkObj.GetComponent<ChunkRenderer>();

//         cr.Initialize(chunk, this);
//         cr.GenerateChunk();

//         renderers.Add(coord, cr);
//     }

//     void UpdateVisibleChunks(ChunkCoord center)
//     {
//         for (int cx = center.x - RenderChunkDistance; cx < center.x + RenderChunkDistance; cx++)
//         {
//             for (int cz = center.z - RenderChunkDistance; cz < center.z + RenderChunkDistance; cz++)
//             {   

// //         for (int cx = Mathf.Max((WorldChunkSize / 2) - RenderChunkDistance, 0); cx < Mathf.Min((WorldChunkSize / 2) + RenderChunkDistance, WorldChunkSize); cx++)
// //         {
// //             for (int cz = Mathf.Max((WorldChunkSize / 2) - RenderChunkDistance, 0); cz < Mathf.Min((WorldChunkSize / 2) + RenderChunkDistance, WorldChunkSize); cz++)
// //             {    
//                 ChunkCoord coord = new ChunkCoord(cx, cz);

//                 //skip chunks out of bounds
//                 if (!isChunkInWorld(coord))
//                     continue;

//                 // create chunk only if its missing
//                 if (!chunks.ContainsKey(coord))
//                 {
//                     CreateChunk(coord);
//                 }

//                 // ensure its visible
//                 renderers[coord].gameObject.SetActive(true);
//             }
//         }

//         // disable chunks out of render distance
//         foreach (var cr in renderers)
//         {
//             int dx = Mathf.Abs(cr.Key.x - center.x);
//             int dz = Mathf.Abs(cr.Key.z - center.z);

//             if (dx > RenderChunkDistance || dz > RenderChunkDistance)
//                 cr.Value.gameObject.SetActive(false);
//         }
//     }

//     //helpers

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
//             coord.x >= 0 && coord.x < WorldChunkSize &&
//             coord.z >= 0 && coord.z < WorldChunkSize 
//         ){
//             return true;
//         }
//         else{
//             return false;
//         }
//     }

//     public ChunkCoord GetChunkFromPosition (Vector3 pos){
//         int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
//         int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkDepth);

//         return new ChunkCoord(x,z);
//     }

// // inspector
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

// //extra classes

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

//     public override bool Equals(object obj)
//     {
//         if (!(obj is ChunkCoord)) return false;
//         ChunkCoord other = (ChunkCoord)obj;
//         return x == other.x && z == other.z;
//     }

//     public override int GetHashCode()
//     {
//         unchecked
//         {
//             return (x * 397) ^ z;
//         }
//     }

// }
