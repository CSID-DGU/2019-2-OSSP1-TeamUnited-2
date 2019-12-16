using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimeLimit : MonoBehaviour
{
    float limitTime = 100;
    // Update is called once per frame
    void Update()
    {
        limitTime -= Time.deltaTime;

        if (limitTime < 0)
        {
            SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
        }
        else
        {
            GetComponent<Text>().text = "TimeLimit : " + (int)limitTime;
        }

    }
}
