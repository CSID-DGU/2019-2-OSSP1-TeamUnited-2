using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public double density;
    public int postsmooth;
    public GameObject boundary;
    public GameObject[] floor;
    public GameObject[] wall;
    public int mapHeight, mapWidth;
    public int minRoomSize, maxRoomSize;
    public GameObject corridorTile;
    public GameObject[,] floorPosition;
    public GameObject[,] tunnelPosition;
    public GameObject[,] corridorPosition;
    public GameObject[,] wallPosition;
    public GameObject[,] boundaryPosition;
    protected SubDungeon rootSubDungeon;
    protected List<SubDungeon> subDungeonList;
    public RoomType[] roomTypes;
    private Transform pos;
    private string seed;
    public void CreateBSP(SubDungeon subDungeon)
    {
        Debug.Log("Splitting sub-dungeon " + subDungeon.debugId + ": " + subDungeon.rect);
        if (subDungeon.IAmLeaf())
        {
            // if the sub-dungeon is too large split it
            if (subDungeon.rect.width > maxRoomSize
                || subDungeon.rect.height > maxRoomSize
                || Random.Range(0.0f, 1.0f) > 0.25)
            {

                if (subDungeon.Split(minRoomSize, maxRoomSize))
                {
                    Debug.Log("Splitted sub-dungeon " + subDungeon.debugId + " in "
                        + subDungeon.left.debugId + ": " + subDungeon.left.rect + ", "
                        + subDungeon.right.debugId + ": " + subDungeon.right.rect);

                    CreateBSP(subDungeon.left);
                    CreateBSP(subDungeon.right);
                }
            }
        }
    }
    public RoomType GetRandomRoomtypeFromPool()
    {
        // 전체 풀의 weight를 측정합니다.
        int totalWeight = 0;
        foreach(var room in roomTypes)
        {
            totalWeight += room.weight;
        }
        
        // 목표 지점을 잡습니다.
        int targetPoint = Random.Range(0, totalWeight);
        
        // 차례로 weight를 가산하며 목표 지점이 넘긴 시점에 room을 반환합니다.
        int currentPoint = 0;
        foreach(var room in roomTypes)
        {
            currentPoint += room.weight;
            if (currentPoint > targetPoint)
            {
                return room;
            }
        }

        // 제대로 생성이 되지 않았을 경우
        Debug.LogError("Critical error on room weight calculation : " + currentPoint + ", " + targetPoint);
        return null;
    }
    public RoomType GetRoomtypeFromPool(RoomCategory category)
    {
        foreach (var room in roomTypes)
        {
            if (room.type == category)
            {
                return room;
            }
        }

        Debug.LogError("Trouble on find spicified roomtype : " + category);
        return null;
    }

    public void SpawnObjectsToEveryDungeons()
    {
        foreach (SubDungeon dungeon in subDungeonList)
        {
            SpawnObjectsInDungeon(dungeon);
        }
    }

    public void SpawnObjectsInDungeon(SubDungeon dungeon)
    {
        RoomType roomType = dungeon.roomType;
        // 확정 수량 enemy 소환
        foreach (EnemyType enemy in roomType.enemies)
        {
            Debug.Log("spawning enemys..");
            if (enemy.guaranteedAmount > 0)
            {
                Debug.Log("spawning guaranteed amount..");
                for (int i = 0; i < enemy.guaranteedAmount; ++i)
                {
                    Debug.Log("random throw..");
                    RandomThrowObjectInRoom(dungeon, enemy.type);
                    Debug.Log("random throw complete");
                }
            }
        }

        // enemy 랜덤 소환 시작
        // 전체 풀의 weight 측정
        int enemyTotalWeight = 0;
        foreach (EnemyType enemy in roomType.enemies)
        {
            enemyTotalWeight += enemy.weight;
        }

        // 랜덤 수량 enemy 소환을 위한 총 비용
        int maxEnemyCost = roomType.maxEnemyCost;
        int spawnedEnemyPoint = 0;

        // 소환된 수량이 total cost를 넘을때까지 소환
        while (spawnedEnemyPoint < maxEnemyCost)
        {
            // weight 비율에 따라 소환
            bool spawned = false;
            int targetPoint = Random.Range(0, enemyTotalWeight);
            int currentPoint = 0;
            foreach (EnemyType enemy in roomType.enemies)
            {
                currentPoint += enemy.weight;
                if (currentPoint > targetPoint)
                {
                    spawnedEnemyPoint += enemy.cost;
                    // RandomThrowObjectInRoom(dungeon, enemy.type);
                    Debug.Log("Enemy Spawned : " + enemy.type.GetComponent<Unit>());
                    spawned = true;
                }
            }
            if (!spawned)
            {
                Debug.LogError("Trouble on spawning");
                break;
            }
        }

        // // 확정 수량 item 소환
        // foreach (EnemyType item in roomType.items)
        // {
        //     if (enemy.guaranteedAmount > 0)
        //     {
        //         foreach (int i in enemy.guaranteedAmount)
        //         {
        //             RandomThrowObjectInRoom(enemy.type);
        //         }
        //     }
        // }


        // // item 랜덤 소환 시작
        // // 전체 풀의 weight 측정
        // int itemTotalWeight = 0;
        // foreach (ItemType item in roomType.items)
        // {
        //     itemTotalWeight += item.weight;
        // }


    }

    public void DrawFloorsRecursive(SubDungeon subDungeon)
    {
        if (subDungeon == null)
        {
            return;
        }
        if (subDungeon.IAmLeaf())
        {
            for (int i = (int)subDungeon.rect.x; i < subDungeon.rect.xMax; i++)
            {
                for (int j = (int)subDungeon.rect.y; j < subDungeon.rect.yMax; j++)
                {
                    if (floorPosition[i, j] == null)
                    {
                        GameObject toInstantiate = floor[Random.Range(0, floor.Length)];
                        GameObject instance = Instantiate(toInstantiate, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(transform);
                        floorPosition[i, j] = instance;
                    }
                }
            }
        }
        else
        {
            DrawFloorsRecursive(subDungeon.left);
            DrawFloorsRecursive(subDungeon.right);
        }
    }
    public void DeployItems()
    {
        foreach (SubDungeon dungeon in subDungeonList)
        {
            foreach (GameObject item in dungeon.items)
            {
                RandomThrowObjectInRoom(dungeon, item);
            }
        }
    }
    public void SpawnEnemys()
    {
        foreach (SubDungeon dungeon in subDungeonList)
        {

        }
    }

    public bool IsSafeToDeploy(int x, int y)
    {
        if (wallPosition[x, y] || boundaryPosition[x, y])
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    
    public void RandomThrowObjectInRoom(SubDungeon dungeon, GameObject obj)
    // 방 내부에 랜덤하게 아이템, 적을 뿌립니다. 일단은 랜덤 버전만...
    {
        bool deployed = false;
        int trial = 0;
        while (!deployed)
        {
            int x = (int)(dungeon.room.x + Random.Range(0.0f, dungeon.room.width));
            int y = (int)(dungeon.room.y + Random.Range(0.0f, dungeon.room.height));
            Vector2 toDeployOn = new Vector2(x, y);
            if (IsSafeToDeploy(x, y))
            {
                GameObject instance = Instantiate(obj);
                instance.transform.SetParent(transform);
                instance.transform.position = new Vector3(x, y, -10);
                Debug.Log("Deploy on : " + x + ", " + y);
                deployed = true;
            }

            ++trial;
            if (trial > 100)
            {
                Debug.LogError("Got trouble in deploying object...");
                break;
            }
        }
    }

    void DrawCorridorsRecursive(SubDungeon subDungeon)
    {
        if (subDungeon == null)
        {
            return;
        }

        DrawCorridorsRecursive(subDungeon.left);
        DrawCorridorsRecursive(subDungeon.right);

        foreach (Rect corridor in subDungeon.corridors)
        {
            for (int i = (int)corridor.x; i < corridor.xMax; i++)
            {
                for (int j = (int)corridor.y; j < corridor.yMax; j++)
                {
                    GameObject instance = Instantiate(corridorTile, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(transform);
                    corridorPosition[i, j] = instance;
                }
            }
        }
    }

    void DrawBoundarysRecursive(SubDungeon subDungeon)
    {
        // 바운더리는 room 내부에 설치하지 않습니다.
        if (subDungeon.IAmLeaf())
        {
            return;
        }
    
        // 터널과 이미 설치된 곳을 제외한 모든 rect의 경계에 예외없이 바운더리를 설치합니다.
        for (int i = (int)subDungeon.rect.x; i < subDungeon.rect.xMax; i++)
        {
            for (int j = (int)subDungeon.rect.y; j < subDungeon.rect.yMax; j++)
            {
                if (boundaryPosition[i, j] == null && tunnelPosition[i, j] == null &&
                (i == (int)subDungeon.rect.x 
                || i == (int)subDungeon.rect.xMax - 1 
                || j == (int)subDungeon.rect.y 
                || j == (int)subDungeon.rect.yMax - 1
                || (subDungeon.partitionAlignment == SubDungeon.Alignment.horizontal && j == subDungeon.partition.y)
                || (subDungeon.partitionAlignment == SubDungeon.Alignment.vertical && i == subDungeon.partition.x)))
                {
                    GameObject instance = Instantiate(boundary, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(transform);
                    boundaryPosition[i, j] = instance;
                }
            }
        }

        //
        if (subDungeon.left != null)
        {
            DrawBoundarysRecursive(subDungeon.left);
        } 
        if (subDungeon.right != null)
        {
            DrawBoundarysRecursive(subDungeon.right);
        }
    }

    public void DrawTunnel(SubDungeon subDungeon)
    {
        foreach (Rect tunnel in subDungeon.tunnels)
        {
            for (int y = (int)tunnel.y; y < tunnel.yMax; ++y)
            {
                for (int x = (int)tunnel.x; x < tunnel.xMax; ++x)
                {
                    if (floorPosition[x,y])
                    {
                        // Debug.LogError("Try to draw tunnel on floor (tunnel should be deployed first)");
                        Destroy(floorPosition[x,y]);
                    }
                    GameObject toInstantiate = floor[Random.Range(0, floor.Length)];
                    GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(transform);
                    tunnelPosition[x, y] = instance;
                    floorPosition[x, y]  = instance;
                }
            }
        }
    }
    public void DrawTunnelRecursive(SubDungeon dungeon)
    {
        if (dungeon == null)
        {
            return;
        }
        DrawTunnel(dungeon);
        if (dungeon.left != null)
        {
            DrawTunnelRecursive(dungeon.left);
        }
        if (dungeon.right != null)
        {
            DrawTunnelRecursive(dungeon.right);
        }
    }

    public void StoreMapsIntoSubDungeonList()
    {
        subDungeonList = new List<SubDungeon>();
        SubDungeon nextinsert = rootSubDungeon.RandomPopDungeon();
        while (nextinsert != null)
        {
            subDungeonList.Add(nextinsert);
            nextinsert = rootSubDungeon.RandomPopDungeon();
        }
        Debug.Log(subDungeonList.Count);
    }

    public void SetRoomTypeForSubDungeons()
    {
        subDungeonList[0].roomType = GetRoomtypeFromPool(RoomCategory.home);
        subDungeonList[subDungeonList.Count - 1].roomType = GetRoomtypeFromPool(RoomCategory.boss);

        foreach (SubDungeon dungeon in subDungeonList)
        {
            if (dungeon.roomType == null)
            {
                dungeon.roomType = GetRandomRoomtypeFromPool(); 
            }
        }
    }

    public void FillRoomsRecursive(SubDungeon subDungeon)
    {
        if (subDungeon == null)
        {
            return;
        }
        // 자신이 실제 서브던전 일 경우
        if (subDungeon.IAmLeaf())
        {
            // 채우기를 위한 임시 맵 배열 생성
            int[,] map = new int[mapWidth, mapHeight];

            // rect는 전부 채우고 room은 밀도만큼 채웁니다.
            RandomFillMap(subDungeon.rect, subDungeon.room, map);

            // tunnel부분은 0으로 채워줍니다.
            for (int y = 0; y < mapHeight; ++y)
            {
                for (int x = 0; x < mapWidth; ++x)
                {
                    if (tunnelPosition[x, y] != null)
                    {
                        map[x, y] = 0;
                    }
                    if (boundaryPosition[x, y] != null)
                    {
                        map[x, y] = 1;
                    }
                }
            }
            for (int i = 0; i < postsmooth; ++i)
            {
                SmoothMapPsudo(subDungeon.rect, map);
            }

            for (int y = (int)subDungeon.rect.y; y < subDungeon.rect.yMax; ++y)
            {
                for (int x = (int)subDungeon.rect.x; x < subDungeon.rect.xMax; ++x)
                {
                    if (map[x,y] == 1 && boundaryPosition[x, y] == null)
                    {
                        GameObject toInstantiate = wall[Random.Range(0, wall.Length)];
                        toInstantiate.layer = LayerMask.NameToLayer("Wall");
                        GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(transform);
                        wallPosition[x, y] = instance;
                    }
                }
            }
        }
        // 자신이 복도일 경우 재귀 호출
        else
        {
            FillRoomsRecursive(subDungeon.left);
            FillRoomsRecursive(subDungeon.right);
        }
    }
    private void Start()
    {
        // 각종 맵 오브젝트를 정수좌표계에 연동해서 넣을 배열을 초기화합니다.
        tunnelPosition              = new GameObject[mapWidth, mapHeight];
        floorPosition               = new GameObject[mapWidth, mapHeight];
        corridorPosition            = new GameObject[mapWidth, mapHeight];
        wallPosition                = new GameObject[mapWidth, mapHeight];
        boundaryPosition            = new GameObject[mapWidth, mapHeight];

        // 루트 서브던전은 맵 전체크기로 생성합니다.        
        rootSubDungeon = new SubDungeon(new Rect(0, 0, mapWidth, mapHeight));

        // 전체 맵을 재귀호출하여 적당한 구획(변수명 rect)으로 나눠줍니다.
        CreateBSP(rootSubDungeon);

        // 재귀호출을 통해 서브던전의 변수 room과 corridor를 정의해줍니다.
        rootSubDungeon.CreateRoomRecursive();

        // 
        DrawTunnelRecursive(rootSubDungeon);
        DrawBoundarysRecursive(rootSubDungeon);
        // DrawCorridorsRecursive(rootSubDungeon);
        DrawFloorsRecursive(rootSubDungeon);
        FillRoomsRecursive(rootSubDungeon);
        StoreMapsIntoSubDungeonList();
        SetRoomTypeForSubDungeons();
        SpawnObjectsToEveryDungeons();

    }


    //------------------------------------------------------------------------------------------------------------------------------

    void RandomFillMap(Rect rect, Rect room, int[,] map)
    {
        seed = System.DateTime.Now.ToString();
        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        // rect 내부를 1로 채웁니다
        for (int x = (int)rect.x; x < rect.xMax; ++x)
        {
            for (int y = (int)rect.y; y < rect.yMax; ++y)
            {
                map[x, y] = (pseudoRandom.Next(0, 100) < 90) ? 1 : 0;
            }
        }
        // room 내부는 랜덤값으로 채웁니다.
        for (int x = (int)room.x; x < room.xMax; ++x)
        {
            for (int y = (int)room.y; y < room.yMax; ++y)
            {
                map[x,y] = (pseudoRandom.Next(0, 100) < density) ? 1 : 0;
            }
        }
    }

    void SmoothMapPsudo(Rect rect, int[,] map)
    {
        for (int x = (int)rect.x; x < rect.xMax; ++x)
        {
            for (int y = (int)rect.y; y < rect.yMax; ++y)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y, map);

                if (neighbourWallTiles >= 5)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 5)
                    map[x, y] = 0;
            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY, int[,] map)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX < 0 || neighbourX >= mapWidth || neighbourY < 0 || neighbourY >= mapHeight)
                {
                    ++wallCount;
                }
                else if (boundaryPosition[neighbourX,neighbourY] != null)
                {
                    wallCount += 4;
                }
                // else if (tunnelPosition[neighbourX,neighbourY] != null)
                // {
                //     wallCount -= 0;
                // }
                else if (map[neighbourX, neighbourY] == 1)
                {
                    ++wallCount;
                }
            }
        }
        return wallCount;
    }
}
