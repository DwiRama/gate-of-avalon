using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardDropzoneController : MonoBehaviour {
    public List<GameObject> cardGos;

    public CardBin bin;

    public int totalPoint = 0;
    public bool increasePoint = true;
    public Text totalPointText;
    
    public bool changePoint = false;
    public float changeDelay = 0.05f;
    public int currTotalPoint = 0;

    float changePointTimer;

    public void PlaceCard(int index)
    {
        CardController placedCard = cardGos[index].GetComponent<CardController>();
        placedCard.enabled = true;

        placedCard.FaceUp();
        placedCard.PlaySFX();

        cardGos[index].transform.SetParent(this.transform);
        //cards[index].transform.localPosition = new Vector3(index * 0.6f, 0, 0);
        placedCard.targetPos = new Vector3(index * 0.6f, 0, 0);
        placedCard.moving = true;

        placedCard.cardGFX.sortingLayerName = "Card Board";
        placedCard.cardGFXBack.sortingLayerName = "Card Board";
        placedCard.cardGFX.sortingOrder = cardGos.Count - 1;
        placedCard.cardGFXBack.sortingOrder = cardGos.Count - 1;

        AddCardValue(placedCard.cardValue);
    }

    public void PlaceCard(int index, int multipleValue)
    {
        CardController placedCard = cardGos[index].GetComponent<CardController>();
        placedCard.enabled = true;
        placedCard.FaceUp();

        cardGos[index].transform.SetParent(this.transform);
        //cards[index].transform.localPosition = new Vector3(index * 0.6f, 0, 0);
        placedCard.targetPos = new Vector3(index * 0.6f, 0, 0);
        placedCard.moving = true;

        placedCard.cardGFX.sortingLayerName = "Card Board";
        placedCard.cardGFXBack.sortingLayerName = "Card Board";
        placedCard.cardGFX.sortingOrder = cardGos.Count - 1;
        placedCard.cardGFXBack.sortingOrder = cardGos.Count - 1;

        MultipleCardValue(multipleValue);
    }

    public void AddCardValue(int value)
    {
        increasePoint = true;
        totalPoint += value;
        ChangePoint();
        //Debug.Log("Total Value: " + totalPoint);
    }

    public void SubtractCardValue(int value)
    {
        increasePoint = false;
        totalPoint -= value;
        ChangePoint();
        //Debug.Log("Total Value: " + totalPoint);
    }

    public void MultipleCardValue(int value)
    {
        totalPoint *= value;
        ChangePoint();
        //Debug.Log("Total Value: " + totalPoint);
    }

    public void ChangePoint()
    {
        changePointTimer = changeDelay;
        changePoint = true;
    }

    public void AddCardToBin(int index)
    {
        if (cardGos.Count > 0)
        {
            CardController card = cardGos[index].GetComponent<CardController>();

            Card cardInfo = new Card();
            cardInfo.cardId = card.cardId;
            cardInfo.cardSp = card.cardSp;
            cardInfo.cardType = card.cardType;
            cardInfo.cardValue = card.cardValue;

            bin.cards.Add(cardInfo);
            bin.cardGos.Add(cardGos[index]);

            cardGos.RemoveAt(index);

            SubtractCardValue(cardInfo.cardValue);
            bin.AddToBin(bin.cards.Count - 1);
        }
    }

    public void ClearBoard()
    {
        for (int i = cardGos.Count - 1; i >= 0; i--)
        {
            AddCardToBin(i);
            totalPoint = 0;
            currTotalPoint = 0;
            totalPointText.text = "0";
        }
    }

    void Update()
    {
        if (changePoint)
        {
            if (changePointTimer <= 0)
            {
                if (increasePoint)
                {
                    if (currTotalPoint < totalPoint)
                    {
                        currTotalPoint++;
                        totalPointText.text = currTotalPoint + "";
                        changePointTimer = changeDelay;
                    }
                    else
                    {
                        changePoint = false;
                    }
                } else
                {
                    if (currTotalPoint > totalPoint)
                    {
                        currTotalPoint--;
                        totalPointText.text = currTotalPoint + "";
                        changePointTimer = changeDelay;
                    }
                    else
                    {
                        changePoint = false;
                    }
                }
            } else
            {
                changePointTimer -= Time.deltaTime;
            }
        }
    }
}
