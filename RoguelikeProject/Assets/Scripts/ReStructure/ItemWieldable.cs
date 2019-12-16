using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemWieldable : MonoBehaviour
{
    public GameObject weapon;
    public Texture weaponImage;

    protected void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<Player>())
        {
            GameObject.Find("WeaponImage").GetComponent<RawImage>().texture = weaponImage;
            col.GetComponent<Player>().ChangeWeapon(weapon);
            Destroy(gameObject);
        }
    }
}
