using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructor : MonoBehaviour
// 붙은 오브젝트를 일정시간후 파괴시키는 간이 소멸자입니다.
{
    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
    void Start()
    {
        StartCoroutine(SelfDestruct());
    }
}
