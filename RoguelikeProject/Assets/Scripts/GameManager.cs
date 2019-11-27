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
    [HideInInspector]
    public int[,] map;
    public int width;
    public int height;
    // public GameObject boundary;
    // public GameObject floor;
    // public GameObject wall;
    public GameObject plane;

    private Texture2D tex;
    public GameObject playerEntity;
    public GameObject miniGold; // 미니맵에 보여줄 코인
    private int enemyNum; // 적들의 수.

    public int EnemyNum
    {
        get { return enemyNum; }
        set { enemyNum = value; }
    }
    private Transform boardHolder;


    public int boardRows, boardColumns;
    public int minRoomSize, maxRoomSize;
    public GameObject corridorTile;
    private GameObject[,] boardPositionsFloor;
    private GameObject[,] boardPositionsNonchange;
    private Transform pos;


    void Start()
    {        
        InitializeGame();
    }
    
    private void InitializeGame()
    {
        // 맵은 게임매니저에서 관리합니다. 일단 최초에 한번 복사하는 방식으로 합니다만, 추후 문제가 발생할 수 있습니다.
        // 참조로 받아오는 편이 나을 수도 있습니다.
        map = mapManager.GetComponent<MapManager>().map;
        width = mapManager.GetComponent<MapManager>().mapWidth;
        height = mapManager.GetComponent<MapManager>().mapHeight;

        // 시야 처리 관련
        tex = new Texture2D(width, height);
        plane.GetComponent<Renderer>().material.mainTexture = tex;
        plane.GetComponent<Renderer>().material.mainTexture.filterMode = FilterMode.Point;
        RenderToString();
    }

    private void RenderToString()
    {
        // map = new int[width,height];
        bool[,] lit = new bool[width, height];
        int radius = 15;
        int playerX = (int)playerEntity.transform.position.x;
        int playerY = (int)playerEntity.transform.position.y;
        // Debug.Log(playerX + " "  + playerY);
        // ShadowCaster.ComputeFieldOfViewWithShadowCasting(playerX, playerY, radius, (x1, y1) => map[x1, y1] == 1, (x2, y2) => { lit[x2, y2] = true; });

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
                    tex.SetPixel(x, y, new Color(0f, 0f, 0f, 0.1f));
                }
            }
        }
        tex.Apply(false);
    }

    void Update()
    {
        // if (map[(int)playerEntity.transform.position.x, (int)playerEntity.transform.position.y] != 1)
        {
            // ??? 시야처리 관련?
            RenderToString();
        }
    }
}