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
    private GameObject[,] boardPositionsNonchange;
    protected SubDungeon rootSubDungeon;
    protected ArrayList subDungeonList;
    private Transform pos;
    private string seed;

    public class SubDungeon
    {
        public SubDungeon left, right;
        public Rect rect;
        public Rect room = new Rect(-1, -1, 0, 0); // i.e null
        public int debugId;
        public List<Rect> corridors = new List<Rect>();
        protected bool savedToList = false;
        public Rect boundaryLine;

        private static int debugCounter = 0;
        public ArrayList monsters;
        public ArrayList items;
        protected RoomType roomType;

        public SubDungeon(Rect mrect)
        {
            rect = mrect;
            debugId = debugCounter;
            debugCounter++;
        }

        public bool IAmLeaf()
        {
            return left == null && right == null;
        }

        public bool Split(int minRoomSize, int maxRoomSize)
        {
            // 방이 아니면 자르지 않습니다.
            if (!IAmLeaf())
            {
                return false;
            }

            // choose a vertical or horizontal split depending on the proportions
            // i.e. if too wide split vertically, or too long horizontally,
            // or if nearly square choose vertical or horizontal at random
            bool splitH = false;
            bool splitV = false;
            float minSplitRate = 0.25f;

            // 가로로 길쭉한 맵은 세로로 자릅니다.
            if (rect.width / rect.height >= 1.05)
            {
                splitV = true;
            }
            // 세로로 길쭉한 맵은 가로로 자릅니다.
            else if (rect.height / rect.width >= 1.05)
            {
                splitH = true;
            }
            // 비슷비슷하면 랜덤으로 자릅니다
            else
            {
                splitH = Random.Range(0.0f, 1.0f) > 0.5;
                splitV = true;
            }

            if (Mathf.Min(rect.height, rect.width) / 2 < minRoomSize)
            {
                Debug.Log("Sub-dungeon " + debugId + " will be a leaf");
                return false;
            }

            // 가로로 자르려는 경우
            if (splitH)
            {
                // 잘랐을때 최소크기 이하면 자르지 않습니다.
                if (rect.height <= minRoomSize * 2)
                    return false;

                // 방은 최소크기 혹은 최소비율 이상으로 잘려야 합니다.
                minRoomSize = (int)Mathf.Max(minRoomSize, rect.height * minSplitRate);

                // 양쪽 구획이 최소크기 이상이 되도록 보정합니다.
                int split = Random.Range(minRoomSize, (int)(rect.height - minRoomSize));

                // 가로로 잘려진 두개의 구획 생성
                left = new SubDungeon(new Rect(rect.x, rect.y, rect.width, split));
                right = new SubDungeon(new Rect(rect.x, rect.y + split, rect.width, rect.height - split));
                left.boundaryLine = new Rect(rect.x, rect.y + split, rect.width, 1);
                right.boundaryLine = new Rect(rect.x, rect.y + split, rect.width, 1);
            }
            // 세로로 자르려는 경우
            else if (splitV)
            {
                // 잘랐을때 최소크기 이하면 자르지 않습니다.
                if (rect.width <= minRoomSize * 2)
                    return false;

                // 방은 최소크기 혹은 최소비율 이상으로 잘려야 합니다.
                minRoomSize = (int)Mathf.Max(minRoomSize, rect.width * minSplitRate);

                // 양쪽 구획이 최소크기 이상이 되도록 보정합니다.
                int split = Random.Range(minRoomSize, (int)(rect.width - minRoomSize));

                // 세로로 잘려진 두개의 구획 생성
                left = new SubDungeon(new Rect(rect.x, rect.y, split, rect.height));
                right = new SubDungeon(new Rect(rect.x + split, rect.y, rect.width - split, rect.height));
                left.boundaryLine = new Rect(rect.x + split, rect.y, 1, rect.height);
                right.boundaryLine = new Rect(rect.x + split, rect.y, 1, rect.height);
            }
            // 그 외의 경우가 있을 수 있을까요?
            else
            {
                Debug.LogError("Room spliting error");
            }

            return true;
        }

        public void CreateRoom()
        {
            if (left != null)
            {
                left.CreateRoom();
            }
            if (right != null)
            {
                right.CreateRoom();
            }
            if (left != null && right != null)
            {
                CreateCorridorBetween(left, right);
            }
            if (IAmLeaf())
            {
                int roomWidth = (int)Random.Range(rect.width * 0.75f, rect.width - 2);
                int roomHeight = (int)Random.Range(rect.height * 0.75f, rect.height - 2);
                int roomX = (int)Random.Range(1, rect.width - roomWidth - 1);
                int roomY = (int)Random.Range(1, rect.height - roomHeight - 1);

                // room position will be absolute in the board, not relative to the sub-dungeon
                room = new Rect(rect.x + roomX, rect.y + roomY, roomWidth, roomHeight);
                Debug.Log("Created room " + room + " in sub-dungeon " + debugId + " " + rect);
            }
        }


        public void CreateCorridorBetween(SubDungeon left, SubDungeon right)
        {
            Rect lroom = left.GetRoom();
            Rect rroom = right.GetRoom();
            //todo: make corridorWidth changeable at unity inspector
            int corridorWidth = 1;

            Debug.Log("Creating corridor(s) between " + left.debugId + "(" + lroom + ") and " + right.debugId + " (" + rroom + ")");

            // attach the corridor to a random point in each room
            Vector2 lpoint = new Vector2((int)Random.Range(lroom.x + 1, lroom.xMax - 1), (int)Random.Range(lroom.y + 1, lroom.yMax - 1));
            Vector2 rpoint = new Vector2((int)Random.Range(rroom.x + 1, rroom.xMax - 1), (int)Random.Range(rroom.y + 1, rroom.yMax - 1));

            // always be sure that left point is on the left to simplify the code
            if (lpoint.x > rpoint.x)
            {
                Vector2 temp = lpoint;
                lpoint = rpoint;
                rpoint = temp;
            }

            int w = (int)(lpoint.x - rpoint.x);
            int h = (int)(lpoint.y - rpoint.y);

            Debug.Log("lpoint: " + lpoint + ", rpoint: " + rpoint + ", w: " + w + ", h: " + h);

            // if the points are not aligned horizontally
            if (w != 0)
            {
                //boundaryLine이 가로로 있는 경우
                if (left.boundaryLine.height == 1)
                {
                    //lpoint가 rpoint 보다 낮은 경우
                    if (h < 0)
                    {
                        //draw corridors from lpoint to boundaryLine
                        corridors.Add(new Rect(lpoint.x, lpoint.y, corridorWidth, Mathf.Abs((int)(lpoint.y - left.boundaryLine.y))));
                        //draw corridors fallowing boundaryLine
                        corridors.Add(new Rect(lpoint.x, left.boundaryLine.y, Mathf.Abs(w) + 1, corridorWidth));
                        //draw corridors form boundaryLine to rpoint
                        corridors.Add(new Rect(rpoint.x, left.boundaryLine.y, corridorWidth, Mathf.Abs((int)(rpoint.y - left.boundaryLine.y))));
                    }
                    //lpoint가 rpoint 보다 높은 경우
                    else
                    {
                        //draw corridors from lpoint to boundaryLine
                        corridors.Add(new Rect(lpoint.x, left.boundaryLine.y, corridorWidth, Mathf.Abs((int)(lpoint.y - left.boundaryLine.y))));
                        //draw corridors fallowing boundaryLine
                        corridors.Add(new Rect(lpoint.x, left.boundaryLine.y, Mathf.Abs(w) + 1, corridorWidth));
                        //draw corridors form boundaryLine to rpoint
                        corridors.Add(new Rect(rpoint.x, rpoint.y, corridorWidth, Mathf.Abs((int)(rpoint.y - left.boundaryLine.y))));
                    }
                }
                //bundaryLine이 세로로 있는 경우
                else if (left.boundaryLine.width == 1)
                {
                    //lpoint 가 rpoint 보다 낮은 경우
                    if (h < 0)
                    {
                        //draw corridors from lpoint to boundaryLine
                        corridors.Add(new Rect(lpoint.x, lpoint.y, Mathf.Abs((int)(lpoint.x - left.boundaryLine.x)), corridorWidth));
                        //draw corridors fallowing boundaryLine
                        corridors.Add(new Rect(left.boundaryLine.x, lpoint.y, corridorWidth, Mathf.Abs(h)));
                        //draw corridors form boundaryLine to rpoint
                        corridors.Add(new Rect(left.boundaryLine.x, rpoint.y, Mathf.Abs((int)(left.boundaryLine.x - rpoint.x)), corridorWidth));
                    }
                    //lpoint 가 rpoint 보다 높은 경우
                    else
                    {
                        //draw corridors from lpoint to boundaryLine
                        corridors.Add(new Rect(lpoint.x, lpoint.y, Mathf.Abs((int)(lpoint.x - left.boundaryLine.x)), corridorWidth));
                        //draw corridors fallowing boundaryLine
                        corridors.Add(new Rect(left.boundaryLine.x, rpoint.y, corridorWidth, Mathf.Abs(h) + 1));
                        //draw corridors form boundaryLine to rpoint
                        corridors.Add(new Rect(left.boundaryLine.x, rpoint.y, Mathf.Abs((int)(left.boundaryLine.x - rpoint.x)), corridorWidth));
                    }
                }
                //error
                else
                {
                    Debug.LogError("Create corridor error");
                }
            }
            else
            {
                // if the points are aligned horizontally
                // go up or down depending on the positions
                if (h < 0)
                {
                    corridors.Add(new Rect((int)lpoint.x, (int)lpoint.y, corridorWidth, Mathf.Abs(h)));
                }
                else
                {
                    corridors.Add(new Rect((int)rpoint.x, (int)rpoint.y, corridorWidth, Mathf.Abs(h)));
                }
            }

            Debug.Log("Corridors: ");
            foreach (Rect corridor in corridors)
            {
                Debug.Log("corridor: " + corridor);
            }
        }

        public Rect GetRoom()
        {
            if (IAmLeaf())
            {
                return room;
            }
            if (left != null)
            {
                Rect lroom = left.GetRoom();
                if (lroom.x != -1)
                {
                    return lroom;
                }
            }
            if (right != null)
            {
                Rect rroom = right.GetRoom();
                if (rroom.x != -1)
                {
                    return rroom;
                }
            }

            // workaround non nullable structs
            return new Rect(-1, -1, 0, 0);
        }
        public SubDungeon RandomPopDungeon()
        // 던전이 빌때까지 차례로 방을 반환합니다.
        {
            // 자신이 말단 서브던전이고, 아직 pop된적이 없으면 반환합니다.
            if (IAmLeaf() && !savedToList)
            {
                savedToList = true;
                return this;
            }
            // 자신이 말단 서브던전이지만, 이미 pop되었다면 null을 반환합니다.
            else if (IAmLeaf() && savedToList)
            {
                return null;
            }
            else
            {
                // 자신이 말단이 아닌경우, left부터 차례로 연쇄호출을 시도합니다.
                // left 연쇄호출에서 말단 노드를 발견했다면 반환합니다.
                SubDungeon nextPop = left.RandomPopDungeon();
                if(nextPop != null)
                {
                    return nextPop;
                }

                // left 연쇄호출에서 말단 노드를 찾지 못했을 경우, right 연쇄호출에서 말단 노드를 발견했다면 반환합니다.
                nextPop = right.RandomPopDungeon();
                if (nextPop != null)
                {
                    return nextPop;
                }
                
                // 더이상 반환할 것이 없습니다. null를 반환합니다.
                return null;
            }
        }   
    }
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
            GameObject instance = Instantiate(obj);
            instance.transform.SetParent(transform);

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
            BoardSetup(subDungeon.room);
        }
        // 자신이 복도일 경우 재귀 호출
        else
        {
            FillRooms(subDungeon.left);
            FillRooms(subDungeon.right);
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
                    boardPositionsNonchange[i, j] = instance;
                }
            }
        }
    }

    //this fuction is for debug
    void DrawBL(SubDungeon subDungeon)
    {
        if (subDungeon == null)
        {
            return;
        }

        DrawBL(subDungeon.left);
        DrawBL(subDungeon.right);
        for (int i = (int)subDungeon.boundaryLine.x; i < subDungeon.boundaryLine.xMax; i++)
        {
            for (int j = (int)subDungeon.boundaryLine.y; j < subDungeon.boundaryLine.yMax; j++)
            {
                GameObject instance = Instantiate(floor, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(transform);
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
                if (boardPositionsFloor[i, j] == null && boardPositionsNonchange[i, j] == null)
                {
                    GameObject instance = Instantiate(boundary, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(transform);
                    boardPositionsNonchange[i, j] = instance;
                }
            }
        }
    }

    public void DrawMap()
    {
        rootSubDungeon = new SubDungeon(new Rect(0, 0, mapHeight, mapWidth));
        CreateBSP(rootSubDungeon);
        rootSubDungeon.CreateRoom();

        boardPositionsFloor = new GameObject[mapHeight, mapWidth];
        boardPositionsNonchange = new GameObject[mapHeight, mapWidth];
        DrawCorridors(rootSubDungeon);
        //DrawBL(rootSubDungeon);
        DrawRooms(rootSubDungeon);
        DrawBoundarys(rootSubDungeon);
        FillRooms(rootSubDungeon);
    }
    public void StoreMapsIntosubDungeonList()
    {
        subDungeonList = new ArrayList();
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
        DrawMap();
        StoreMapsIntosubDungeonList();
    }


    //------------------------------------------------------------------------------------------------------------------------------

    void BoardSetup(Rect rect)
    {
        map = new int[mapHeight, mapWidth];
        ArrayList listX = new ArrayList();
        ArrayList listY = new ArrayList();

        // RandomFillMap(rect);
        // for (int i = 0; i < smoothness; ++i)
        // {
        //     SmoothMap(rect);
        // }
        // for (int i = 0; i < postsmooth; ++i)
        // {
        //     SmoothMapPsudo(rect);
        // }

        for (int y = (int)rect.y; y < rect.yMax; ++y)
        {
            for (int x = (int)rect.x; x < rect.xMax; ++x)
            {
                if (map[x, y] == 1)
                {
                    if (boardPositionsNonchange[x, y] == null)
                    {
                        GameObject toInstantiate = wall;
                        toInstantiate.layer = LayerMask.NameToLayer("Wall");
                        GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                        //instance.transform.SetParent(boardHolder);
                        instance.transform.SetParent(transform);
                        boardPositionsFloor[x, y] = instance;
                    }
                }
                else if (map[x, y] == 0) // 맵이 0이고
                {
                    if (NoWallSurround(x, y, rect)) // 주변에 겹칠만한게 없을때.
                    {
                        listX.Add(x);
                        listY.Add(y);
                    }
                }
            }
        }
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
                if (x == rect.x || x == rect.xMax - 1 || y == rect.y || y == rect.yMax - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
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
                int neighbourWallTiles = GetSurroundingWallCount(x, y, rect);

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
                int neighbourWallTiles = GetSurroundingWallCount(x, y, rect);

                if (neighbourWallTiles > 4)
                    nextMap[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    nextMap[x, y] = 0;
            }
        }
        map = nextMap;
    }

    int GetSurroundingWallCount(int gridX, int gridY, Rect rect)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= rect.x && neighbourX < rect.xMax && neighbourY >= rect.y && neighbourY < rect.yMax)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
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
