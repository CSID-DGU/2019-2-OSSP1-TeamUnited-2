﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Unit
{
    protected double invincible; // 단위는 초 입니다. (Time.deltaTime 사용)
    // public GameObject weapon1;
    public GameObject mainWeapon;
    public GameObject aimObject;
    [HideInInspector]
    public float angle;

    new void Start()
    {
        // 모든 유닛의 공통 처리를 합니다.
        base.Start();
        // 플레이어는 초기 무적과 초기 hp값을 다르게 갖습니다 (For Debug)
        invincible = 5.0;
        currentHP = 5;

        aimObject = Instantiate(aimObject, (Vector2)transform.position, Quaternion.identity, transform);

        // 무기 인스턴스 생성
        Wield(mainWeapon);
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

        // 실제로 받은 데미지가 존재한다면 무적시간 추가
        if (actualDamage > 0)
            invincible += 1.0;

        // 추적이 필요할 경우를 위한 반환
        return actualDamage;
    }
    protected override void SelfDestruction()
    {
        gameObject.SetActive(false);
        Invoke("nextScene", 1);
    }
    void nextScene()
    {
        SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
    }
    public void Wield(GameObject newWeapon)
    {
        // 주 무기의 인스턴스를 생성하고 부모를 플레이어로 설정.
        mainWeapon = Instantiate(newWeapon);
        mainWeapon.transform.SetParent(GameObject.Find("GameManager").transform);

        // 무기의 소유자를 설정해줍니다.
        if (mainWeapon.GetComponent<Wieldable>() == false)
            Debug.LogError("Weapon has no Wieldable component");
        mainWeapon.GetComponent<Wieldable>().Owner = gameObject;
    }
    public void ChangeWeapon(GameObject newWeapon)
    {
        // 현재로써는 기존의 무기는 그냥 삭제하도록 되어있습니다.
        Destroy(mainWeapon);
        Wield(newWeapon);
    }
    protected new void Update()
    {
        // 우선 모든 유닛에 대한 공통적인 처리
        base.Update();

        // 무적의 처리와 스프라이트 깜빡임의 처리
        if (invincible > 0.0)
        {
            invincible -= Time.deltaTime;
            if (invincible % 0.5 > 0.25)
                transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 90);
            else
                transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        }

        // 방향키를 입력받아 이동을 처리하기 위해 Move 메서드를 호출합니다.
        int horizontal = (int)Math.Round((Input.GetAxisRaw("Horizontal")));
        int vertical = (int)Math.Round((Input.GetAxisRaw("Vertical")));
        Vector2 keyboardDirection = new Vector2(horizontal, vertical);
        keyboardDirection.Normalize();
        Move(keyboardDirection);

        // 마우스의 방향을 추출합니다
        // TODO :: 현재 마우스 방향으로 즉시 회전하게 되어있습니다, 시간지연을 넣는 편이 부드러울 것입니다.
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        aimObject.transform.position = mousePosition;
        aimObject.transform.Translate(0, 0, 10f);
        aimObject.transform.SetParent(GameObject.Find("GameManager").transform);
        Vector2 mouseDirection = (Vector2)(mousePosition - transform.position);
        mouseDirection.Normalize();

        // 캐릭터의 실제 방향과(transform.rotation), 단위벡터값으로 변환된 사용자 정의 방향값(moustDirection)을 커서와 맞추어 줍니다
        // TODO :: 현재 마우스 커서의 방향으로 즉시 바뀌게 되어있습니다. 지연을 넣는 편이 부드러울 것입니다.
        rotationVector = mouseDirection;
        // transform.rotation = Quaternion.LookRotation(mouseDirection, Vector3.up);
        // transform.rotation = Quaternion.Euler(10,10,10);

        // 플레이어가 보는 방향을 그래픽적으로 표현
        //transform.GetChild(0).transform.position = (Vector2)((Vector2)transform.position + rotationVector);

        //현재 무기의 위치 
        Vector2 positionOnScreen = transform.GetChild(0).transform.position;

        //// 무기와 마우스의 위치의 각도 
        angle = AngleBetweenTwoPoints(positionOnScreen, mousePosition) + 90.0f;
        // Debug.Log(positionOnScreen + " " + mousePosition + " " + angle);

        //// 각도만큼 로테이션값 주기 
        transform.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));



        float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
        {
            return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
        }


        // 마우스 입력를 Wieldable 객체로 연결
        if (Input.GetMouseButtonDown(0))
        {
            mainWeapon.GetComponent<Wieldable>().OnPush();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            mainWeapon.GetComponent<Wieldable>().OnRelease();
        }
        else if (Input.GetMouseButton(0))
        {
            mainWeapon.GetComponent<Wieldable>().OnHold();
        }
    }
}
