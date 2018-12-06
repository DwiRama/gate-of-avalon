using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour {

    public string sceneName;
    public float zoomButtonSize;
    public AudioSource slideAudio;

    private Vector3 originalButtonSize;

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
    }

    public void zoomOutButton()
    {
        transform.localScale = originalButtonSize;
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
}
