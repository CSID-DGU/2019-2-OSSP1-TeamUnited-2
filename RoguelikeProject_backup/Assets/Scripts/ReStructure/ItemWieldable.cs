using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWieldable : MonoBehaviour
{
    public GameObject weapon;

    protected void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<Player>())
        {
            col.GetComponent<Player>().ChangeWeapon(weapon);
            Destroy(gameObject);
        }
    }
}
