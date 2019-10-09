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

    // Update is called once per frame
    void Update()
    {
        hpUI.sprite = hpSprite[PlayerControl.knowHP + 1];
    }
}
