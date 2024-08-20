using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static bool GameIsPaused = false;
    GameObject pauseMenuUI;

    AudioManager audioManager;
    SceneTransition transition;

    private void Start()
    {
        transition = FindObjectOfType<SceneTransition>();
        pauseMenuUI = transform.GetChild(0).GetChild(1).gameObject;
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
        Resume();
        transition.MoveIn(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        audioManager.Play("BlipSelect");
        FindObjectOfType<SpeedrunManager>().QuitRun();
        Resume();
        transition.MoveIn(0);
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        GameIsPaused = true;
    }

    public void makeTheChoice(bool accept)
    {
        LevelManager.makeChoice(accept);
    }

}
