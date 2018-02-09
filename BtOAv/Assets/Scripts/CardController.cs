using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour {
    public int cardId;
    public int cardValue;
    public CardType cardType;
    public Sprite cardSp;
    public SpriteRenderer cardGFX;
    public SpriteRenderer cardGFXBack;
    public Animator cardAnimator;
    public AudioSource sfx;

    public Vector3 targetPos;
    public float speed = 10;
    public bool moving = false;

    float step;

    public void FaceUp()
    {
        PlaySFX();
        cardAnimator.SetBool("Face Up", true);
    }

    public void FaceDown()
    {
        PlaySFX();
        cardAnimator.SetBool("Face Up", false);
    }    

    public void ShowCard()
    {
        cardAnimator.SetTrigger("Show");
        cardAnimator.SetBool("Face Up", true);
    }

    public void PlaySFX()
    {
        sfx.Play();
    }

    void Update()
    {
        if (moving)
        {
            //step = speed * Time.deltaTime;
            step = speed * Vector3.Distance(transform.localPosition, targetPos) / 1.5f;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPos, step * Time.deltaTime);

            if (transform.localPosition == targetPos)
            {
                moving = false;
                this.enabled = false;
            }
        }
    }
}
