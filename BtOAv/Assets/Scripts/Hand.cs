﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {
    public Deck deck;
    public BoardDropzoneController dropZone;
    public CardBin cardBin;

    public Hand handOppo;
    public BoardDropzoneController dropZoneOppo;

    public List<Card> cards;

    public List<GameObject> cardGOs;
    public Transform[] cardPos;

    public AudioSource drawAudioSource;

    public bool sortAscend = true;
    public int selectedIndex = 0;

    public int cardOpenIndex = 0;
    public bool cardsOpen = false;
          
    public void Draw10()
    {
        for (int i = 0; i < 10; i++)
        {
            cards.Add(deck.Draw());
        }

        StartCoroutine(DrawingCard(0, 0.3f));
    }

    IEnumerator DrawingCard(int index, float wait)
    {
        GameMaster.gm.isDrawing = true;
        yield return new WaitForSeconds(wait);
        if (index < cards.Count)
        {
            drawAudioSource.Play();
            GameMaster.gm.CreateCards(cards[index], this.deck.transform, cardGOs, cardPos[index], index, sortAscend, cards.Count - 1);
            StartCoroutine(DrawingCard(index+1, wait));
        }
        else
        {
            GameMaster.gm.isDrawing = false;
            GameMaster.gm.sort = true;
            yield return null;
        }
    }

    public void OpenCX()
    {
        cardsOpen = true;
        for (int i = 0; i < cardGOs.Count; i++)
        {
            OpenCard(cardGOs[i].GetComponent<CardController>());
        }
    }

    public void OpenCard(CardController cardController)
    {
        cardController.enabled = true;
        cardController.FaceUp();
        if (cardOpenIndex >= cards.Count)
        {
            cardOpenIndex = 0;
        }
        cardOpenIndex++;
    }

    public void CloseCX()
    {
        cardsOpen = false;
        for (int i = 0; i < cardGOs.Count; i++)
        {
            CloseCard(cardGOs[i].GetComponent<CardController>());
        }
    }

    public void CloseCard(CardController cardController)
    {
        cardController.enabled = true;
        cardController.FaceDown();
        if (cardOpenIndex >= cards.Count)
        {
            cardOpenIndex = 0;
        }
        cardOpenIndex++;
    }

    public void PlaceCard(bool backToDeck = false)
    {
        if (cards.Count > 0)
        {
            GameMaster.gm.selector.UnHighLight();

            bool revive = false;
            if (GameMaster.gm.currBolted == dropZone)
            {
                if (cards[selectedIndex].cardId == 0)
                {
                    revive = true;
                    GameMaster.gm.currBolted = null;
                }
                else if (cards[selectedIndex].cardType == CardType.blast)
                {
                    //Do nothing
                }
                else
                {
                    dropZone.AddCardToBin(dropZone.cardGos.Count - 1, false);
                    dropZone.currBoltCard = null;
                    GameMaster.gm.currBolted = null;
                }
            }

            if (revive)
            {
                StartCoroutine(ShowCardEffect(1f, true, CardType.revive));
            }
            else if (cards[selectedIndex].cardType == CardType.normal || cards[selectedIndex].cardId == 0)
            {
                //Debug.Log("Put");
                dropZone.cardGos.Add(cardGOs[selectedIndex]);

                cards.RemoveAt(selectedIndex);
                cardGOs.RemoveAt(selectedIndex);
                
                dropZone.PlaceCard(dropZone.cardGos.Count - 1);
                
                if (selectedIndex >= cards.Count - 1)
                {
                    selectedIndex = cards.Count - 1;
                }

                RearrangeCard();
            }
            else if (cards[selectedIndex].cardType == CardType.force)
            {
                //Debug.Log("Put");
                dropZone.cardGos.Add(cardGOs[selectedIndex]);

                cards.RemoveAt(selectedIndex);
                cardGOs.RemoveAt(selectedIndex);

                dropZone.PlaceCard(dropZone.cardGos.Count - 1, 2);

                if (selectedIndex >= cards.Count - 1)
                {
                    selectedIndex = cards.Count - 1;
                }

                RearrangeCard();
            }
            else if (cards[selectedIndex].cardType == CardType.mirror)
            {
                StartCoroutine(ShowCardEffect(1f, true, CardType.mirror));
            }
            else if (cards[selectedIndex].cardType == CardType.bolt)
            {
                StartCoroutine(ShowCardEffect(1f, true, CardType.bolt));
            }
            else if (cards[selectedIndex].cardType == CardType.blast)
            {
                StartCoroutine(ShowCardEffect(1f, true, CardType.blast));
            }
            
            if (!backToDeck)
            {
                if (cards.Count > 0)
                {
                    SpriteRenderer cardGFX = cardGOs[selectedIndex].GetComponent<CardController>().cardGFX;
                    SpriteRenderer cardGFXBack = cardGOs[selectedIndex].GetComponent<CardController>().cardGFXBack;
                    GameMaster.gm.selector.SetSelector(cardPos[selectedIndex]);
                    GameMaster.gm.selector.HighLight(cardGFX, cardGFXBack, selectedIndex, false);
                }
                else
                {
                    GameMaster.gm.selector.RemoveSelector();
                }
            }
        } else
        {
            Debug.Log("No more cards in Hand.");
        }
    }
    
    IEnumerator ShowCardEffect(float wait, bool throwToBin, CardType cardType)
    {
        GameMaster.gm.isShowingEffect = true;
        CardController cc = cardGOs[selectedIndex].GetComponent<CardController>();

        yield return new WaitForSeconds(0.05f);

        cc.cardGFX.sortingLayerName = "Show";
        cc.cardGFX.sortingOrder = 1;
        cc.ShowCard();

        yield return new WaitForSeconds(0.35f);

        CardController placedCard = new CardController();

        switch (cardType)
        {
            case CardType.revive:
                //Showing Effect...//
                Debug.Log("Show Effects");

                yield return new WaitForSeconds(wait);

                placedCard = dropZone.cardGos[dropZone.cardGos.Count - 1].GetComponent<CardController>();

                placedCard.FaceUp();
                dropZone.currBoltCard = placedCard;

                dropZone.AddCardValue(dropZone.lastAddValue);
                break;
            case CardType.bolt:

                //Showing Effect...//
                Debug.Log("Show Effects");

                yield return new WaitForSeconds(wait);

                placedCard = dropZoneOppo.cardGos[dropZoneOppo.cardGos.Count - 1].GetComponent<CardController>();

                placedCard.FaceDown();
                dropZoneOppo.currBoltCard = placedCard;
                GameMaster.gm.currBolted = dropZoneOppo;
                
                dropZoneOppo.SubtractCardValue(dropZoneOppo.lastAddValue);
                break;
            case CardType.blast:
                //Showing Effect...//
                Debug.Log("Show Effects");

                yield return new WaitForSeconds(wait);

                if (handOppo.cards.Count > 0)
                {
                    handOppo.selectedIndex = 0;
                    SpriteRenderer currSpRen = handOppo.cardGOs[handOppo.selectedIndex].GetComponent<CardController>().cardGFX;
                    SpriteRenderer currSpRenBack = handOppo.cardGOs[handOppo.selectedIndex].GetComponent<CardController>().cardGFXBack;
                    if (!GameMaster.gm.blastOppo)
                    {                        
                        GameMaster.gm.selector.SetSelector(handOppo.cardPos[0], SelectorPosition.Enemy);
                        GameMaster.gm.selector.HighLight(currSpRen, currSpRenBack, currSpRen.sortingOrder, false);
                    } 
                }
                break;
            case CardType.mirror:

                //Showing Effect...//
                Debug.Log("Show Effects");

                yield return new WaitForSeconds(wait);

                //Swap cards on board
                List<GameObject> tempCardGos = new List<GameObject>();
                tempCardGos = dropZone.cardGos;
                dropZone.cardGos = dropZoneOppo.cardGos;
                dropZoneOppo.cardGos = tempCardGos;

                //Set card's parent
                dropZone.RearrangeCards(0.1f);
                dropZoneOppo.RearrangeCards(0.1f);

                //Change Total Point
                int tempPoint = dropZone.totalPoint;
                dropZone.totalPoint = dropZoneOppo.totalPoint;
                dropZoneOppo.totalPoint = tempPoint;

                //Change totalPoint UIs
                dropZone.ChangePoint(true);
                dropZoneOppo.ChangePoint(true);
                break;
        }
        
        yield return new WaitForSeconds(1.2f);

        if (throwToBin)
        {            
            ThrowToBin(false,true);
        }
        
        if (selectedIndex >= cards.Count - 1)
        {
            selectedIndex = cards.Count - 1;
        }

        RearrangeCard();

        GameMaster.gm.isShowingEffect = false;
        yield return null;
    }
    
    public void ThrowToBin(bool getThorwn = false, bool zeroOutPos = false, bool show = false, bool setSelector = false)
    {
        if (cards.Count > 0)
        {
            cardBin.cards.Add(cards[selectedIndex]);
            cardBin.cardGos.Add(cardGOs[selectedIndex]);

            cards.RemoveAt(selectedIndex);
            cardGOs.RemoveAt(selectedIndex);

            if (show)
            {
                StartCoroutine(ShowBeforeThrow(cardBin.cardGos[cardBin.cardGos.Count - 1].GetComponent<CardController>()));
            }
            else
            {
                if (zeroOutPos)
                {
                    cardBin.AddToBin(cardBin.cards.Count - 1, true);
                }
                else
                {
                    cardBin.AddToBin(cardBin.cards.Count - 1);
                }
            }

            if (getThorwn)
            {
                //GameMaster.gm.selector.UnHighLight();
                GameMaster.gm.selector.RemoveSelector();

                //if (selectedIndex >= cards.Count - 1)
                //{
                //    selectedIndex = cards.Count - 1;
                //}
                RearrangeCard();                
            }

            if (setSelector)
            {
                handOppo.selectedIndex = 0;
                SpriteRenderer currSpRen = handOppo.cardGOs[handOppo.selectedIndex].GetComponent<CardController>().cardGFX;
                SpriteRenderer currSpRenBack = handOppo.cardGOs[handOppo.selectedIndex].GetComponent<CardController>().cardGFXBack;
                GameMaster.gm.selector.SetSelector(handOppo.cardPos[0], SelectorPosition.Hand);
                GameMaster.gm.selector.HighLight(currSpRen, currSpRenBack, currSpRen.sortingOrder, false);
            }
        }
    }

    IEnumerator ShowBeforeThrow(CardController cc)
    {
        cc.cardGFX.sortingLayerName = "Show";
        cc.cardGFX.sortingOrder = 1;
        cc.cardGFXBack.sortingLayerName = "Show";
        cc.cardGFXBack.sortingOrder = 1;

        cc.ShowCard();

        yield return new WaitForSeconds(1.5f);

        cardBin.AddToBin(cardBin.cards.Count - 1, true);
        yield return null;
    }

    public void RearrangeCard()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            CardController cardController = cardGOs[i].GetComponent<CardController>();
            cardController.enabled = true;

            cardGOs[i].transform.SetParent(cardPos[i]);
            //cardGOs[i].transform.localPosition = Vector3.zero;

            cardController.targetPos = Vector3.zero;
            cardController.moving = true;
            if (sortAscend)
            {
                cardController.cardGFX.sortingOrder = i;
                cardController.cardGFXBack.sortingOrder = i;
            } else
            {
                cardController.cardGFX.sortingOrder = cards.Count - 1 - i;
                cardController.cardGFXBack.sortingOrder = cards.Count - 1 - i;
            }
        }

        for (int i = 0; i < cards.Count; i++)
        {
            GameMaster.gm.RenderCards(cards[i], cardGOs[i]);
        }
    }

    public void SortCards()
    {
        List<Card> result = new List<Card>();
        List<Card> unNormalCards = new List<Card>();
        
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].cardType != CardType.normal || cards[i].cardId != 0)
            {
                unNormalCards.Add(cards[i]);
            } else
            {
                result.Add(cards[i]);
            }
        }

        for (int i = 0; i < result.Count - 1; i++)
        {
            for (int j = i + 1; j > 0 ; j--)
            {
                if (result[j-1].cardValue > result[j].cardValue)
                {
                    Card temp = result[j - 1];
                    result[j - 1] = result[j];
                    result[j] = temp;
                }
            }
        }

        for (int i = 0; i < unNormalCards.Count - 1; i++)
        {
            for (int j = i + 1; j > 0; j--)
            {
                if (unNormalCards[j - 1].cardId > unNormalCards[j].cardId)
                {
                    Card temp = unNormalCards[j - 1];
                    unNormalCards[j - 1] = unNormalCards[j];
                    unNormalCards[j] = temp;
                }
            }
        }

        cards = new List<Card>(result.Count + unNormalCards.Count);
        cards.AddRange(result);
        cards.AddRange(unNormalCards);

        for (int i = 0; i < cards.Count; i++)
        {
            GameMaster.gm.RenderCards(cards[i], cardGOs[i]);
        }
    }

    public void ShuffleHand()
    {
        List<Card> result = cards;

        for (int i = 0; i < cards.Count; i++)
        {
            int index = Random.Range(0, cards.Count);
            result.Add(cards[index]);
            cards.RemoveAt(index);
        }

        cards = result;

        for (int i = 0; i < cards.Count; i++)
        {
            GameMaster.gm.RenderCards(cards[i], cardGOs[i]);
        }
    }

    #region Create & Render Card Method
    /*public void CreateCards()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, cardPos[i]);
            newCard.transform.localPosition = Vector3.zero;
            newCard.GetComponent<SpriteRenderer>().sortingOrder = i;
            cardGOs.Add(newCard);
        }

        RenderCards();
    }

    public void RenderCards()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            cardGOs[i].GetComponent<CardController>().cardValue = cards[i].cardValue;
            cardGOs[i].GetComponent<CardController>().cardType = cards[i].cardType;
            cardGOs[i].GetComponent<CardController>().cardSp = cards[i].cardSp;
            cardGOs[i].GetComponent<CardController>().cardGFX.sprite = cards[i].cardSp;
        }
    }*/
    #endregion

    #region Select Next Prev Card Method
    /*public void Next()
    {
        if (cards.Count > 0)
        {
            if (selectedIndex < cards.Count - 1)
            {
                selectedIndex++;
            }
            else
            {
                selectedIndex = 0;
            }

            GameMaster.gm.selector.UnHighLight();

            GameMaster.gm.selector.SetSelector(cardPos[selectedIndex]);
            GameMaster.gm.selector.HighLight(cardGOs[selectedIndex].GetComponent<SpriteRenderer>(), selectedIndex, true);
            Debug.Log("Select Card index: " + selectedIndex);
        }
        else
        {
            GameMaster.gm.selector.RemoveSelector();
        }
    }

    public void Prev()
    {
        if (cards.Count > 0)
        {
            if (selectedIndex > 0)
            {
                selectedIndex--;
            }
            else
            {
                selectedIndex = cards.Count - 1;
            }

            GameMaster.gm.selector.UnHighLight();

            GameMaster.gm.selector.SetSelector(cardPos[selectedIndex]);
            GameMaster.gm.selector.HighLight(cardGOs[selectedIndex].GetComponent<SpriteRenderer>(), selectedIndex, true);
            Debug.Log("Select Card index: " + selectedIndex);
        }
        else
        {
            GameMaster.gm.selector.RemoveSelector();
        }
    }*/
    #endregion
}
