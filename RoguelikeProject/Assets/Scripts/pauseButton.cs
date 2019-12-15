using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pauseButton : MonoBehaviour
{
    bool startStop = true;
    public Image image;
    public Sprite stop, start;
    public void Stop()
    {
        if (startStop)
        {
            Time.timeScale = 0;
            startStop = false;
            image.sprite = start;
        }
        else
        {
            Time.timeScale = 1;
            startStop = true;
            image.sprite = stop;
        }
    }
}
