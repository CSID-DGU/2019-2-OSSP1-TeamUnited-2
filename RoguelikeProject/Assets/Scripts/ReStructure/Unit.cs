using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int HP;
    protected int currentHP;
    public int CurrentHP 
    { 
        get { return currentHP; } 
        set { currentHP = value; } 
    }
    private int coinScore;
    public int CoinScore
    {
        get { return coinScore; }
        set { coinScore = value; }
    }
    public double maxSpeed;
    protected Vector2 currentSpeed;
    protected float acceleration;
    protected Vector2 targetAcceleration;
    protected Vector2 rotationVector; // 플레이어가 보는 방향의 벡터값
    public Vector2 RotationVector
    {
        get { return rotationVector; }
        set { rotationVector = value; }
    }
    protected Animator animator = null;


    protected virtual void Start()
    {
        currentHP = HP;
    }

    protected void Move(Vector2 direction)
    // 호출시 단위속도만큼 이동합니다. Update등의 지속호출과 연계하여야 제대로 된 움직임이 나옵니다.
    {
        // 임시로 가속도는 최대속도의 0.05배로 했습니다.
        acceleration = GetComponent<Rigidbody2D>().mass * (float)maxSpeed / 20.0f;

        // 방향벡터를 추출하고 목표 가속치와 현재속도를 기록합니다.
        direction.Normalize();
        targetAcceleration = direction * acceleration;
        currentSpeed = GetComponent<Rigidbody2D>().velocity;

        // 임시로 임계속도를 구현했습니다. 임계속도 이상에서는 가속력이 줄어들고 지속적으로 감속당합니다.
        if (currentSpeed.magnitude > maxSpeed)
        {
            GetComponent<Rigidbody2D>().velocity *= 0.9f;
            targetAcceleration *= 0.75f;
        }
        
        // 실제 가속이 이루어집니다.
        GetComponent<Rigidbody2D>().AddForce(targetAcceleration, ForceMode2D.Impulse);
    }

    public virtual int GetDamage(int damage)
    // 모든 유닛의 HP 조작 처리는 이 메서드를 사용해야만 합니다.
    // 유닛 종류별 재정의 하는것이 *강력히* 권장됩니다.
    {
        // 실제로 받게 될 데미지를 추적연산합니다.
        int actualDamage = damage;
        
        // 실제 연산
        currentHP -= actualDamage;

        // 추적된 값을 반환합니다.        
        return actualDamage;
    }

    public virtual void GetStrike (Strike strike)
    // 모든 유닛의 모든 피격은 이 메서드를 사용해야만 합니다. 
    // 직접적인 HP, transform 등의 조작은 나중에 큰 문제를 야기할 수 있습니다.
    {
        // 실제로 받게 되는 물리량을 추적연산합니다.
        Strike actualStrike = new Strike(strike);
        // 호출하는 순간 강한 힘으로 오브젝트를 밀어버립니다.
        Vector2 pushDirection = (Vector2)transform.position - strike.attackPosition;
        pushDirection.Normalize(); 
        if (gameObject.GetComponent<Rigidbody2D>() && strike.force > 0.0)
           gameObject.GetComponent<Rigidbody2D>().AddForce(pushDirection * (float)strike.force, ForceMode2D.Impulse);
           
        // force 값은 양의 실수여야 합니다.
        if ((strike.force >= 0) != true)
            Debug.LogError("Force setting error" + strike.damage + "|" + strike.force + "|" + strike.attackPosition + "|" + transform.position);

        // 데미지가 있다면 데미지도 받습니다
        actualStrike.damage = GetDamage(strike.damage);

        // 애니메이터가 있다면 피격 애니메이션도 재생합니다.
        if (animator)
            animator.SetTrigger("Hit");  
    }

    protected virtual void SelfDestruction()
    // 기본적으로는 단순히 오브젝트를 파괴하는것으로 처리했습니다.
    // 오버라이딩해서 개별 처리를 구현하시길 바랍니다.
    {
        if (gameObject.name == "Wall(Clone)")
            GameObject.Find("GameManager").GetComponent<GameManager>().map[(int)transform.position.x, (int)transform.position.y] = 0;

        Destroy(gameObject);
    }

    protected void Update()
    {
        if (currentHP <= 0)
        {
            // SelfDestruction 메서드는 유닛 종류별로 오버라이딩하여 실행됩니다.
            SelfDestruction();
        }
    }
}
