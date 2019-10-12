using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWieldable : MonoBehaviour
{
    public Wieldable weapon;

    protected override OnTriggerStay2D(Collider2D col)
    {
        if (col.GetComponent<Player>())
        {
            
        }
    }
}
