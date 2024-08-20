 using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;

public class SpeedrunManager : MonoBehaviour
{
    public Stopwatch stopwatch;
    public float timeElapsed;
    TextMeshProUGUI text;

    private void Start()
    {
        stopwatch = new Stopwatch();
        text = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        stopwatch.Start();
    }

    private void Update()
    {
        timeElapsed = stopwatch.ElapsedMilliseconds;
        Format();
    }

    void Format()
    {
        float time = timeElapsed;

        float hours;
        float minutes;
        float seconds;
        float mill;

        seconds = (int) (time / 1000);
        mill = time % 1000;
        minutes = (int) (seconds / 60); 
        seconds = seconds % 60;
        hours = (int) (minutes / 60);
        minutes = minutes % 60;

        string s = seconds > 9 ? seconds.ToString() : "0" + seconds.ToString();
        string min = minutes > 9 ? minutes.ToString() : "0" + minutes.ToString();
        string mil = mill > 9 ? mill > 99 ? mill.ToString() : "0" + mill.ToString() : "00" + mill.ToString();

            text.text = hours.ToString() + " : " + min + " : " + s + " : " + mil;
    }
}
