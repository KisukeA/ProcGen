using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkCoord {
    public int x;
    public int z;

    public ChunkCoord (int newX, int newZ) {
        x = newX;
        z = newZ;
    }

    public bool IsEqual(ChunkCoord coord) {

        if (coord == null)
            return false;
        else if (coord.x == x && coord.z == z)
            return true;
        else
            return false;

    }

    public override bool Equals(object obj)
    {
        if (!(obj is ChunkCoord)) return false;
        ChunkCoord other = (ChunkCoord)obj;
        return x == other.x && z == other.z;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            //random function, it could be anything but this has to be overrode
            return (x * 397) ^ z;
        }
    }

}

