using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomList
{
    public RoomType[] Rooms;
}

[System.Serializable]
public class RoomType
{
    public string type;
    public int weight;
}   

