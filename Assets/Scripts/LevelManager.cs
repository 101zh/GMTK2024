using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private bool isEndingScene;
    [SerializeField] private TMP_Text textBox;
    [SerializeField] private List<Message> messages;
    [SerializeField] private GameObject SelectionChoice;
    [SerializeField] private GameObject declineScreen;
    [SerializeField] private GameObject acceptScreen;

    SceneTransition transition;

    private Coroutine dialogue;
    private bool doneTyping;
    AudioManager audioManager;
    GameObject kira;

    // Start is called before the first frame update
    void Start()
    {
        transition = FindObjectOfType<SceneTransition>();
        audioManager = GameObject.FindObjectOfType<AudioManager>();
        if (!audioManager.IsPlaying("MainTheme"))
        {
            audioManager.Play("MainTheme");
        }

        kira = GameObject.FindGameObjectWithTag("Player");

        if (isEndingScene)
        {
            audioManager.Stop("MainTheme");
            StartCoroutine(EndingScene());
        }
        else
        {
            dialogue = StartCoroutine(writeDialogue());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) || kira.transform.position.y < -7.0f)
        {
            transition.MoveIn(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void restartDialogue(List<Message> newDialogue)
    {
        StopAllCoroutines();
        textBox.text = " ";
        messages = newDialogue;
        dialogue = StartCoroutine(writeDialogue());
    }

    IEnumerator EndingScene()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(writeDialogue());
        yield return new WaitUntil(DialogueIsDone);
        Time.timeScale = 0.0f;
        SelectionChoice.SetActive(true);
        Time.timeScale = 1.0f;
        yield return new WaitUntil(ChoiceMade);

        SelectionChoice.SetActive(false);

        acceptScreen.SetActive(acceptJob);
        declineScreen.SetActive(!acceptJob);

        yield return new WaitForSeconds(2.5f);


        if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            transition.MoveIn(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            transition.MoveIn(0);
        }

    }

    bool dialogueDone = false;
    IEnumerator writeDialogue()
    {
        dialogueDone = false;
        foreach (Message message in messages)
        {
            StartCoroutine(typeOut(message));
            yield return new WaitUntil(IsDoneTyping);
        }
        dialogueDone = true;

    }

    private static bool choiceMade = false;
    private static bool acceptJob = false;

    public static void makeChoice(bool accept)
    {
        choiceMade = true;
        acceptJob = accept;
    }

    private bool ChoiceMade()
    {
        return choiceMade;
    }

    private bool DialogueIsDone()
    {
        return dialogueDone;
    }

    IEnumerator typeOut(Message message)
    {
        doneTyping = false;

        for (int i = 0; i < message.message.Length; i++)
        {
            textBox.text = textBox.text.Substring(0, textBox.text.Length - 1);
            string cursor = i % 2 == 0 ? "|" : " ";
            textBox.text += message.message[i] + cursor;
            yield return new WaitForSeconds(1f / message.charPerSec);
        }
        if (textBox.text.Contains("|"))
        {
            textBox.text = textBox.text.Substring(0, textBox.text.Length - 1);
        }

        yield return new WaitForSeconds(message.leaveMessageForSeconds);
        if (message.clearTextBox) textBox.text = " ";

        doneTyping = true;
    }

    private bool IsDoneTyping()
    {
        return doneTyping;
    }

}
[System.Serializable]
public class Message
{
    public string message;
    public float charPerSec = 15f;
    public float leaveMessageForSeconds;
    public bool clearTextBox = true;
}
