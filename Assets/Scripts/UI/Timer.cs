using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float time;
    public Text TimerText;
    public Image Fill;
    public float Max;

    private void Update()
    {
        time -= Time.deltaTime;
        TimerText.text = " " + (int)time;
        Fill.fillAmount = time / Max;

        if (time < 0)
        time = 0;
    }
}
