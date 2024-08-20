using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public float transTime = 1f;
    Animator anim;

    [Header("Only Assign on End Level")]
    public int levelBeforeStop = 12;
    public int[] timerStopScenesIndex;

    SpeedrunManager speedrunManager;    

    void Start()
    {
        anim = GetComponent<Animator>();
        speedrunManager = FindObjectOfType<SpeedrunManager>();
        MoveAway();
    }

    // Update is called once per frame
    public void MoveAway()
    {
        if (speedrunManager != null && SceneManager.GetActiveScene().name.Equals("Level 1") && speedrunManager.speedrunning)
        {
            speedrunManager.StartTime();
        }
        anim.Play("TransMoveAway");
    }

    public void MoveIn(int scene)
    {
        foreach (int i in timerStopScenesIndex)
            if (scene == i)
                speedrunManager.stop = true;

        anim.Play("TransMoveIn");
        StartCoroutine(WaitToLoad(scene));
    }

    IEnumerator WaitToLoad(int scene)
    {
        yield return new WaitForSeconds(transTime);
        SceneManager.LoadScene(scene);
    }
}
