using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    public bool unlocked = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.tag.Equals("Player") && !unlocked) { return; }


        Debug.Log("Loading Next Level");
    }
}
