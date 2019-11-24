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
    public GameObject mapManager;
    protected int[,] map;
    public int width;
    public int height;
    public GameObject boundary;
    public GameObject floor;
    public GameObject wall;
    public GameObject plane;

    private Texture2D tex;

    public GameObject Player;
    public GameObject miniGold; // 미니맵에 보여줄 코인
    private int enemyNum; // 적들의 수.

    public int EnemyNum
    {
        get { return enemyNum; }
        set { enemyNum = value; }
    }
    private Transform boardHolder;


    void Start()
    {        
        InitializeGame();
    }
    // void BoardSetup()
    // {
    //     Debug.Log("Board setup start");
    //     boardHolder = new GameObject("Board").transform;
    //     Debug.Log("BoardHolder created");

    //     for (int x = -1; x < width + 1; ++x)
    //     {
    //         for (int y = -1; y < height + 1; ++y)
    //         {
    //             GameObject toInstantiate = floor;
    //             if (x == -1 || x == width || y == -1 || y == height)
    //             {
    //                 toInstantiate = boundary;
    //             }
    //             GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
    //             instance.transform.SetParent(boardHolder);
    //         }
    //     }
    //     Debug.Log("instantiated");

    //     map = new int[width, height];
    //     ArrayList listX = new ArrayList();
    //     ArrayList listY = new ArrayList();

    //     GameObject ItemsParent = new GameObject("Items"); // 아이템들의 부모 설정.
    //     ItemsParent.layer = LayerMask.NameToLayer("Wall"); // 일단 Wall 로 합니다. 추후 변경가능성.
    //     GameObject[] healtem = new GameObject[10];
    //     if (listX.Count > 10)
    //     {
    //         for (int i = 0; i < 10; i++)
    //         {
    //             index = Random.Range(0, listX.Count);
    //             healtem[i] = Instantiate(SpawnedItem1, new Vector3((int)listX[index], (int)listY[index], -10), Quaternion.identity) as GameObject;
    //             healtem[i].transform.SetParent(ItemsParent.transform);
    //             listX.RemoveAt(index);
    //             listY.RemoveAt(index);
    //         }
    //     }
    //     else Debug.Log("하트 만들 공간 없음..");
    //     if (listX.Count > 10)
    //     {
    //         GameObject[] coin = new GameObject[5];
    //         GameObject[] miniCoin = new GameObject[5];
    //         GameObject[] bomtem = new GameObject[5];
    //         for (int i = 0; i < 5; i++)
    //         {
    //             index = Random.Range(0, listX.Count);
    //             coin[i] = Instantiate(SpawnedItem2, new Vector3((int)listX[index], (int)listY[index], -10), Quaternion.identity) as GameObject;
    //             miniCoin[i] = Instantiate(miniGold, coin[i].transform) as GameObject;
    //             coin[i].transform.SetParent(ItemsParent.transform);
    //             listX.RemoveAt(index);
    //             listY.RemoveAt(index);

    //             index = Random.Range(0, listX.Count);
    //             bomtem[i] = Instantiate(SpawnedItem3, new Vector3((int)listX[index], (int)listY[index], -10), Quaternion.identity) as GameObject;
    //             bomtem[i].transform.SetParent(ItemsParent.transform);
    //             listX.RemoveAt(index);
    //             listY.RemoveAt(index);
    //         }
    //     }
    //     else Debug.Log("아이템 만들 공간 없음..");
    // }
    
    private void InitializeGame()
    {
        // 맵은 게임매니저에서 관리합니다. 일단 최초에 한번 복사하는 방식으로 합니다만, 추후 문제가 발생할 수 있습니다.
        // 참조로 받아오는 편이 나을 수도 있습니다.
        map = mapManager.GetComponent<MapManager>().Map;
        width = mapManager.GetComponent<MapManager>().mapWidth;
        height = mapManager.GetComponent<MapManager>().mapHeight;

        map = new int[100, 100];

        foreach (var i in map)
        {
            Debug.LogError(map[0,0]);
        }
        tex = new Texture2D(width, height);
        Debug.Log("TEX GENERATE");
        plane.GetComponent<Renderer>().material.mainTexture = tex;
        plane.GetComponent<Renderer>().material.mainTexture.filterMode = FilterMode.Point;
        RenderToString();
    }

    private void RenderToString()
    {
        map = new int[width,height];
        bool[,] lit = new bool[width, height];
        int radius = 15;
        int playerX = (int)Player.transform.position.x;
        int playerY = (int)Player.transform.position.y;
        ShadowCaster.ComputeFieldOfViewWithShadowCasting(playerX, playerY, radius, (x1, y1) => map[x1, y1] == 1, (x2, y2) => { lit[x2, y2] = true; });

        Color colorFloor = new Color(0f, 0f, 0f, 0f);

        for (int y = height - 1; y >= 0; --y)
        {
            for (int x = 0; x < width; ++x)
            {
                if (lit[x, y])
                {
                    if (map[x, y] == 1)
                    {
                        tex.SetPixel(x, y, colorFloor);
                    }
                    else
                    {
                        tex.SetPixel(x, y, colorFloor);
                    }
                }
                else
                {
                    tex.SetPixel(x, y, new Color(0f, 0f, 0f, 0.5f));
                }
            }
        }
        tex.Apply(false);
    }

    void Update()
    {
        // if (map[(int)Player.transform.position.x, (int)Player.transform.position.y] != 1)
        {
            // ??? 시야처리 관련?
            RenderToString();
        }
    }
}