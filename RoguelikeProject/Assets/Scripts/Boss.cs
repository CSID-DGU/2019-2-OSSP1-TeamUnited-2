using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss : Unit
{
    public GameObject target;
    private GameObject player;
    public int meleeDamage;
    public double force;

    protected override void Start()
    {
        currentHP = HP;
        InvokeRepeating("FindPlayer", Random.Range(0.0f, 1.0f), 1);
    }
    protected new void Update()
    {
        base.Update();

        if (target != null)
        {
            float angle = Mathf.Atan2(transform.position.y - target.transform.position.y, transform.position.x - target.transform.position.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
            Vector2 direction = target.transform.position - transform.position;
            Move(direction);
        }
    }
    protected override void SelfDestruction()
    {
        Destroy(gameObject);
        SceneManager.LoadScene("GameClear", LoadSceneMode.Single);
    }
    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.gameObject.GetComponent<Player>())
            coll.gameObject.GetComponent<Player>().GetStrike(new Strike(meleeDamage, force, transform.position));
    }
    void FindPlayer()
    {
        player = GameObject.Find("Player");

        float distance = Vector2.Distance(player.transform.position, transform.position);
        if (distance < 15.0f)
        {
            Debug.Log("Boss tracking : " + distance);
            target = player;
        }
        else
        {
            Debug.Log("Boss idle : " + distance);
            target = null;
        }
    }
}