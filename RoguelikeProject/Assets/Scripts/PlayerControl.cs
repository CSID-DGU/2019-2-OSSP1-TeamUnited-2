using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public int life;
    public static int knowHP; // 4로 시작하는 이유 : 5부터 하면 0이 되었을때 끝나는데 그러면 초기값이 0이라서 GameManger에서 Invoke할때 잘안됨.
    public double invincible;
    public GameObject makeBullet;
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Item")
        {
            Destroy(col.gameObject);
            if (life < 4)
                life++;
        }
    }



    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag == "Enemys")
        {
            if (invincible <= 0)
            {
                invincible += 1.0;
                life--;
                Vector2 moveR = transform.position - col.transform.position;
                GetComponent<Rigidbody2D>().AddForce(moveR * 10, ForceMode2D.Impulse);
                StartCoroutine("TurnColor"); // 깜빡 거리기
            }

        }

    }

    IEnumerator TurnColor()
    {
        GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 90);
        yield return new WaitForSeconds(.1f);
        GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
    }
    void Start()
    {
        knowHP = life;
    }
    void Update()
    {
        if (invincible >= 0)
            invincible -= 0.02;
        knowHP = life;
        int horizontal = 0;
        int vertical = 0;
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));
        GetComponent<Rigidbody2D>().AddForce(new Vector2(horizontal, vertical) * .5f, ForceMode2D.Impulse);

        if (Input.GetMouseButtonDown(0))
        {
            //마우스 위치 받아옴
            Vector3 mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            Vector2 direction = new Vector2(
            mousePosition.x - transform.position.x,
            mousePosition.y - transform.position.y);

            direction.Normalize();
            //총알 생성 위치는 player부터 마우스방향으로 1만큼 떨어진 위치
            Vector3 pos = transform.position;
            pos.x += direction.x;
            pos.y += direction.y;
            //총알 복사해서 생성
            GameObject ins = Instantiate(makeBullet, pos, transform.rotation) as GameObject;

            ins.GetComponent<Rigidbody2D>().AddForce(direction * 300.0f);
        }
    }
}