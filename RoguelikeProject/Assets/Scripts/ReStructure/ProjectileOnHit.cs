using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileOnHit : MonoBehaviour
{
    public AttackTypeBase singleAttack;
    public GameObject areaStrikeGenerator;

    void OnTriggerEnter2D(Collider2D col)
    {
        Strike strike = new Strike(singleAttack, transform.position, gameObject);
        if (col.gameObject.GetComponent<Unit>())
        {
            col.gameObject.GetComponent<Unit>().GetStrike(strike);
        }

        Instantiate(areaStrikeGenerator, transform.position, Quaternion.identity) as GameObject;
        areaStrikeGenerator.transform.SetParent(gameObject.transform);
        areaStrikeGenerator.GetComponent<AreaStrike>().Activate();

        Destroy(gameObject);
    }
}
