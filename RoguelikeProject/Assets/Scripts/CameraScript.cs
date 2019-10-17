using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject player;
    public GameObject heart;
    public GameObject heartBoard;
    ArrayList HeartList = new ArrayList(); // 하트 관리

    void Start()
    {
        for (int i = 0; i < player.GetComponent<Player>().CurrentHP; i++) // 현재 체력만큼 생성.
        {
            GameObject heartImg = Instantiate(heart);
            heartImg.transform.SetParent(heartBoard.transform);
            HeartList.Add(heartImg);
        }
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, player.transform.position, 2f * Time.deltaTime);
        transform.Translate(Vector3.back);
        if (HeartList.Count < player.GetComponent<Player>().CurrentHP)
        {
            GameObject heartImg = Instantiate(heart);
            heartImg.transform.SetParent(heartBoard.transform);
            HeartList.Add(heartImg);
        }
        else if(HeartList.Count > player.GetComponent<Player>().CurrentHP)
        {
            for (int i = 0; i < HeartList.Count - player.GetComponent<Player>().CurrentHP; i++)
            {
                if (HeartList.Count > 0) // HP가 0보다 클때.
                {
                    Destroy((GameObject)HeartList[0]);
                    HeartList.RemoveAt(0);
                }
            }
        }

    }


}