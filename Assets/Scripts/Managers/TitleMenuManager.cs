using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject titleMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private Slider volumeSlider;
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
        audioManager.Play("TitleTheme");
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

    public void OnLevelSelectButtonPress()
    {
        SceneManager.LoadScene("Levels"); //TODO: load correct scene
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

    public void onBackButtonPressed()
    {
        optionsMenu.SetActive(false);
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