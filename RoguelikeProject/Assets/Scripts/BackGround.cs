using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackGround : MonoBehaviour
{
    public static BackGround instance;
    private SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    private int stageNum;

    void Awake() // 중복생성 안되게하고 계속 정보 갖고있게
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void nextStage()
    {
        if (stageNum > 2)
            stageNum = 0;
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
        spriteRenderer.sprite = sprites[++stageNum];
    }
}
