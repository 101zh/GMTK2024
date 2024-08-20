using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    private int buildIndex;
    private AudioManager audioManager;
    [SerializeField] private TMP_Text levelText;
    SceneTransition transition;

    private void Start()
    {
        transition = FindObjectOfType<SceneTransition>();
    }

    public void OnLevelClick()
    {
        transition.MoveIn(buildIndex);
        audioManager.Stop("TitleTheme");
    }

    public void Init(int level, int buildIndex, AudioManager audioManager)
    {
        levelText.text = level.ToString();
        this.buildIndex = buildIndex;
        this.audioManager = audioManager;
    }
}
