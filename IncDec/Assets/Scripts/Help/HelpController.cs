using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpController : MonoBehaviour {

    public TextboxController textboxController;
    public SceneConnector sceneConnector;
    public Animator loadCanvas;

    public GameObject menu;
    public Animator helpMenuAnimator;
    
    public void ShowMenu()
    {
        menu.SetActive(true);
        helpMenuAnimator.SetBool("Hide", false);
    }

    public void HideMenu()
    {
        helpMenuAnimator.SetBool("Hide", true);
        StartCoroutine(HidingMenu());
    }

    IEnumerator HidingMenu()
    {
        yield return new WaitForSeconds(1);
        menu.SetActive(false);
    }

    public void ShowRules()
    {
        HideMenu();
        StartCoroutine(ShowingRules());
    }

    IEnumerator ShowingRules()
    {
        yield return new WaitForSeconds(1);
        textboxController.StartDialog(textboxController.dialogs[0]);
    }

    public void ShowCard()
    {
        HideMenu();
        StartCoroutine(ShowingCard());
    }

    IEnumerator ShowingCard()
    {
        yield return new WaitForSeconds(1);
        textboxController.StartDialog(textboxController.dialogs[1]);
    }

    public void ToMainMenu()
    {
        StartCoroutine(ShowingMainMenu());
    }

    IEnumerator ShowingMainMenu()
    {
        loadCanvas.SetBool("Fade", true);
        yield return new WaitForSeconds(1);
        sceneConnector.LoadScene("MainMenu");
    }
}
