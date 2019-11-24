using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;
using System.Linq;
using System.Text;

public class GameManager : MonoBehaviour
{
    private GameObject levelImage;

    public int width;
    public int height;
    public GameObject boundary;
    public GameObject floor;
    public GameObject wall;
    public GameObject plane;

    private Texture2D tex;
    private int playerX;
    private int playerY;

    public GameObject SpawnedPlayer;
    public GameObject[] SpawnedEnemy;
    public GameObject[] SpawnedRandEnemy;
    public GameObject SpawnedItem1;
    public GameObject SpawnedItem2;
    public GameObject SpawnedItem3;
    public GameObject miniGold; // 미니맵에 보여줄 코인
    private int enemyNum; // 적들의 수.

    public int EnemyNum
    {
        get { return enemyNum; }
        set { enemyNum = value; }
    }

    public double density;
    public int smoothness;
    public int postsmooth;
    private int[,] map;
    private Transform boardHolder;

    private string seed;

    void Start()
    {
        BoardSetup();

        SpawnedPlayer.SetActive(true);

        enemyNum = SpawnedEnemy.Length + SpawnedRandEnemy.Length; // 길이 설정
        InitializeGame();
    }
    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x < width + 1; ++x)
        {
            for (int y = -1; y < height + 1; ++y)
            {
                GameObject toInstantiate = floor;
                if (x == -1 || x == width || y == -1 || y == height)
                {
                    toInstantiate = boundary;
                }
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }
        }

        map = new int[width, height];
        ArrayList listX = new ArrayList();
        ArrayList listY = new ArrayList();

        RandomFillMap();
        for (int i = 0; i < smoothness; ++i)
        {
            SmoothMap();
        }
        for (int i = 0; i < postsmooth; ++i)
        {
            SmoothMapPsudo();
        }

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                if (map[x, y] == 1)
                {
                    GameObject toInstantiate = wall;
                    toInstantiate.layer = LayerMask.NameToLayer("Wall");
                    GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(boardHolder);
                }
                else if (map[x, y] == 0) // 맵이 0이고
                {
                    if (NoWallSurround(x, y)) // 주변에 겹칠만한게 없을때.
                    {
                        listX.Add(x);
                        listY.Add(y);
                    }
                }
            }
        }

        // 플레이어, 적, 아이템 위치 결정.
        int index = Random.Range(0, listX.Count);
        SpawnedPlayer.transform.position = new Vector3((int)listX[index], (int)listY[index], -10);
        listX.RemoveAt(index);
        listY.RemoveAt(index);

        for (int i = 0; i < SpawnedEnemy.Length; i++)
        {
            index = Random.Range(0, listX.Count);
            SpawnedEnemy[i].transform.position = new Vector3((int)listX[index], (int)listY[index], -10);
            listX.RemoveAt(index);
            listY.RemoveAt(index);

            index = Random.Range(0, listX.Count);
            SpawnedRandEnemy[i].transform.position = new Vector3((int)listX[index], (int)listY[index], -10);
            listX.RemoveAt(index);
            listY.RemoveAt(index);
        }

        GameObject ItemsParent = new GameObject("Items"); // 아이템들의 부모 설정.
        ItemsParent.layer = LayerMask.NameToLayer("Wall"); // 일단 Wall 로 합니다. 추후 변경가능성.
        GameObject[] healtem = new GameObject[10];
        if (listX.Count > 10)
        {
            for (int i = 0; i < 10; i++)
            {
                index = Random.Range(0, listX.Count);
                healtem[i] = Instantiate(SpawnedItem1, new Vector3((int)listX[index], (int)listY[index], -10), Quaternion.identity) as GameObject;
                healtem[i].transform.SetParent(ItemsParent.transform);
                listX.RemoveAt(index);
                listY.RemoveAt(index);
            }
        }
        else Debug.Log("하트 만들 공간 없음..");
        if (listX.Count > 10)
        {
            GameObject[] coin = new GameObject[5];
            GameObject[] miniCoin = new GameObject[5];
            GameObject[] bomtem = new GameObject[5];
            for (int i = 0; i < 5; i++)
            {
                index = Random.Range(0, listX.Count);
                coin[i] = Instantiate(SpawnedItem2, new Vector3((int)listX[index], (int)listY[index], -10), Quaternion.identity) as GameObject;
                miniCoin[i] = Instantiate(miniGold, coin[i].transform) as GameObject;
                coin[i].transform.SetParent(ItemsParent.transform);
                listX.RemoveAt(index);
                listY.RemoveAt(index);

                index = Random.Range(0, listX.Count);
                bomtem[i] = Instantiate(SpawnedItem3, new Vector3((int)listX[index], (int)listY[index], -10), Quaternion.identity) as GameObject;
                bomtem[i].transform.SetParent(ItemsParent.transform);
                listX.RemoveAt(index);
                listY.RemoveAt(index);
            }
        }
        else Debug.Log("아이템 만들 공간 없음..");
    }

    void RandomFillMap()
    {
        seed = System.DateTime.Now.ToString();
        System.Random pseudoRandom = new System.Random(seed.GetHashCode());
        // Debug.Log(seed);
        double randomFillPercent = density;

        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
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
    void SmoothMapPsudo()
    {
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    map[x, y] = 0;
            }
        }
    }

    void SmoothMap()
    {
        int[,] nextMap = new int[width, height];
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
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
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
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



    bool NoWallSurround(int x, int y) // 목적 : 위치 지정할때 범위에 겹치는것이 없도록 한다. 너무 범위 값이 크면 들어갈 자리가 없어진다.
    {
        for (int i = x - 3; i <= x + 3; i++)
        {
            for (int j = y - 3; j <= y + 3; j++)
            {
                if (i > 0 && i < width && j > 0 && j < height)
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

    private void InitializeGame()
    {
        tex = new Texture2D(width, height);
        plane.GetComponent<Renderer>().material.mainTexture = tex;
        plane.GetComponent<Renderer>().material.mainTexture.filterMode = FilterMode.Point;
    }

    void Update()
    {
        bool[,] lit = new bool[width, height]; // 크기
        float radius = 10.0f; // 반지름
        int layerMask = 1 << 10; // 적은 안보게 함.
        layerMask = ~layerMask; // 반전시켜서 이것만 걸러내는거
        Collider2D[] mcols = Physics2D.OverlapCircleAll(SpawnedPlayer.transform.position, radius, layerMask); // 원안에 조사
        List<Vector2> mcolsVector = new List<Vector2>();
        foreach (Collider2D co in mcols) // 좌표만 가져옴(int로 캐스팅해서)
        {
            mcolsVector.Add(new Vector2((int)co.transform.position.x, (int)co.transform.position.y));
        }

        foreach (Vector2 mVec in mcolsVector) // 좌표를 이용해서 시야처리
        {
            Vector2 rayDirection = mVec - (Vector2)SpawnedPlayer.transform.position; // 방향
            rayDirection.Normalize();
            float distance = Vector2.Distance(SpawnedPlayer.transform.position, mVec); // 거리
            RaycastHit2D[] cols = Physics2D.RaycastAll(SpawnedPlayer.transform.position, rayDirection, distance, layerMask); // 직선상에 있는것
            int repeat = 0; bool setRepeat = false; // 벽을 어느정도 보여주기 위함.
            foreach (RaycastHit2D co in cols)
            {
                Collider2D[] colliderCount = Physics2D.OverlapPointAll(co.transform.position); // collider를 타일에 달았기 때문에 Wall 이 있는곳에 2개가 나옴.
                if (setRepeat) // 한번 벽이 나오면 true 가 되어서 repeat을 증가시킴.
                {
                    repeat++;
                    if (repeat > 2 || colliderCount.Length <2) // 벽이 2줄이거나 타일이 나와버리면 그만둔다.
                        break;
                }

                if (co.collider.name == "Wall(Clone)")
                {
                    setRepeat = true;
                }

                if (indexSafe((int)co.transform.position.x, (int)co.transform.position.y)) // 배열에 넣음. point를 쓰지말고 transform position 이 딱 맞는다.
                    lit[(int)co.transform.position.x, (int)co.transform.position.y] = true;
            }
        }
        Color colorFloor = new Color(0f, 0f, 0f, 0f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (lit[x, y])
                    tex.SetPixel(x, y, colorFloor);
                else
                    tex.SetPixel(x, y, new Color(0f, 0f, 0f, 0.5f));
            }
        }
        tex.Apply(false);
    }
    bool indexSafe(int x, int y)
    {
        if (x < 0 || x > width - 1 || y < 0 || y > height - 1)
            return false;
        else return true;
    }
}