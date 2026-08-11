using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockType
{
    public string name;
    public bool isSolid;
    public int mapKey;

    public int frontTexture;
    public int rightTexture;
    public int backTexture;
    public int leftTexture;
    public int topTexture;
    public int bottomTexture;

    // indexes order is: front, right, back, left, top, bottom

    public int GetTextureID (int faceIndex){
        switch(faceIndex){
            case 0:
                return frontTexture;
            case 1:
                return rightTexture;
            case 2:
                return backTexture;
            case 3:
                return leftTexture;
            case 4:
                return topTexture;
            case 5:
                return bottomTexture;
            default: 
                return 0;
        }
    }
}
