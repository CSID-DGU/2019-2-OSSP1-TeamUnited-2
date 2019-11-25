using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public double density;
    public int smoothness;
    public int postsmooth;
    public int[,] map;
    private Transform boardHolder;
    public GameObject boundary;
    public GameObject floor;
    public GameObject wall;
    public int mapHeight, mapWidth;
    public int minRoomSize, maxRoomSize;
    public GameObject corridorTile;
    private GameObject[,] boardPositionsFloor;
    private GameObject[,] corridorPosition;
    private GameObject[,] wallPosition;
    private GameObject[,] boundaryPosition;
    protected SubDungeon rootSubDungeon;
    protected List<SubDungeon> subDungeonList;
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
    public void DrawRooms(SubDungeon subDungeon)
    {
        if (subDungeon == null)
        {
            return;
        }
        if (subDungeon.IAmLeaf())
        {
            for (int i = (int)subDungeon.room.x; i < subDungeon.room.xMax; i++)
            {
                for (int j = (int)subDungeon.room.y; j < subDungeon.room.yMax; j++)
                {
                    GameObject instance = Instantiate(floor, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(transform);
                    boardPositionsFloor[i, j] = instance;
                }
            }
        }
        else
        {
            DrawRooms(subDungeon.left);
            DrawRooms(subDungeon.right);
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
    public void RandomThrowObjectInRoom(SubDungeon dungeon, GameObject obj)
    // 방 내부에 랜덤하게 아이템, 적을 뿌립니다. 일단은 랜덤 버전만...
    {
        bool deployed = false;
        int trial = 0;
        while (!deployed)
        {
            Vector2 toDeployOn;
            toDeployOn.x = dungeon.room.x + Random.Range(0.0f, dungeon.room.width);
            toDeployOn.y = dungeon.room.y + Random.Range(0.0f, dungeon.room.height);
            if (true)
            {
                GameObject instance = Instantiate(obj);
                instance.transform.SetParent(transform);
                deployed = true;
            }

            ++trial;
            if (trial > 100)
            {
                Debug.LogError("There is some trouble in deploying object...");
                break;
            }
        }
    }

    public void FillRooms(SubDungeon subDungeon)
    {
        // TODO :: 서브던전이 없는 상황은 언제 일어나게 되는거죠? 아무튼 예외처리
        if (subDungeon == null)
        {
            return;
        }
        // 자신이 실제 서브던전 일 경우
        if (subDungeon.IAmLeaf())
        {
            FillRoom(subDungeon.room);
        }
        // 자신이 복도일 경우 재귀 호출
        else
        {
            FillRooms(subDungeon.left);
            FillRooms(subDungeon.right);
        }
    }

    public void FillRoom(Rect room)
    {
        for (int y = (int)room.y; y < room.yMax; ++y)
        {
            for (int x = (int)room.x; x < room.xMax; ++x)
            {
                if (corridorPosition[x, y] == null)
                {
                    GameObject toInstantiate = wall;
                    toInstantiate.layer = LayerMask.NameToLayer("Wall");
                    GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(transform);
                    wallPosition[x, y] = instance;
                }
            }
        }
    } 

    void DrawCorridors(SubDungeon subDungeon)
    {
        if (subDungeon == null)
        {
            return;
        }

        DrawCorridors(subDungeon.left);
        DrawCorridors(subDungeon.right);

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

    void DrawBoundarys(SubDungeon subDungeon)
    {
        if (subDungeon == null)
        {
            return;
        }

        for (int i = (int)subDungeon.rect.x; i < subDungeon.rect.xMax; i++)
        {
            for (int j = (int)subDungeon.rect.y; j < subDungeon.rect.yMax; j++)
            {
                if (boardPositionsFloor[i, j] == null && corridorPosition[i, j] == null)
                {
                    GameObject instance = Instantiate(boundary, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(transform);
                    boundaryPosition[i, j] = instance;
                }
            }
        }
    }

    public void DrawMap()
    {

    }
    public void StoreMapsIntosubDungeonList()
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

    private void Start()
    {
        // 각종 맵 오브젝트를 정수좌표계에 연동해서 넣을 배열을 초기화합니다.
        boardPositionsFloor         = new GameObject[mapHeight, mapWidth];
        corridorPosition            = new GameObject[mapHeight, mapWidth];
        wallPosition                = new GameObject[mapHeight, mapWidth];
        boundaryPosition            = new GameObject[mapHeight, mapWidth];

        // 루트 서브던전은 맵 전체크기로 생성합니다.        
        rootSubDungeon = new SubDungeon(new Rect(0, 0, mapHeight, mapWidth));

        // 전체 맵을 재귀호출하여 적당한 구획(변수명 rect)으로 나눠줍니다.
        CreateBSP(rootSubDungeon);

        // 재귀호출을 통해 서브던전의 변수 room과 corridor를 정의해줍니다.
        rootSubDungeon.CreateRoomRecursive();

        // 
        DrawCorridors(rootSubDungeon);
        DrawRooms(rootSubDungeon);
        DrawBoundarys(rootSubDungeon);
        FillRooms(rootSubDungeon);
        StoreMapsIntosubDungeonList();
    }


    //------------------------------------------------------------------------------------------------------------------------------

    void BoardSetup(Rect rect)
    {
        // map = new int[mapHeight, mapWidth];
        // ArrayList listX = new ArrayList();
        // ArrayList listY = new ArrayList();

        // RandomFillMap(rect);
        // for (int i = 0; i < smoothness; ++i)
        // {
        //     SmoothMap(rect);
        // }
        // for (int i = 0; i < postsmooth; ++i)
        // {
        //     SmoothMapPsudo(rect);
        // }


    }

    void RandomFillMap(Rect rect)
    {
        seed = System.DateTime.Now.ToString();
        System.Random pseudoRandom = new System.Random(seed.GetHashCode());
        // Debug.Log(seed);
        double randomFillPercent = density;

        for (int x = (int)rect.x; x < rect.xMax; ++x)
        {
            for (int y = (int)rect.y; y < rect.yMax; ++y)
            {
                GameObject toInstantiate = null;
                if (x == rect.x || x == rect.xMax - 1 || y == rect.y || y == rect.yMax - 1)
                {
                    toInstantiate = wall;
                }
                else
                {
                    toInstantiate = (pseudoRandom.Next(0, 100) < randomFillPercent) ? wall : null;
                }
            }
        }
    }
    void SmoothMapPsudo(Rect rect)
    {
        for (int x = (int)rect.x; x < rect.xMax; ++x)
        {
            for (int y = (int)rect.y; y < rect.yMax; ++y)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    map[x, y] = 0;
            }
        }
    }



    void SmoothMap(Rect rect)
    {
        int[,] nextMap = new int[(int)rect.xMax, (int)rect.yMax];
        for (int x = (int)rect.x; x < rect.xMax; ++x)
        {
            for (int y = (int)rect.y; y < rect.yMax; ++y)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 4)
                    nextMap[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    nextMap[x, y] = 0;
            }
        }
        map = nextMap;
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                // 우선 wall과 boundary만 고려합니다
                if (wallPosition[neighbourX, neighbourY] != null || boundaryPosition[neighbourX, neighbourY] != null)
                {
                    ++wallCount;
                }
            }
        }
        return wallCount;
    }

    bool NoWallSurround(int x, int y, Rect rect) // 목적 : 위치 지정할때 범위에 겹치는것이 없도록 한다. 너무 범위 값이 크면 들어갈 자리가 없어진다.
    {
        for (int i = x - 5; i <= x + 5; i++)
        {
            for (int j = y - 5; j <= y + 5; j++)
            {
                if (i > rect.x && i < rect.xMax && j > rect.y && j < rect.yMax)
                {
                    if (map[i, j] == 1) // 벽있으면 ㅂ2
                        return false;
                    else
                        map[i, j] = 2; // 조건들을 다 통과할시 이곳에또 안겹치게 설정.
                }
                else // 조건이 별로면 ㅂ2
                    return false;
            }
        }
        return true; // 다 지나왔다면 패스
    }
}
