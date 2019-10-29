﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coin : MonoBehaviour
{
    private Text score;
    private GameObject backGround;
 
    void Start()
    {
        score = GameObject.Find("Score").GetComponent<Text>();
        backGround = GameObject.Find("backGround");
    }

    protected void OnTriggerStay2D(Collider2D col)
    {
        if (col.GetComponent<Player>())
        {
            col.GetComponent<Player>().CoinScore++;// 코인점수 증가
            score.text = "Score : " + col.GetComponent<Player>().CoinScore.ToString();
            Destroy(gameObject);
            backGround.GetComponent<BackGround>().nextStage();
        }
    }
}
