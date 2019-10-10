using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject player;
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, player.transform.position, 2f * Time.deltaTime);
        transform.Translate(Vector3.back);
    }
}