using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public float transTime = 1f;
    Animator anim;

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
        if (SceneManager.GetActiveScene().name.Equals("Level 1") && speedrunManager.speedrunning)
        {
            speedrunManager.StartTime();
        }
        anim.Play("TransMoveAway");
    }

    public void MoveIn(int scene)
    {
        if (scene == 0 && speedrunManager.speedrunning && SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings-1)
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
