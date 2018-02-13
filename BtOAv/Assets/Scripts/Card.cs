using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject{
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
