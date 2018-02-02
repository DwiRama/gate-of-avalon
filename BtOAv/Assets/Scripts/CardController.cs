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
    public float speed = 30;
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

    public void PlaySFX()
    {
        sfx.Play();
    }

    void Update()
    {
        if (moving)
        {
            step = speed * Time.deltaTime;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPos, step);

            if (transform.localPosition == targetPos)
            {
                moving = false;
                this.enabled = false;
            }
        }
    }
}
