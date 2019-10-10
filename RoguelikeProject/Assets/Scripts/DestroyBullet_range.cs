using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBullet_range : MonoBehaviour
{
    public float rangeRadius;

    void OnTriggerEnter2D(Collider2D coll)
    {
        //StartCoroutine("TurnColorRed");
        //총알의 위치 기준으로 rangeRadius 범위의  collider2d를 추출
        Vector3 bulletPosition = gameObject.transform.position;
        Collider2D[] colliders2D = Physics2D.OverlapCircleAll(bulletPosition, rangeRadius);
        foreach (Collider2D _col in colliders2D)
        {
            //rigidbody2D를 가지면
            if (_col.GetComponent<Rigidbody2D>())
            {
                Vector3 _col_location = _col.gameObject.transform.position;
                Vector2 rangeAttackDirection = new Vector2(
                        _col_location.x - bulletPosition.x,
                        _col_location.y - bulletPosition.y);
                rangeAttackDirection.Normalize();
                //객체가 총알이 아닌경우
                if (_col.GetComponent<Rigidbody2D>() != gameObject.GetComponent<Rigidbody2D>() &&
                    _col.gameObject.tag != "Player")
                {
                    //마우스로부터 멀어지는 방향으로 addforce
                    //_col.GetComponent<Rigidbody2D>().AddForce(rangeAttackDirection * attackPower);
                    _col.GetComponent<Rigidbody2D>().AddForce(rangeAttackDirection * 10, ForceMode2D.Impulse);
                }
            }
        }
        Destroy(gameObject);
    }

    // IEnumerator TurnColorRed()
    // {
    //     GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 90);
    //     yield return new WaitForSeconds(.1f);
    //     GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255);
    // }
}
