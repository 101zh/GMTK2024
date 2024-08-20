using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject titleMenu;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private GameObject LevelsLayoutGrid;
    [SerializeField] private GameObject LevelButtonPrefab;
    [SerializeField] private SceneTransition transition;

    private AudioManager audioManager;

    private void Start()
    {
        float bestTime = PlayerPrefs.GetFloat("BestTimeFloat", -1.0f);
        string bestTimeString = "";
        if (bestTime > 0.0f)
        {
            bestTimeString = PlayerPrefs.GetString("BestTimeString", "--:--:--:---");
            statsText.text = $"Quickest Time ★:\n{bestTimeString}";
        }

        float masterVolume = PlayerPrefs.GetFloat("LastMasterVolume", 0.0f);
        refreshVolumeSlideTo(masterVolume);

        if (audioManager == null)
            audioManager = FindObjectOfType<AudioManager>();

        audioManager.Stop("MainTheme");
        audioManager.Play("TitleTheme");

        int count = 1;
        for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            {
                LevelButton levelButton = Instantiate(LevelButtonPrefab, LevelsLayoutGrid.transform).GetComponent<LevelButton>();
                levelButton.Init(count, i, audioManager);
                count++;
            }
        }

    }

    public void OnStartButtonPress()
    {
        StopTitleTheme();
        transition.MoveIn(1);
    }
    public void OnSpeedrunButtonPress()
    {
        FindObjectOfType<SpeedrunManager>().speedrunning = true;

        StopTitleTheme();
        transition.MoveIn(1);
    }

    public void OnQuitButtonPress()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }

    public void OnMenuChangeButtonPress(GameObject menu)
    {
        titleMenu.SetActive(false);
        menu.SetActive(true);
    }

    public void onBackButtonPressed(GameObject curMenu)
    {
        curMenu.SetActive(false);
        titleMenu.SetActive(true);
    }

    public void setVolume(float volume)
    {
        masterMixer.SetFloat("MasterVolume", volume);
        PlayerPrefs.SetFloat("LastMasterVolume", volume);
    }

    private void refreshVolumeSlideTo(float value)
    {
        volumeSlider.value = value;
    }

    private void StopTitleTheme()
    {
        audioManager.Stop("TitleTheme");
    }

}