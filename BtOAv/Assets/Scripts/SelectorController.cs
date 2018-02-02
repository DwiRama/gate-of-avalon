using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorController : MonoBehaviour {
    public Transform selectorHome;

    public SelectorPosition selectorPos;
    
    public SpriteRenderer currSpRen;
    public SpriteRenderer currSpRenBack;
    public int currHighLightIndex = 0;
    
    public AudioSource selectorAudioSource;
    public Animator selectorAnimator;
    
    public void SetSelector(Transform target)
    {
        this.gameObject.SetActive(true);
        this.transform.SetParent(target);
        this.transform.localPosition = Vector3.zero;

        selectorAnimator.Play("Fade In", -1, 0f);
    }

    public void SetSelector(Transform target,SelectorPosition selectorPos)
    {
        this.gameObject.SetActive(true);
        this.transform.SetParent(target);
        this.transform.localPosition = Vector3.zero;
        this.selectorPos = selectorPos;
        selectorAnimator.Play("Fade In", -1, 0f);
    }

    public void HighLight(SpriteRenderer spRen, SpriteRenderer spRenBack,int highLightIndex, bool audioOn)
    {
        spRen.sortingLayerName = "Selector";
        spRen.sortingOrder = 0;
        spRenBack.sortingLayerName = "Selector";
        spRenBack.sortingOrder = 0;

        this.currSpRen = spRen;
        this.currSpRenBack = spRenBack;
        this.currHighLightIndex = highLightIndex;

        if (audioOn)
        {
            selectorAudioSource.Play();
        }
    }

    public void UnHighLight()
    {
        if (currSpRen != null)
        {
            currSpRen.sortingLayerName = "Card";
            currSpRen.sortingOrder = currHighLightIndex;

            currSpRenBack.sortingLayerName = "Card";
            currSpRenBack.sortingOrder = currHighLightIndex;
        }
    }

    public void RemoveSelector()
    {
        SetSelector(selectorHome);
        this.gameObject.SetActive(false);
    }

    public void Next(Hand hand)
    {
        if (hand.cards.Count > 0)
        {
            if (hand.selectedIndex < hand.cards.Count - 1)
            {
                hand.selectedIndex++;
            }
            else
            {
                hand.selectedIndex = 0;
            }

            UnHighLight();

            SpriteRenderer currSpRen = hand.cardGOs[hand.selectedIndex].GetComponent<CardController>().cardGFX;
            SpriteRenderer currSpRenBack = hand.cardGOs[hand.selectedIndex].GetComponent<CardController>().cardGFXBack;

            SetSelector(hand.cardPos[hand.selectedIndex]);
            HighLight(currSpRen, currSpRenBack,currSpRen.sortingOrder, true);
            //Debug.Log("Select Card index: " + hand.selectedIndex);
        }
        else
        {
            GameMaster.gm.selector.RemoveSelector();
        }
    }

    public void Prev(Hand hand)
    {
        if (hand.cards.Count > 0)
        {
            if (hand.selectedIndex > 0)
            {
                hand.selectedIndex--;
            }
            else
            {
                hand.selectedIndex = hand.cards.Count - 1;
            }

            UnHighLight();

            SpriteRenderer currSpRen = hand.cardGOs[hand.selectedIndex].GetComponent<CardController>().cardGFX;
            SpriteRenderer currSpRenBack = hand.cardGOs[hand.selectedIndex].GetComponent<CardController>().cardGFXBack;

            SetSelector(hand.cardPos[hand.selectedIndex]);
            HighLight(currSpRen, currSpRenBack, currSpRen.sortingOrder, true);
            //Debug.Log("Select Card index: " + hand.selectedIndex);
        }
        else
        {
            GameMaster.gm.selector.RemoveSelector();
        }
    }
}

public enum SelectorPosition{
    Deck,
    Hand,
    Enemy
}