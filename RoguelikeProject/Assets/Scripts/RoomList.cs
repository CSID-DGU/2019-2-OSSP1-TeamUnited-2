using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType {common, boss, treasure, horde}

[System.Serializable]
public class RoomList
{
    public Room[] Rooms;
}

[System.Serializable]
public class Room
{
    public RoomType type;
    public int weight;
}   

