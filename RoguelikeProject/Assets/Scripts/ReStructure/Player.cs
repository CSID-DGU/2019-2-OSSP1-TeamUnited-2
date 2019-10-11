using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Player : Unit
{
    protected double invincible; // 단위는 60프레임 초 입니다.
    public Wieldable mainHand;

    public GameObject faceArrow; // --for debug-- 플레이어가 보는 방향을 표현

    void Start()
    {
        // --for debug--
        invincible = 5.0;
        faceArrow = Instantiate(faceArrow, (Vector2)transform.position, transform.rotation) as GameObject;
    }

    public new void GetStrike(Strike strike)
    // Unit의 GetStrike를 공유합니다, 다만 플레이어는 데미지를 입을 경우 추가적인 무적 타임이 존재합니다.
    {
        base.GetStrike(strike);
        if (strike.attackType.damage > 0)
            invincible += 1.0;
    }

    new void Update()
    {
        // 무적의 처리와 스프라이트 깜빡임의 처리
        if (invincible > 0.0)
        {
            invincible -= (1.0 / 60.0);
            if (invincible % 0.5 > 0.25)
                GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 90);
            else
                GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        }

        // 방향키를 입력받아 이동을 처리하기 위해 Move 메서드를 호출합니다.
        int horizontal = (int)Math.Round((Input.GetAxisRaw("Horizontal")));
        int vertical = (int)Math.Round((Input.GetAxisRaw("Vertical")));
        Vector2 keyboardDirection = new Vector2(horizontal, vertical);
        keyboardDirection.Normalize();
        Move(keyboardDirection);

        // 캐릭터의 방향을 마우스 방향으로 맞추어 줍니다.
        // TODO :: 현재 마우스 방향으로 즉시 회전하게 되어있습니다, 시간지연을 넣는 편이 부드러울 것입니다.
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 mouseDirection = (Vector2)(mousePosition - transform.position);
        mouseDirection.Normalize();
        faceDirection = mouseDirection;

        // 플레이어가 보는 방향을 그래픽적으로 표현
        faceArrow.transform.position = (Vector2)((Vector2)transform.position + faceDirection);

        // 마우스 입력를 Wieldable 객체로 연결
        if (Input.GetMouseButtonDown(0))
        {
            // Debug.Log("left mouse pushed");
            mainHand.OnPush(); // 추후 해당 부분이 구현되면 주석을 해제해주세요.
            mainHand.holding = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            // Debug.Log("left mouse released");
            // mainHand.OnRelease(); // 추후 해당 부분이 구현되면 주석을 해제해주세요.
            mainHand.holding = false;
        }
        
        // 무기들의 owner 링크 처리
        mainHand.owner = gameObject;

        // 무기들의 aim 처리
        mainHand.aim = faceDirection;

        // 무기들의 인스턴스 위치/방향을 유닛과 맞춰준다.
        mainHand.transform.position = transform.position;
        mainHand.transform.rotation = transform.rotation;
    }
}
