using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    [SerializeField] private TMP_Text textBox;
    [SerializeField] private List<Message> messages;

    private Coroutine dialogue;
    private bool doneTyping;
    AudioManager audioManager;
    GameObject kira;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = GameObject.FindObjectOfType<AudioManager>();
        if (!audioManager.IsPlaying("MainTheme"))
        {
            audioManager.Play("MainTheme");
        }

        kira = GameObject.FindGameObjectWithTag("Player");

        dialogue = StartCoroutine(writeDialogue());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) || kira.transform.position.y < -7.0f)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void restartDialogue(List<Message> newDialogue)
    {
        StopAllCoroutines();
        textBox.text = " ";
        messages = newDialogue;
        dialogue = StartCoroutine(writeDialogue());
    }

    IEnumerator writeDialogue()
    {
        foreach (Message message in messages)
        {
            StartCoroutine(typeOut(message));
            yield return new WaitUntil(IsDoneTyping);
        }

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
