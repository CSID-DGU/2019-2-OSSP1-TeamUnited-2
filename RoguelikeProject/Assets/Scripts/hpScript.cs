using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hpScript : MonoBehaviour
{
    public Sprite[] hpSprite;
    public Image hpUI;
    void Start()
    {
    }

    void Update()
    {
        if (GameObject.Find("Player") != null) //살아있으면 getHP로 가져와서 보여줌.
            hpUI.sprite = hpSprite[GameObject.Find("Player").GetComponent<Player>().GetHP()];
        else
            hpUI.sprite = hpSprite[0]; // 0으로
    }
}