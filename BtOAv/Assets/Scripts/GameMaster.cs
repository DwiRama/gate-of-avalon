using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {
    public static GameMaster gm;    

    [Header("Player")]
    public Deck deck;
    public Hand hand;
    public SelectorController selector;
    public BoardDropzoneController board;
    [Space(5)]

    [Header("Opponent")]
    public Deck deckOppo;
    public Hand handOppo;
    public SelectorController selectorOppo;
    public BoardDropzoneController boardOppo;
    [Space(5)]

    public GameObject cardPrefab;
    
    public float actionDelay = 0.5f;
    public bool canMove = true;
    
    float timer;
    bool drawn10 = false;

    void Awake()
    {
        gm = this;
    }

	void Start () {
        timer = actionDelay;
        deck.Shuffle();
        deckOppo.Shuffle();
    }

    void Update()
    {
        if (!canMove)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                canMove = true;
            }
        }

        if (!board.changePoint && canMove)
        {
            if (Input.GetKeyDown(KeyCode.Z) && !drawn10)
            {
                drawn10 = true;

                hand.Draw10();
                handOppo.Draw10();

                hand.OpenCX();
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                if (hand.cardsOpen)
                {
                    hand.CloseCX();
                } else
                {
                    hand.OpenCX();
                }
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                hand.SortCards();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                hand.ShuffleHand();
            }

            if (selector.selectorPos == SelectorPosition.Deck)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    deck.PlaceCard();
                    deckOppo.PlaceCard();
                    if (hand.cards.Count > 0)
                    {
                        hand.selectedIndex = 0;
                        SpriteRenderer currSpRen = hand.cardGOs[hand.selectedIndex].GetComponent<CardController>().cardGFX;
                        SpriteRenderer currSpRenBack = hand.cardGOs[hand.selectedIndex].GetComponent<CardController>().cardGFXBack;
                        selector.HighLight(currSpRen, currSpRenBack, currSpRen.sortingOrder, false);
                        selector.SetSelector(hand.cardPos[0], SelectorPosition.Hand);
                    }
                }
            }
            else if (selector.selectorPos == SelectorPosition.Hand)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    hand.PlaceCard();
                }

                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    selector.Next();
                }

                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    selector.Prev();
                }
            }

            if (Input.GetKeyDown(KeyCode.M) && selector.selectorPos != SelectorPosition.Hand)
            {
                if (hand.cards.Count > 0)
                {
                    hand.selectedIndex = 0;
                    SpriteRenderer currSpRen = hand.cardGOs[hand.selectedIndex].GetComponent<CardController>().cardGFX;
                    SpriteRenderer currSpRenBack = hand.cardGOs[hand.selectedIndex].GetComponent<CardController>().cardGFXBack;
                    selector.HighLight(currSpRen, currSpRenBack, currSpRen.sortingOrder, false);
                    selector.SetSelector(hand.cardPos[0], SelectorPosition.Hand);
                }
            }

            if (Input.GetKeyDown(KeyCode.B) && selector.selectorPos != SelectorPosition.Deck)
            {
                if (hand.cards.Count > 0)
                {
                    selector.UnHighLight();
                }
                selector.SetSelector(selector.selectorHome, SelectorPosition.Deck);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                board.AddCardToBin(board.cardGos.Count - 1);
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                board.ClearBoard();
                boardOppo.ClearBoard();
            }
        }
    }

    public void ActionOn()
    {
        canMove = false;
        timer = actionDelay;
    }

    public void CreateCards(Card cardInfo, Transform startPos, List<GameObject> cardList, Transform cardPos, int sortOrder, bool sortAscend = true, int maxOrder = 0)
    {
        GameObject newCard = Instantiate(cardPrefab, startPos);
        newCard.transform.SetParent(cardPos);

        CardController newCardController = newCard.GetComponent<CardController>();
        newCardController.enabled = true;

        //newCard.transform.localPosition = Vector3.zero;
        newCardController.targetPos = Vector3.zero;
        newCardController.moving = true;
        if (sortAscend)
        {
            newCardController.cardGFX.sortingOrder = sortOrder;
            newCardController.cardGFXBack.sortingOrder = sortOrder;
        } else
        {
            newCardController.cardGFX.sortingOrder = maxOrder - sortOrder;
            newCardController.cardGFXBack.sortingOrder = maxOrder - sortOrder;
        }
        cardList.Add(newCard);

        RenderCards(cardInfo, newCard);
    }

    public void RenderCards(Card cardInfo,GameObject cardGO)
    {
        CardController newCardController = cardGO.GetComponent<CardController>();

        newCardController.cardId = cardInfo.cardId;
        newCardController.cardValue = cardInfo.cardValue;
        newCardController.cardType = cardInfo.cardType;
        newCardController.cardSp = cardInfo.cardSp;
        newCardController.cardGFX.sprite = cardInfo.cardSp;
    }
}
