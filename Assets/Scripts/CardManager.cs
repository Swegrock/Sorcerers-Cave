using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    private Card _card;

    private Vector3 _position, _rotation;

    public GameObject mainCover;

    public Card card;

    public bool flip;

    public bool placed;

    private void Start()
    {
        this._position = new Vector3(transform.position.x, 0, transform.position.z);
        this._rotation = transform.eulerAngles;
    }

    private void FixedUpdate()
    {
        if (placed)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(this._rotation.x + (this.flip ? 1 : 0) * 180, this._rotation.y, this._rotation.z)), Time.deltaTime * 2.5f);
        }
        else
        {
            if (transform.position.y > 0.01)
            {
                transform.position = Vector3.Lerp(transform.position, this._position, Time.deltaTime * 4);
            }
            else if (transform.position.y > 0)
            {
                transform.position = this._position;
                this.placed = true;
            }
        }
    }

    public void SetCard(Card card)
    {
        this.card = card;
        this.mainCover.GetComponent<Renderer>().material.mainTexture = card.image;
    }

    public bool CanMove(Direction direction)
    {
        return this.card.directions.Contains(direction);
    }
}
