using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coin : MonoBehaviour
{
    private Text score;
    private BackGround backGround;
 
    void Start()
    {
        score = GameObject.Find("Score").GetComponent<Text>();
        backGround = GameObject.Find("backGround").GetComponent<BackGround>();
    }

    protected void OnTriggerStay2D(Collider2D col)
    {
        if (col.GetComponent<Player>())
        {
            col.GetComponent<Player>().CoinScore++;// 코인점수 증가
            score.text = "Score : " + col.GetComponent<Player>().CoinScore.ToString();
            Destroy(gameObject);

            if (col.GetComponent<Player>().CoinScore >= 5)
                backGround.nextStage();
        }
    }
}
