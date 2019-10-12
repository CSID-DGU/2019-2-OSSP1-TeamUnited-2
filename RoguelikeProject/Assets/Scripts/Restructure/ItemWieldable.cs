﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWieldable : MonoBehaviour
{
    public Wieldable weapon;

    protected void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<Player>())
        {
            col.GetComponent<Player>().Wield(weapon);
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
