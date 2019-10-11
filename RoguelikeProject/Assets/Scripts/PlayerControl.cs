using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public int life;
    public static int knowHP; // 4로 시작하는 이유 : 5부터 하면 0이 되었을때 끝나는데 그러면 초기값이 0이라서 GameManger에서 Invoke할때 잘안됨.
    public GameObject[] makeBullet;

    private static int weaponFlag;
    private Transform bullets;    
    private int bulletIndex;
    private int itemTime, invincible;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name == "item(Clone)") // 이름으로 가져옴
        {
            Destroy(col.gameObject);
            if (life < 4) // 4보다 작을때만 추가(안그러면 index 범위 벗어남)
                life++;
        }
        if (col.gameObject.name == "tumang(Clone)") // 투명망토
        {
            Destroy(col.gameObject);
            GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 30); /// 투명해짐
            invincible = 300;
        }

        if (col.gameObject.name == "bomb(Clone)") // 폭탄 아이템
        {
            Destroy(col.gameObject);
            bulletIndex = 1;
            itemTime = 600; // 지속시간(약 10초)
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag == "Enemys")
        {
            if (invincible <= 0)
            {
                invincible += 100;
                life--;
                Vector2 moveR = transform.position - col.transform.position;
                GetComponent<Rigidbody2D>().AddForce(moveR * 5f, ForceMode2D.Impulse);
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
        bullets= new GameObject("Bullets").transform;
    }
    void Update()
    {
        if (invincible > 0)
            invincible--;
        else
            GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255); // 밝기 원래대로.

        if (itemTime > 0)
            itemTime--;
        else
            bulletIndex = 0;



        knowHP = life;
        int horizontal = 0;
        int vertical = 0;
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));
        GetComponent<Rigidbody2D>().AddForce(new Vector2(horizontal, vertical) * .5f, ForceMode2D.Impulse);

        if (Input.GetMouseButtonDown(0))
        {
            //마우스 좌표 얻음
            Vector3 mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector2 direction = new Vector2(
            mousePosition.x - transform.position.x,
            mousePosition.y - transform.position.y);

            direction.Normalize();//정규화

            //총알 생성 위치는 player부터 마우스방향으로 1만큼 떨어진 위치
            Vector3 pos = transform.position;
            pos.x += direction.x;
            pos.y += direction.y;

            //총알 복사해서 생성
            if (bulletIndex == 0)
            {
                GameObject bull = Instantiate(makeBullet[0], pos, transform.rotation) as GameObject;
                bull.GetComponent<Rigidbody2D>().AddForce(direction * 1000.0f); // 총알을 direction방향으로 1000만큼의 힘으로 던진다.
                bull.transform.SetParent(bullets);
            }
            else if (bulletIndex == 1)
            {
                GameObject bullR = Instantiate(makeBullet[1], pos, transform.rotation) as GameObject;
                bullR.GetComponent<Rigidbody2D>().AddForce(direction * 1000.0f); // 총알을 direction방향으로 1000만큼의 힘으로 던진다.
                bullR.transform.SetParent(bullets);
            }

        }
    }
}