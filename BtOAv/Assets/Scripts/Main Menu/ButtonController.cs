using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour {


	public void MoveToScene(string sceneName)
    {
        //SceneManager.LoadScene(sceneName);

        if (gameObject.GetComponent<AudioSource>() == null)
        {
            Debug.Log("Audio doesn't exist");
        }
    }
}
