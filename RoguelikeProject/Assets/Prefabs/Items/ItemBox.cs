using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : Unit
{
    private enum SCALEDIRECTION
    {
        UP = 0,
        DOWN = 1,
    }
    private SCALEDIRECTION ScaleDirection;
    private float ScaleXY = 1.0f;

    private float StartingScale = 0.0f;
    public GameObject bombItem;
    protected override void Start()
    {
        currentHP = HP;
        StartingScale = transform.localScale.x;
    }
    protected new void Update()
    {
        base.Update();
        // change the scale factor the object based on the current state
        if (ScaleDirection == SCALEDIRECTION.UP)
        {
            ScaleXY += 0.5f * Time.deltaTime;
        }
        else if (ScaleDirection == SCALEDIRECTION.DOWN)
        {
            ScaleXY -= 0.5f * Time.deltaTime;
        }

        // limit the scale in both directions
        if (ScaleXY > 1.15f)
        {
            ScaleDirection = SCALEDIRECTION.DOWN;
            ScaleXY = 1.15f;
        }

        if (ScaleXY < 0.85f)
        {
            ScaleDirection = SCALEDIRECTION.UP;
            ScaleXY = 0.85f;
        }

        // apply the scale factor
        transform.localScale = new Vector3(StartingScale * ScaleXY, StartingScale * ScaleXY, transform.localScale.z);
    }
    protected override void SelfDestruction()
    {
        Instantiate(bombItem, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
