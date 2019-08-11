using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck
{
    public DeckController _deckController;
    public PlayerController _player;

    public PlayerDeck(DeckController deckController, PlayerController player)
    {
        _deckController = deckController;
        _player = player;
    }

    public void DrawCard()
    {
        _player.AddCardToHand(_deckController.GetCard(_player));
    }
}
