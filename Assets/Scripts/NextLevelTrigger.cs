using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelTrigger : MonoBehaviour
{
    public bool unlocked = false;

    SceneTransition transition;

    private void Start()
    {
        transition = FindObjectOfType<SceneTransition>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.tag.Equals("Player") || !unlocked) { return; }

        LoadNext();
    }

    void LoadNext()
    {
        // TODO: Add Transition

        if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            transition.MoveIn(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            transition.MoveIn(0);
        }
    }
}
