using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBin : MonoBehaviour {
    public List<GameObject> cardGos;
    public List<Card> cards;
    
    public void AddToBin(int index)
    {
        cardGos[index].transform.SetParent(this.transform);
        //cardGos[index].transform.localPosition = Vector3.zero;

        CardController cardController = cardGos[index].GetComponent<CardController>();
        cardController.enabled = true;

        cardController.targetPos = Vector3.zero;
        cardController.moving = true;

        cardController.FaceUp();

        cardController.cardGFX.sortingLayerName = "Card Board";
        cardController.cardGFXBack.sortingLayerName = "Card Board";
        cardController.cardGFX.sortingOrder = cards.Count - 1;
        cardController.cardGFXBack.sortingOrder = cards.Count - 1;
    }
}
