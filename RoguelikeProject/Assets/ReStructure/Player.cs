﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Unit
{
    protected double invincible; // 단위는 초 입니다. (Time.deltaTime 사용)
    public Wieldable mainHand;

    public GameObject faceArrow; // 플레이어가 보는 방향을 그래픽적으로 표현하기 위한 이미지

    void Start()
    {
        invincible = 5.0;
        faceArrow = Instantiate(faceArrow, (Vector2)transform.position, transform.rotation) as GameObject;
        currentHP = 5;
    }

    public override int GetDamage(int damage)
    // 플레이어의 HP조작은 반드시 이 메서드를 통해서만 이루어져야 합니다.
    {
        // 실제로 받게 되는 데미지를 추적합니다.
        int actualDamage;

        // 이하 if문의 모든 경로에는 actualDamage변수의 처리가 포함되어야 합니다.
        // 회복인 경우 (데미지가 음수)
        if (damage < 0)
        {
            // 최대 HP를 넘길 수 없습니다.
            if (currentHP - damage > HP)
                actualDamage = currentHP - HP;
            else
                actualDamage = damage;
        }
        // 피해이지만 무적인 경우
        else if (invincible > 0)
        {
            actualDamage = 0;
        }
        // 그 외에는 데미지를 그대로 받습니다 (일단은)
        else
        {
            actualDamage = damage;
        }
        
        // 실제 연산된 피해 결과를 HP에 적용합니다. (회복일 수 있습니다)
        currentHP -= actualDamage;

        return actualDamage;
    }

    protected override void SelfDestruction()
    {
        gameObject.SetActive(false);
        Invoke("nextScene", 1);
    }
    void nextScene()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    public new Strike GetStrike(Strike strike)
    // Unit의 GetStrike를 활용합니다, 다만 플레이어는 데미지를 입을 경우 추가적인 무적 타임이 존재합니다.
    {
        // 실제로 받게 되는 물리량을 추적중입니다.
        Strike actualStrike = base.GetStrike(strike);

        // 실제로 받은 데미지가 존재한다면 무적시간 추가
        if (actualStrike.damage > 0 || actualStrike.force > 0)
            invincible += 1.0;

        return actualStrike;
    }

    public void Wield(Wieldable weapon)
    {
        mainHand = weapon;
        // TODO :: 버려진 무기에 대한 처리도 해야 할 것입니다.
    }

    new void Update()
    {
        // 우선 모든 유닛에 대한 공통적인 처리
        base.Update();

        // 무적의 처리와 스프라이트 깜빡임의 처리
        if (invincible > 0.0)
        {
            invincible -= Time.deltaTime;
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
            mainHand.OnPush();
        else if (Input.GetMouseButtonUp(0))
            mainHand.OnRelease();
        else if (Input.GetMouseButton(0))
            mainHand.OnHold();

        // 무기들의 소유자 링크 처리
        mainHand.owner = gameObject;

        // 무기들의 조준점 처리
        mainHand.aim = faceDirection;

        // 무기들의 인스턴스 위치/방향을 유닛과 맞춰준다.
        mainHand.transform.position = transform.position;
        mainHand.transform.rotation = transform.rotation;
    }
}
