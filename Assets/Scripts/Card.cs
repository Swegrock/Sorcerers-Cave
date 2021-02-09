using System.Text.RegularExpressions;
using UnityEngine;

[CreateAssetMenu]
public class Card : ScriptableObject
{
    public CardType cardType;

    public string cardName
    {
        get
        {
            return Regex.Replace(this.cardType.ToString(), "(\\B[A-Z])", " $1");
        }
    }

    public Direction[] directions;

    public Texture2D image;
}

public enum CardType
{
    Corridor, Event, CorridorWithStairs, Gateway
}

public enum Direction
{
    Left, Top, Bottom, Right
}