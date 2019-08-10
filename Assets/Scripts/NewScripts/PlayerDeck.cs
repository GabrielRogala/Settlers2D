using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck
{
    public DeckController _deckController;
    public PlayerController _player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DrawCard()
    {
        _player.AddCardToHand(_deckController.DrawCard());
    }
}
