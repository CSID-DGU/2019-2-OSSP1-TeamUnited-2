using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType {common, boss, treasure, horde}

[System.Serializable]
public class RoomList
{
    public Room[] rooms;
}

[System.Serializable]
public class Room
{
    public RoomType type;
    public int weight;
    public EnemyPool[] enemies;
    public ItemPool[] items;
}

[System.Serializable]
public class EnemyPool
{
    public GameObject enemy;
    public int weight;
}

[System.Serializable]
public class ItemPool
{
    public GameObject item;
    public int weight;
}

