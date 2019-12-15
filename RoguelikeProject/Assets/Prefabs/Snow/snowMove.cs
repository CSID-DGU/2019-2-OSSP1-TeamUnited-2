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
        go1 = Random.Range(10, 30);
        go2 = Random.Range(10, 30);
        direction = Random.Range(-0.4f, 0.4f);
        flow = Random.Range(-0.4f, -0.2f);
    }

    // Update is called once per frame
    void Update()
    {

        if (transform.position.y > 0)
        {
            transform.GetChild(0).rotation = Quaternion.Euler(0, 0, Random.Range(0, 360) * Time.deltaTime * 10);
            if (go1 > 0)
            {
                transform.Translate(direction * Time.deltaTime * 10, flow * Time.deltaTime * 10, 0);
                go1--;
            }
            else if (go2 > 0)
            {
                transform.Translate(-direction * Time.deltaTime * 10, flow * Time.deltaTime * 10, 0);
                go2--;
            }
            else
            {
                go1 = Random.Range(10, 30);
                go2 = Random.Range(10, 30);
            }
        }
        else
        {
            flow = Random.Range(-0.4f, -0.2f);
            transform.position = originalTransform;
        }
    }
}
