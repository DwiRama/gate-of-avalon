using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour {

    public string sceneName;
    public float zoomButtonSize;
    public AudioSource slideAudio;

    private Vector3 originalButtonSize;
    private int originalSiblingPosition;

    public void MoveToScene()
    {

        if (gameObject.GetComponent<AudioSource>() != null)
        {
            gameObject.GetComponent<AudioSource>().Play();
            StartCoroutine(PlayAudio());
        }
    }

    public void zoomInButton()
    {
        originalButtonSize = transform.localScale;
        transform.localScale = new Vector3(zoomButtonSize, zoomButtonSize, zoomButtonSize);
        if(slideAudio != null)
        {
            slideAudio.Play();
        }

        buttonMoveToFront();
    }

    public void zoomOutButton()
    {
        transform.localScale = originalButtonSize;

        buttonBackToPosition();
    }


    public void ExitGame()
    {
        Application.Quit();
    }


    IEnumerator PlayAudio()
    {
        yield return new WaitForSeconds(gameObject.GetComponent<AudioSource>().clip.length);
        SceneManager.LoadScene(sceneName);
    }

    void buttonMoveToFront()
    {
        originalSiblingPosition = transform.GetSiblingIndex();
        transform.SetSiblingIndex(3);
    }

    void buttonBackToPosition()
    {
        transform.SetSiblingIndex(originalSiblingPosition);
    }
}
