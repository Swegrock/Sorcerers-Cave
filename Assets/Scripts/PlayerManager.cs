using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public int playerID;

    public GameObject controlsHUD;

    public CardsManager cardsManager;

    public Button leftButton, rightButton, upButton, downButton;

    // Start is called before the first frame update
    private void Start()
    {
        this.leftButton.onClick.AddListener(() => this.RequestNewCard(Direction.Left));
        this.rightButton.onClick.AddListener(() => this.RequestNewCard(Direction.Right));
        this.upButton.onClick.AddListener(() => this.RequestNewCard(Direction.Top));
        this.downButton.onClick.AddListener(() => this.RequestNewCard((Direction.Bottom)));
    }

    private void LateUpdate()
    {
        if ((cardsManager.currentPlayerID == playerID) != controlsHUD.activeSelf)
        {
            this.controlsHUD.SetActive(cardsManager.currentPlayerID == playerID);
        }

        if (cardsManager.gameState == GameState.Event)
        {
            CardType cardType = this.cardsManager.currentCard.GetComponent<CardManager>().card.cardType;
            switch (cardType)
            {
                case CardType.Corridor:
                    break;
                case CardType.Event:
                    break;
            }
            this.cardsManager.currentCard.GetComponent<CardManager>().card.cardType = CardType.Corridor;
            cardsManager.gameState = GameState.Default;
        }
    }

    private void RequestNewCard(Direction direction)
    {
        GameObject create = this.cardsManager.CreateCard(direction);
        if (create == null) return;
        if (cardsManager.currentCard != create)
        {
            this.cardsManager.MoveToCard(direction);
        }
        CardManager cardManager = cardsManager.currentCard.GetComponent<CardManager>();
        this.leftButton.gameObject.SetActive(cardManager.CanMove(Direction.Left));
        this.rightButton.gameObject.SetActive(cardManager.CanMove(Direction.Right));
        this.upButton.gameObject.SetActive(cardManager.CanMove(Direction.Top));
        this.downButton.gameObject.SetActive(cardManager.CanMove(Direction.Bottom));
    }
}

public enum PlayerState
{
    Default, Rolling
}