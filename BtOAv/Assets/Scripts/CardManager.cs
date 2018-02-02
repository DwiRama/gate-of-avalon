using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour {
    public List<Card> cardData;

    public Card getCard(int id)
    {
        for (int i = 0; i < cardData.Count; i++)
        {
            if (id == cardData[i].cardId)
            {
                return cardData[i];
            }
        }
        return null;
    }
}
