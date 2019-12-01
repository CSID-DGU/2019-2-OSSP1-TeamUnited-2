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
    public GameObject boss;
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

    private string seed;
    // private Transform boardHolder;
    // public int boardRows, boardColumns;
    // public int minRoomSize, maxRoomSize;
    // public GameObject corridorTile;
    // private GameObject[,] boardPositionsFloor;
    // private GameObject[,] boardPositionsNonchange;
    // private Transform pos;



    void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        // 맵은 게임매니저에서 관리합니다. 일단 최초에 한번 복사하는 방식으로 합니다만, 추후 문제가 발생할 수 있습니다.
        // 참조로 받아오는 편이 나을 수도 있습니다.
        // map = mapManager.GetComponent<MapManager>().map;
        width = mapManager.GetComponent<MapManager>().mapWidth;
        height = mapManager.GetComponent<MapManager>().mapHeight;

        // 시야 처리 관련
        tex = new Texture2D(width, height);
        plane.GetComponent<Renderer>().material.mainTexture = tex;
        plane.GetComponent<Renderer>().material.mainTexture.filterMode = FilterMode.Point;


        seed = System.DateTime.Now.ToString();
        System.Random pseudoRandom = new System.Random(seed.GetHashCode());
        GameObject instance = Instantiate(boss, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
        instance.transform.GetChild(0).Translate(new Vector3(pseudoRandom.Next(20, 80), pseudoRandom.Next(0, 100), 0f));
    }
    void Update()
    {
        bool[,] lit = new bool[width, height]; // 크기
        float radius = 10.0f; // 반지름
        int layerMask = 1 << 10; // 적은 안보게 함.
        layerMask = ~layerMask; // 반전시켜서 이것만 걸러내는거
        Collider2D[] mcols = Physics2D.OverlapCircleAll(playerEntity.transform.position, radius, layerMask); // 원안에 조사
        List<Vector2> mcolsVector = new List<Vector2>();
        foreach (Collider2D co in mcols) // 좌표만 가져옴(int로 캐스팅해서)
        {
            mcolsVector.Add(new Vector2((int)co.transform.position.x, (int)co.transform.position.y));
        }
        foreach (Vector2 mVec in mcolsVector) // 좌표를 이용해서 시야처리
        {
            Vector2 rayDirection = mVec - (Vector2)playerEntity.transform.position; // 방향
            rayDirection.Normalize();
            float distance = Vector2.Distance(playerEntity.transform.position, mVec); // 거리
            RaycastHit2D[] cols = Physics2D.RaycastAll(playerEntity.transform.position, rayDirection, distance, layerMask); // 직선상에 있는것
            int repeat = 0; bool setRepeat = false; // 벽을 어느정도 보여주기 위함.
            foreach (RaycastHit2D co in cols)
            {
                Collider2D[] colliderCount = Physics2D.OverlapPointAll(co.transform.position); // collider를 타일에 달았기 때문에 Wall 이 있는곳에 2개가 나옴.

                if (setRepeat) // 한번 벽이 나오면 true 가 되어서 repeat을 증가시킴.
                {
                    repeat++;
                    if (repeat > 2 || colliderCount.Length < 2) // 벽이 2줄이거나 타일이 나와버리면 그만둔다.
                        break;
                }

                if (co.collider.name == "Wall(Clone)" || co.collider.name == "Boundary(Clone)")
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
                    tex.SetPixel(x, y, new Color(0f, 0f, 0f, 0.25f));
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