using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardsManager : MonoBehaviour
{
    private int len, posX, posZ;

    public int currentPlayerID;

    public GameState gameState;

    public GameObject card;
    public Card[] choiceOfCards;
    public Stack<Card> cardStack;
    public GameObject[,] currentCards;
    public GameObject currentCard;
    public Transform mainCamera;

    /// <summary>
    /// The starting method.
    /// </summary>
    private void Start()
    {
        List<Card> allCards = new List<Card>();

        foreach (Card card in choiceOfCards)
        {
            for (int i = 0; i < 3; i++)
            {
                allCards.Add(card);
            }
        }

        this.cardStack = new Stack<Card>(allCards.OrderBy(x => Random.value));

        this.currentCards = new GameObject[this.cardStack.Count, this.cardStack.Count];
        this.len = this.cardStack.Count / 2;
        this.posX = this.len;
        this.posZ = this.len;
        this.currentCards[this.posX, this.posZ] = this.currentCard;
    }

    private void FixedUpdate()
    {
        if (this.gameState == GameState.Adding && this.currentCard.GetComponent<CardManager>().placed)
        {
            this.gameState = GameState.Moving;
        }
        else if (this.gameState == GameState.Moving)
        {
            if (Vector3.Distance(mainCamera.position, new Vector3(this.currentCard.transform.position.x, 3, this.currentCard.transform.position.z - 2)) > 0.01)
            {
                mainCamera.position = Vector3.Lerp(mainCamera.position, new Vector3(this.currentCard.transform.position.x, 3, this.currentCard.transform.position.z - 2), Time.deltaTime * 3);
            }
            else if (Vector3.Distance(transform.position, new Vector3(this.currentCard.transform.position.x, 3, this.currentCard.transform.position.z - 2)) != 0)
            {
                mainCamera.position = new Vector3(this.currentCard.transform.position.x, 3, this.currentCard.transform.position.z - 2);
                this.gameState = GameState.Event;
            }
        }
    }

    public void SelectCard(int x, int z)
    {
        if (this.currentCards[x, z] != null)
        {
            this.currentCard = this.currentCards[x, z];
            this.posX = x;
            this.posZ = z;
        }
    }

    public IEnumerator FlipCard((int, int) pos, CardManager card)
    {
        this.gameState = GameState.Checking;
        card.flip = false;
        yield return new WaitForSeconds(2);
        this.gameState = GameState.Moving;
    }

    public void MoveToCard(Direction direction)
    {
        if (this.gameState != GameState.Default ||
            !this.currentCard.GetComponent<CardManager>().CanMove(direction)) return;

        (int, int) newPos = this.ApplyDirection(this.posX, this.posZ, direction);

        Direction oppositeDirection = this.GetOppositeDirection(direction);

        if (this.currentCards[newPos.Item1, newPos.Item2] != null
            && this.currentCards[newPos.Item1, newPos.Item2].GetComponent<CardManager>().CanMove(oppositeDirection))
        {
            currentCard = this.currentCards[newPos.Item1, newPos.Item2];
            this.posX = newPos.Item1;
            this.posZ = newPos.Item2;
            if (this.currentCards[newPos.Item1, newPos.Item2].GetComponent<CardManager>().flip)
            {
                StartCoroutine(this.FlipCard(newPos, this.currentCards[newPos.Item1, newPos.Item2].GetComponent<CardManager>()));
            }
            else
            {
                this.gameState = GameState.Moving;
            }
        }
    }

    public GameObject CreateCard(Direction direction)
    {
        if (this.gameState != GameState.Default) return null;

        if (!this.currentCard.GetComponent<CardManager>().CanMove(direction)) return null;

        (int, int) newPos = this.ApplyDirection(this.posX, this.posZ, direction);

        if (this.currentCards[newPos.Item1, newPos.Item2] != null) return this.currentCards[newPos.Item1, newPos.Item2];

        this.gameState = GameState.Adding;

        Card newCard = this.cardStack.Pop();
        GameObject newCardObject = Instantiate(this.card, this.GridToVector3(newPos.Item1, newPos.Item2), Quaternion.identity);
        newCardObject.GetComponent<CardManager>().SetCard(newCard);

        this.currentCards[newPos.Item1, newPos.Item2] = newCardObject;

        Direction oppositeDirection = this.GetOppositeDirection(direction);

        if (!newCardObject.GetComponent<CardManager>().CanMove(oppositeDirection))
        {
            newCardObject.GetComponent<CardManager>().flip = true;
            return null;
        }

        this.posX = newPos.Item1;
        this.posZ = newPos.Item2;
        this.currentCard = newCardObject;

        return newCardObject;
    }

    private (int, int) ApplyDirection(int x, int z, Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                x -= 1;
                break;
            case Direction.Right:
                x += 1;
                break;
            case Direction.Top:
                z += 1;
                break;
            case Direction.Bottom:
                z -= 1;
                break;
        }

        return (x, z);
    }

    private Vector3 GridToVector3(int x, int z)
    {
        return new Vector3((x - this.len) * 1.5f, 10, z - this.len);
    }

    private Direction GetOppositeDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                return Direction.Right;
            case Direction.Right:
                return Direction.Left;
            case Direction.Top:
                return Direction.Bottom;
            case Direction.Bottom:
                return Direction.Top;
            default:
                return 0;
        }
    }
}

public enum GameState
{
    Adding, Moving, Default, Checking, Event
}