using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class GameManager : MonoBehaviour
{
    private GameObject levelImage;

    public int width;
    public int height;
    public GameObject boundary;
    public GameObject floor;
    public GameObject wall;

    public GameObject SpawnedPlayer;
    public GameObject[] SpawnedEnemy;
    public GameObject[] SpawnedRandEnemy;
    public GameObject SpawnedItem1;
    public GameObject SpawnedItem2;
    public GameObject SpawnedItem3;

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

        AstarPath.active.Scan(); // 이거를 해야 벽하고 부딪히네요
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

        for (int i = 0; i < 4; i++)
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

        Transform ItemsParent = new GameObject("Items").transform; // 아이템들의 부모 설정.

        GameObject[] healtem = new GameObject[10];
        for (int i = 0; i < 10; i++)
        {
            index = Random.Range(0, listX.Count);
            healtem[i] = Instantiate(SpawnedItem1, new Vector3((int)listX[index], (int)listY[index], -10), Quaternion.identity) as GameObject;
            healtem[i].transform.SetParent(ItemsParent);
            listX.RemoveAt(index);
            listY.RemoveAt(index);
        }

        GameObject[] tutem = new GameObject[5];
        GameObject[] bomtem = new GameObject[5];
        for (int i = 0; i < 5; i++)
        {
            index = Random.Range(0, listX.Count);
            tutem[i] = Instantiate(SpawnedItem2, new Vector3((int)listX[index], (int)listY[index], -10), Quaternion.identity) as GameObject;
            tutem[i].transform.SetParent(ItemsParent);
            listX.RemoveAt(index);
            listY.RemoveAt(index);

            index = Random.Range(0, listX.Count);
            bomtem[i] = Instantiate(SpawnedItem3, new Vector3((int)listX[index], (int)listY[index], -10), Quaternion.identity) as GameObject;
            bomtem[i].transform.SetParent(ItemsParent);
            listX.RemoveAt(index);
            listY.RemoveAt(index);
        }
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
        for (int i = x - 5; i <= x + 5; i++)
        {
            for (int j = y - 5; j <= y + 5; j++)
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
}