using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class Dialog
{
    public string name = "Dialog";
    [TextArea(2,4)]
    public List<string> text;    
}

public class TextboxController : MonoBehaviour {

    public List<Dialog> dialogs;
    public RectTransform textboxTransform;
    public Animator textboxAnimator;
    public Text textBox;
    public float typeDelay = 0.1f;
    public AudioSource typeAudioSoure;
    public AudioClip typeAudio;
    public bool startDialog = false;
    public UnityEvent OnClose;

    Dialog currentDialog;
    int dialogTextsIndex;
    bool isTyping;
    bool nextText = true;

	// Use this for initialization
	void Start () {
        typeAudioSoure.clip = typeAudio;
        if (OnClose == null)
        {
            OnClose = new UnityEvent();
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (startDialog) {
            if (!isTyping)
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (nextText)
                    {
                        isTyping = true;
                        StartText();
                    } else if (!nextText)
                    {
                        HideDialog();
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Mouse0))
                {
                    StopAllCoroutines();
                    textBox.text = currentDialog.text[dialogTextsIndex];

                    dialogTextsIndex++;
                    if (dialogTextsIndex < currentDialog.text.Count)
                    {
                        nextText = true;
                    }

                    isTyping = false;
                }
            }
        } 
	}

    public void StartDialog(Dialog dialog)
    {
        startDialog = true;
        currentDialog = dialog;
        dialogTextsIndex = 0;
        textboxTransform.gameObject.SetActive(true);
        textboxAnimator.SetBool("Show", true);
        StartText();
    }

    public void StartText()
    {
        textBox.text = "";
        StartCoroutine(TypingText(currentDialog.text[dialogTextsIndex], 0));
    }

    public void HideDialog()
    {
        textboxAnimator.SetBool("Show", false);
        StartCoroutine(HidingDialog(1.2f));
    }

    IEnumerator TypingText(string text, int index)
    {
        if (index < text.Length)
        {
            nextText = false;
            isTyping = true;
            textBox.text += text[index];
            index++;
            typeAudioSoure.Play();
            yield return new WaitForSeconds(typeDelay);
            StartCoroutine(TypingText(text, index));
        } else
        {
            dialogTextsIndex++;
            if (dialogTextsIndex < currentDialog.text.Count)
            {
                nextText = true;
            }

            isTyping = false;
            yield return null;
        }
    }

    IEnumerator HidingDialog (float delay)
    {
        yield return new WaitForSeconds(delay);
        startDialog = false;
        textboxTransform.gameObject.SetActive(false);
        OnClose.Invoke();
    }
}
