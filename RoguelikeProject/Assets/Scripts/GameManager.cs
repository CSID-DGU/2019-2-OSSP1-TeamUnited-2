using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private Text levelText;
    private GameObject levelImage;

    public int width;
    public int height;
    public GameObject boundary;
    public GameObject floor;
    public GameObject wall;

    public GameObject SpawnedPlayer;
    public GameObject SpawnedEnemy;
    public GameObject SpawnedRandEnemy;
    public GameObject SpawnedItem;

    public double density;
    public int smoothness;
    private int[,] map;
    private Transform boardHolder;

    private string seed;

    void Start()
    {
        InitGame();
        AstarPath.active.Scan(); // 이거를 해야 벽하고 부딪히네요
    }
	void InitGame() {
		SetupScene();

        levelImage = GameObject.Find("LevelImage");

        levelText = GameObject.Find("Text").GetComponent<Text>();

        levelImage.SetActive(false);

        SpawnedPlayer.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        if (PlayerControl.knowHP < 0)
        {
            SpawnedPlayer.SetActive(false);
            Invoke("nextScene", 1);
        }
        else
        {
            levelText.text = null;
            levelImage.SetActive(false);
        }
    }
    void nextScene()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
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
        RandomFillMap();
        for (int i = 0; i < smoothness; ++i)
        {
            SmoothMap();
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
            }
        }

        while (true)
        {
            int playerX = Random.Range(1, width - 1);
            int playerY = Random.Range(1, height - 1);
            int enemyX = Random.Range(1, width - 1);
            int enemyY = Random.Range(1, height - 1);
            int enemyRandX = Random.Range(1, width - 1);
            int enemyRandY = Random.Range(1, height - 1);
            int item1X = Random.Range(1, width - 1);
            int item1Y = Random.Range(1, height - 1);
            int item2X = Random.Range(1, width - 1);
            int item2Y = Random.Range(1, height - 1);
            int item3X = Random.Range(1, width - 1);
            int item3Y = Random.Range(1, height - 1);

            if (map[playerX, playerY] == 0 && map[enemyX, enemyY] == 0 && map[enemyRandX, enemyRandY] == 0 && map[item1X, item1Y] == 0 && map[item2X, item2Y] == 0 && map[item3X, item3Y] == 0)
            {
                SpawnedPlayer.transform.position = new Vector3(playerX, playerY, -10);
                SpawnedEnemy.transform.position = new Vector3(enemyX, enemyY, -10);
                SpawnedRandEnemy.transform.position = new Vector3(enemyRandX, enemyRandY, -10);
                GameObject item1 = Instantiate(SpawnedItem, new Vector3(item1X, item1Y, -10), Quaternion.identity) as GameObject;
                GameObject item2 = Instantiate(SpawnedItem, new Vector3(item2X, item2Y, -10), Quaternion.identity) as GameObject;
                GameObject item3 = Instantiate(SpawnedItem, new Vector3(item3X, item3Y, -10), Quaternion.identity) as GameObject;
                break;
            }
        }

    }
    public void SetupScene()
    {
        BoardSetup();
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
    void SmoothMap()
    {
        // int[,] nextMap = new int[width, height];
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
        // map = nextMap;
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
}
