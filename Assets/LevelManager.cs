using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = GameObject.FindObjectOfType<AudioManager>();
        if (!audioManager.IsPlaying("MainTheme"))
        {
            audioManager.Play("MainTheme");
        }
    }
}
