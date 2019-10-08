using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBullet : MonoBehaviour
{
    public GameObject makeBullet;
    // Update is called once per frame
    void Update()
    {
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
            GameObject ins = Instantiate(makeBullet, pos, transform.rotation) as GameObject;

            ins.GetComponent<Rigidbody2D>().AddForce(direction * 1000.0f); // 총알을 direction방향으로 1000만큼의 힘으로 던진다.
        }
    }
}