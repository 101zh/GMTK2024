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
    float time;
    string timeString;

    public bool speedrunning = false;
    public bool start = false;
    public bool stop = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        stopwatch = new Stopwatch();
        text = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void Update()
    {
        if (speedrunning && start)
        {
            if (stop)
            {
                start = false;
                speedrunning = false;
                RecordTime();
            }
            timeElapsed = stopwatch.ElapsedMilliseconds;
            Format();
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            ResetBest();
        }
    }

    public void StartTime()
    {
        start = true;
        stopwatch.Stop();
        stopwatch.Start();
        transform.GetChild(0).gameObject.SetActive(true);
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

    void RecordTime()
    {
        Format();
        time = timeElapsed;
        timeString = text.text;

        if (time > PlayerPrefs.GetFloat("BestTimeFloat", -1))
        {
            PlayerPrefs.SetString("BestTimeString", timeString);
            PlayerPrefs.SetFloat("BestTimeFloat", time);
        }

        stop = false;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void QuitRun()
    {
        stopwatch.Stop();
        speedrunning = false;
        stop = false;
        start = false;
        time = 0;
        timeString = "";
        timeElapsed = 0;
    }

    void ResetBest()
    {
        PlayerPrefs.DeleteKey("BestTimeString");
        PlayerPrefs.DeleteKey("BestTimeFloat");
    }
}
