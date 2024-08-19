﻿using System.Collections.Generic;
using TMPro;
using UnityEditor;
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

    private AudioManager audioManager;

    private void Start()
    {
        float bestTime = PlayerPrefs.GetFloat("BestTime", -1.0f);
        string bestTimeString = "";
        if (bestTime > 0.0f)
        {
            bestTimeString = bestTime.ToString();
        }
        statsText.text = $"Quickest Time ★:\n{bestTimeString}";

        float masterVolume = PlayerPrefs.GetFloat("LastMasterVolume", 0.0f);
        refreshVolumeSlideTo(masterVolume);

        if (audioManager == null)
            audioManager = FindObjectOfType<AudioManager>();

        audioManager.Stop("MainTheme");
        audioManager.Play("TitleTheme");

        int index = 0;
        int count = 1;
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.path.Contains("level", System.StringComparison.OrdinalIgnoreCase))
            {
                LevelButton levelButton = Instantiate(LevelButtonPrefab, LevelsLayoutGrid.transform).GetComponent<LevelButton>();
                levelButton.Init(count, index, audioManager);
                count++;
            }
            index++;
        }


    }

    public void OnStartButtonPress()
    {
        StopTitleTheme();
        SceneManager.LoadScene("Level 1"); //TODO: load correct scene
    }
    public void OnSpeedrunButtonPress()
    {
        StopTitleTheme();
        SceneManager.LoadScene("Level 1"); //TODO: load corrrect scene
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