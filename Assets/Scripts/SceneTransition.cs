using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public float transTime = 1f;
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        MoveAway();
    }

    // Update is called once per frame
    public void MoveAway()
    {
        anim.Play("TransMoveAway");
    }

    public void MoveIn(int scene)
    {
        anim.Play("TransMoveIn");
        StartCoroutine(WaitToLoad(scene));
    }

    IEnumerator WaitToLoad(int scene)
    {
        yield return new WaitForSeconds(transTime);
        SceneManager.LoadScene(scene);
    }
}
