using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomCategory {home, boss, common, treasure}

[System.Serializable]
public class RoomType
{
    public RoomCategory type;
    public int guaranteedAmount;
    public int weight;
    public int enemySpawnCostTotal;
    public EnemyType[] enemies;
    public int itemSpawnCostTotal;
    public ItemType[] items;
}

[System.Serializable]
public class EnemyType
{
    public GameObject type;
    public int cost;
    public int weight;
    public int guaranteedAmount;
}

[System.Serializable]
public class ItemType
{
    public GameObject type;
    public int cost;
    public int weight;
    public int guaranteedAmount;
}

