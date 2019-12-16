using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Guide : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Text>().color.a > 0)
        {
            GetComponent<Text>().color = new Color(GetComponent<Text>().color.r, GetComponent<Text>().color.g, GetComponent<Text>().color.b, GetComponent<Text>().color.a - Time.deltaTime * 0.5f);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
