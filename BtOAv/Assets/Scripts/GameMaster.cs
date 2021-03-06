﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour {
    public static GameMaster gm;
    public Turn gameTurn;

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
    public bool blastOppo = false;
    [Space(5)]

    [Header("Others")]
    public GameObject cardPrefab;
    public BoardDropzoneController currBolted = null;
    public bool rearrangeBoard = false;

    public Image message;
    public Sprite messageWin;
    public Sprite messageLose;
    public Animator messageAnimator;

    public bool firstFlip = false;
    public bool faceUp = true;
    public bool arrange = false;
    public bool sort = false;

    public bool isDrawing = false;
    public bool isFliping = false;
    public bool isArranging = false;
    public bool isShowingEffect = false;

    public bool checkPoints = false;
    public bool firstCheck = false;

    public bool turn = true;
    public float actionDelay = 1.2f;
    public bool canMove = true;

    public bool roundStart = true;
    public bool roundFinish = false;

    public float restartTime = 2;
    public bool restart = false;
    float restartTimer = 0;

    float timer;
    bool drawn10 = false;

    bool isCheckingPoint = false;

    public bool stop = false;

    void Awake()
    {
        gm = this;
    }

    void Start()
    {
        timer = actionDelay;
        ActionOn(0.5f);
        roundStart = true;
        //handOppo.SortCards();
    }

    public void SortHand()
    {
        hand.SortCards();
    }

    public void Shuffle()
    {
        hand.ShuffleHand();
    }

    public void FlipCard()
    {
        if (!firstFlip)
        {
            firstFlip = true;
        }

        if (faceUp)
        {
            hand.CloseCX();
        } else
        {
            hand.OpenCX();
        }

        faceUp = !faceUp;
        isFliping = true;
        arrange = true;
        ActionOn(1.5f);
    }

    void Update()
    {
        if (restart) {
            if (restartTimer <= 0) {
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            } else {
                restartTimer -= Time.deltaTime;
            }
            return;
        }
        if (!stop)
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
                    if (isFliping)
                    {
                        isFliping = false;
                    }
                }
            }

            if (canMove && roundStart)
            {
                Draw10();
                Invoke("FlipCard", 4);
                Invoke("FlipCard", 5);
                roundStart = false;
            }

            if (!firstFlip)
            {
                return;
            }

            if (arrange && !isFliping && !faceUp)
            {
                if (sort)
                {
                    SortHand();
                }
                else
                {
                    Shuffle();
                }
                FlipCard();
                arrange = false;
            }

            if (!isCheckingPoint && !isArranging && !isFliping && !isDrawing && !isShowingEffect && canMove)
            {
                if (!board.changePoint && !boardOppo.changePoint && checkPoints)
                {
                    isCheckingPoint = true;
                    CheckPoints(firstCheck);
                }
                else if (!board.changePoint && !boardOppo.changePoint && !checkPoints && !roundFinish && !rearrangeBoard)
                {
                    if (Input.GetKeyDown(KeyCode.Z) && !drawn10)
                    {
                        Draw10();
                        handOppo.SortCards();
                    }

                    if (turn)
                    {
                        if (selector.selectorPos == SelectorPosition.Deck)
                        {
                            if (Input.GetKeyDown(KeyCode.Return))
                            {
                                DrawFromDeckToBoard();
                                firstCheck = true;
                                checkPoints = true;
                            }
                        }
                        else if (selector.selectorPos == SelectorPosition.Hand)
                        {
                            if (Input.GetKeyDown(KeyCode.Return))
                            {
                                Card currC = hand.cards[hand.selectedIndex];
                                hand.PlaceCard();
                                if (hand.cards.Count > 0)
                                {
                                    selector.UnHighLight();
                                }
                                if (currC.cardType != CardType.blast)
                                {
                                    selector.SetSelector(selector.selectorHome, SelectorPosition.Deck);
                                    selector.RemoveSelector();
                                    firstCheck = false;
                                    checkPoints = true;
                                } else
                                {
                                    selector.RemoveSelector();
                                }
                                ActionOn(actionDelay);
                            }

                            if (Input.GetKeyDown(KeyCode.RightArrow))
                            {
                                selector.Next(hand);
                            }

                            if (Input.GetKeyDown(KeyCode.LeftArrow))
                            {
                                selector.Prev(hand);
                            }
                        }
                        else if (selector.selectorPos == SelectorPosition.Enemy)
                        {
                            if (Input.GetKeyDown(KeyCode.Return))
                            {
                                handOppo.ThrowToBin(true, true, true, true);
                            }

                            if (Input.GetKeyDown(KeyCode.RightArrow))
                            {
                                selector.Next(handOppo);
                            }

                            if (Input.GetKeyDown(KeyCode.LeftArrow))
                            {
                                selector.Prev(handOppo);
                            }
                        }

                        if (Input.GetKeyDown(KeyCode.M) && selector.selectorPos != SelectorPosition.Hand)
                        {
                            if (hand.cards.Count > 0)
                            {
                                hand.selectedIndex = 0;
                                SpriteRenderer currSpRen = hand.cardGOs[hand.selectedIndex].GetComponent<CardController>().cardGFX;
                                SpriteRenderer currSpRenBack = hand.cardGOs[hand.selectedIndex].GetComponent<CardController>().cardGFXBack;
                                selector.SetSelector(hand.cardPos[0], SelectorPosition.Hand);
                                selector.HighLight(currSpRen, currSpRenBack, currSpRen.sortingOrder, false);
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

                        if (Input.GetKeyDown(KeyCode.N) && selector.selectorPos != SelectorPosition.Enemy)
                        {
                            if (handOppo.cards.Count > 0)
                            {
                                handOppo.selectedIndex = 0;
                                SpriteRenderer currSpRen = handOppo.cardGOs[handOppo.selectedIndex].GetComponent<CardController>().cardGFX;
                                SpriteRenderer currSpRenBack = handOppo.cardGOs[handOppo.selectedIndex].GetComponent<CardController>().cardGFXBack;
                                selector.SetSelector(handOppo.cardPos[0], SelectorPosition.Enemy);
                                selector.HighLight(currSpRen, currSpRenBack, currSpRen.sortingOrder, false);
                            }
                        }
                    }
                    else //Opponent Turn
                    {
                        //Opponent Choice
                        bool found = false;
                        int pointInterval = boardOppo.totalPoint - board.totalPoint;

                        if (blastOppo) //Opponent Randomly choose player's card to discard
                        {
                            if (hand.cards.Count > 0)
                            {
                                hand.selectedIndex = Random.Range(0, hand.cards.Count);
                                hand.ThrowToBin(true, true, true);
                                Debug.Log("throw");
                            }
                            blastOppo = false;
                            ActionOn(actionDelay + 4);
                            return;
                        }

                        //Revive Card
                        if (currBolted == handOppo.dropZone)
                        {
                            Debug.Log("Revive check");
                            for (int i = 0; i < handOppo.cards.Count; i++)
                            {
                                if (handOppo.cards[i].cardType == CardType.revive)
                                {
                                    Debug.Log("Revive");
                                    handOppo.selectedIndex = i;
                                    found = true;
                                    break;
                                }
                            }
                        }

                        //Find Higher Point
                        if (!found)
                        {
                            for (int i = 0; i < handOppo.cards.Count; i++)
                            {
                                if (handOppo.cards[i].cardValue + pointInterval > 0 && (handOppo.cards[i].cardId >= 0 && handOppo.cards[i].cardId < 7))
                                {
                                    handOppo.selectedIndex = i;
                                    found = true;
                                    break;
                                }
                            }
                        }

                        //Find Same Point
                        if (!found)
                        {
                            for (int i = 0; i < handOppo.cards.Count; i++)
                            {
                                if (handOppo.cards[i].cardValue + pointInterval >= 0 && (handOppo.cards[i].cardId >= 0 && handOppo.cards[i].cardId < 7))
                                {
                                    handOppo.selectedIndex = i;
                                    found = true;
                                    break;
                                }
                            }
                        }

                        //Find Blast Card
                        if (!found && handOppo.cards.Count > 0 && handOppo.cards.Count < 3)
                        {
                            for (int i = 0; i < handOppo.cards.Count; i++)
                            {
                                if (handOppo.cards[i].cardType == CardType.blast)
                                {
                                    handOppo.selectedIndex = i;
                                    found = true;
                                    break;
                                }
                            }
                        }

                        //Find Force Card
                        if (!found)
                        {
                            if (boardOppo.totalPoint * 2 > board.totalPoint)
                            {
                                for (int i = 0; i < handOppo.cards.Count; i++)
                                {
                                    if (handOppo.cards[i].cardType == CardType.force)
                                    {
                                        handOppo.selectedIndex = i;
                                        found = true;
                                        break;
                                    }
                                }
                            }
                        }

                        //Find Bolt Card
                        if (!found)
                        {
                            for (int i = 0; i < handOppo.cards.Count; i++)
                            {
                                if (handOppo.cards[i].cardType == CardType.bolt)
                                {
                                    handOppo.selectedIndex = i;
                                    found = true;
                                    break;
                                }
                            }
                        }

                        //Find Mirror Card
                        if (!found)
                        {
                            for (int i = 0; i < handOppo.cards.Count; i++)
                            {
                                if (handOppo.cards[i].cardType == CardType.mirror)
                                {
                                    handOppo.selectedIndex = i;
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (found)
                        {
                            if (handOppo.cards[handOppo.selectedIndex].cardType == CardType.blast)
                            {
                                blastOppo = true;
                                handOppo.PlaceCard(true);
                            }
                            else
                            {
                                handOppo.PlaceCard(true);
                            }
                            ActionOn(actionDelay);
                        }
                        else
                        {
                            //Surrender
                            GameOver(true);
                        }

                        if (!blastOppo || handOppo.cards.Count < 0) // fix
                        {
                            firstCheck = false;
                            checkPoints = true;
                        }

                    }

                    if (Input.GetKeyDown(KeyCode.V))
                    {
                        if (hand.cardsOpen)
                        {
                            hand.CloseCX();
                        }
                        else
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

                    if (Input.GetKeyDown(KeyCode.P))
                    {
                        board.AddCardToBin(board.cardGos.Count - 1);
                    }

                    if (Input.GetKeyDown(KeyCode.Backspace))
                    {
                        ClearBoard();
                    }
                }
                else
                {

                }
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            stop = !stop;
        }
    }

    public void ActionOn(float delay)
    {
        canMove = false;
        timer = actionDelay;
    }

    public void Draw10()
    {
        deck.Shuffle();
        deckOppo.Shuffle();
        drawn10 = true;

        hand.Draw10();
        handOppo.Draw10();

        hand.OpenCX();
    }

    public void DrawFromDeckToBoard()
    {
        deck.PlaceCard();
        deckOppo.PlaceCard();

        selector.RemoveSelector();

        ActionOn(actionDelay);
    }

    public void CheckPoints(bool DrawFromDeck = false)
    {
        if (DrawFromDeck)
        {
            if (board.totalPoint == boardOppo.totalPoint)
            {
                ClearBoard();
            }
            else if (board.totalPoint < boardOppo.totalPoint)
            {
                gameTurn = Turn.Player;
                turn = true;

                if (hand.cards.Count > 0)
                {
                    hand.selectedIndex = 0;
                    SpriteRenderer currSpRen = hand.cardGOs[hand.selectedIndex].GetComponent<CardController>().cardGFX;
                    SpriteRenderer currSpRenBack = hand.cardGOs[hand.selectedIndex].GetComponent<CardController>().cardGFXBack;
                    selector.SetSelector(hand.cardPos[0], SelectorPosition.Hand);
                    selector.HighLight(currSpRen, currSpRenBack, currSpRen.sortingOrder, false);
                }
            }
            else
            {
                gameTurn = Turn.Opponent;
                turn = false;
            }
        } else
        {
            if (gameTurn == Turn.Player)
            {
                if (board.totalPoint == boardOppo.totalPoint)
                {
                    ClearBoard();
                    Debug.Log("Clear");
                }
                else if (board.totalPoint > boardOppo.totalPoint)
                {
                    turn = false;
                    gameTurn = Turn.Opponent;
                    //Debug.Log("Oppo Turn");
                }
                else
                {
                    GameOver(false);
                }
            } else
            {
                if (boardOppo.totalPoint == board.totalPoint)
                {
                    turn = true;
                    gameTurn = Turn.Player;
                    ClearBoard();
                    Debug.Log("Clear");
                }
                else if (boardOppo.totalPoint > board.totalPoint)
                {
                    turn = true;
                    gameTurn = Turn.Player;
                    if (hand.cards.Count > 0)
                    {
                        hand.selectedIndex = 0;
                        SpriteRenderer currSpRen = hand.cardGOs[hand.selectedIndex].GetComponent<CardController>().cardGFX;
                        SpriteRenderer currSpRenBack = hand.cardGOs[hand.selectedIndex].GetComponent<CardController>().cardGFXBack;
                        selector.SetSelector(hand.cardPos[0], SelectorPosition.Hand);
                        selector.HighLight(currSpRen, currSpRenBack, currSpRen.sortingOrder, false);
                    }
                    //Debug.Log("Player Turn");
                }
                else
                {
                    GameOver(true);
                }
            }
        }
        checkPoints = false;
        isCheckingPoint = false;
    }

    public void GameOver(bool win)
    {
        handOppo.OpenCX();
        roundFinish = true;

        if (win)
        {
            message.sprite = messageWin;
        }
        else
        {
            message.sprite = messageLose;
        }
        message.enabled = true;
        messageAnimator.enabled = true;
        messageAnimator.Play("Show", 0, -1f);
        restartTimer = restartTime;
        restart = true;
    }

    public void ClearBoard()
    {
        board.ClearBoard();
        boardOppo.ClearBoard();
        if (hand.cards.Count > 0)
        {
            selector.UnHighLight();
        }
        selector.SetSelector(selector.selectorHome, SelectorPosition.Deck);
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

    public void RenderCards(Card cardInfo, GameObject cardGO)
    {
        CardController newCardController = cardGO.GetComponent<CardController>();

        newCardController.cardId = cardInfo.cardId;
        newCardController.cardValue = cardInfo.cardValue;
        newCardController.cardType = cardInfo.cardType;
        newCardController.cardSp = cardInfo.cardSp;
        newCardController.cardGFX.sprite = cardInfo.cardSp;
    }
}


public enum Turn
{
    Player,
    Opponent
}