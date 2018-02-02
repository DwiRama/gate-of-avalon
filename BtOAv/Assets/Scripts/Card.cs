using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card {
    public int cardId = 0;
    public int cardValue = 1;
    public CardType cardType;
    public Sprite cardSp;
}

public enum CardType
{
    normal,
    bolt,
    blast,
    mirror,
    force
}
