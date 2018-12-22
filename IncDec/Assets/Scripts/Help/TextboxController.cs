using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class Dialog
{
    public string name = "Dialog";
    public List<DialogText> dialogText;
}

[System.Serializable]
public class DialogText
{
    [TextArea(2, 4)]
    public string text;
    public Sprite sprite;
    public float width = 300;
    public float height = 300;

    public Vector2 textboxPos;
    public Vector2 imagePos;
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

    public Image image;
    public Animator imageAnimator;

    public UnityEvent OnClose;

    Dialog currentDialog;
    int dialogTextsIndex;
    bool isTyping;
    bool nextText = true;
    bool isTransitioning;

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
        if (startDialog && !isTransitioning) {
            if (!isTyping)
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (nextText)
                    {
                        isTyping = true;
                        StartCoroutine(StartingText());
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
                    textBox.text = currentDialog.dialogText[dialogTextsIndex].text;

                    dialogTextsIndex++;
                    if (dialogTextsIndex < currentDialog.dialogText.Count)
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
        image.gameObject.SetActive(true);
        textboxAnimator.SetBool("Show", true);
        StartText();
    }

    public void StartText()
    {
        DialogText dialogText = currentDialog.dialogText[dialogTextsIndex];

        if (currentDialog.dialogText[dialogTextsIndex].sprite != null)
        {
            image.sprite = dialogText.sprite;
            image.rectTransform.sizeDelta = new Vector2(dialogText.width, dialogText.height);
            image.rectTransform.anchoredPosition = dialogText.imagePos;
            imageAnimator.SetBool("Show", true);
        }

        textboxTransform.anchoredPosition = dialogText.textboxPos;
        textBox.text = "";
        textboxAnimator.SetBool("Show", true);
        StartCoroutine(TypingText(dialogText.text, 0, .5f));
    }

    public void HideDialog()
    {
        isTransitioning = true;
        textboxAnimator.SetBool("Show", false);
        imageAnimator.SetBool("Show", false);
        StartCoroutine(HidingDialog(1.2f));
    }

    IEnumerator StartingText()
    {
        isTransitioning = true;
        textboxAnimator.SetBool("Show", false);
        imageAnimator.SetBool("Show", false);
        yield return new WaitForSeconds(1.2f);
        StartText();
        isTransitioning = false;
        yield return null;
    }

    IEnumerator TypingText(string text, int index, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
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
            if (dialogTextsIndex < currentDialog.dialogText.Count)
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
        image.gameObject.SetActive(false);
        OnClose.Invoke();
        isTransitioning = false;
    }
}
