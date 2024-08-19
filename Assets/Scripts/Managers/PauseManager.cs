using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static bool GameIsPaused = false;
    GameObject pauseMenuUI;

    AudioManager audioManager;

    private void Start()
    {
        pauseMenuUI = transform.GetChild(0).GetChild(0).gameObject;
        audioManager = GameObject.FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        audioManager.Play("BlipSelect");
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
        GameIsPaused = false;
    }

    public void Restart()
    {
        audioManager.Play("BlipSelect");
        // TODO: Reload Scene
        Resume(); // get rid of this
    }

    public void MainMenu()
    {
        audioManager.Play("BlipSelect");
        // TODO: Go to main menu
        Resume(); // get rid of this
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        GameIsPaused = true;
    }
}
