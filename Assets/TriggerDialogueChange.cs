using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDialogueChange : MonoBehaviour
{
    [SerializeField] List<Message> newDialogue;

    private bool hasResetOnce = false;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.tag.Equals("Player") || hasResetOnce) return;

        hasResetOnce = true;
        GameObject.FindAnyObjectByType<LevelManager>().restartDialogue(newDialogue);
    }
}
