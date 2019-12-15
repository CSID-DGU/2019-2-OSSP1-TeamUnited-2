using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snowMove : MonoBehaviour
{
    Vector3 originalTransform;
    int go1, go2;
    float direction, flow;
    // Start is called before the first frame update
    void Start()
    {
        originalTransform = transform.position;
        go1 = Random.Range(10, 50);
        go2 = Random.Range(10, 50);
        direction = Random.Range(-0.5f, 0.5f);
        flow = Random.Range(-1f, 0f);
    }

    // Update is called once per frame
    void Update()
    {

        if (transform.position.y > 0)
        {
            if (go1 > 0)
            {
                transform.Translate(direction, flow, 0);
                go1--;
            }
            else if (go2 > 0)
            {
                transform.Translate(-direction, flow, 0);
                go2--;
            }
            else
            {
                go1 = Random.Range(10, 50);
                go2 = Random.Range(10, 50);
            }
        }
        else
        {
            flow = Random.Range(-1f, 0f);
            transform.position = originalTransform;
        }
    }
}
