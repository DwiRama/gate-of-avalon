using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour {
    public CardManager CM;
    public List<int> cardIds;

    public BoardDropzoneController dropZone;
    
    public SpriteRenderer deckSpRen;
    public Sprite fullDeckSp;
    public Sprite lastCardSp;

    public Card Draw()
    {
        if (cardIds.Count > 0)
        {
            Card drawnCard = CM.getCard(cardIds[cardIds.Count - 1]);
            cardIds.RemoveAt(cardIds.Count - 1);
            return drawnCard;
        } else
        {
            return null;
        }
    }

    public void Shuffle()
    {
        List<int> result = new List<int>();
        while (cardIds.Count > 0)
        {
            int index = Random.Range(0, cardIds.Count);
            result.Add(cardIds[index]);
            cardIds.RemoveAt(index);
        }
        cardIds = result;
    }

    public void PlaceCard()
    {
        if (cardIds.Count > 0)
        {
            GameMaster.gm.CreateCards(Draw(), this.transform, dropZone.cardGos, this.transform, 1);

            dropZone.cardGos[dropZone.cardGos.Count - 1].GetComponent<CardController>().enabled = true;
            dropZone.cardGos[dropZone.cardGos.Count - 1].GetComponent<CardController>().FaceUp();

            dropZone.PlaceCard(dropZone.cardGos.Count - 1);
            
            if (cardIds.Count == 1)
            {
                deckSpRen.sprite = lastCardSp;
            }

            if (cardIds.Count <= 0)
            {
                deckSpRen.enabled = false;
            }
        }
        else
        {
            Debug.Log("No more cards in Deck.");
        }
    }
}
