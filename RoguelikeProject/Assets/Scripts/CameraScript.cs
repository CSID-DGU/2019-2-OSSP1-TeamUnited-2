using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject player;

    Transform AT;
    void Start()
    {
        AT = player.transform;
    }
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, AT.position, 2f * Time.deltaTime);
        transform.Translate(0, 0, -10);
    }
}