using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInstant : MonoBehaviour
{
    public int HPMod;

    protected void OnTriggerStay2D(Collider2D col)
    {
        if (col.GetComponent<Player>())
        {
            col.GetComponent<Player>().GetDamage(-HPMod);
        }
    }
}
