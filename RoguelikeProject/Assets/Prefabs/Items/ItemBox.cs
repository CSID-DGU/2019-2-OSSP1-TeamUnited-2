using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : Unit
{
    public GameObject bombItem;
    protected override void Start()
    {
        currentHP = HP;
    }
    protected new void Update()
    {
        base.Update();

    }
    protected override void SelfDestruction()
    {
        Instantiate(bombItem, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
