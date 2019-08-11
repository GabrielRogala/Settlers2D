using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandPanelController : MonoBehaviour
{
    public PlayerDeck _defaultDeck;
    public PlayerDeck _fractionDeck;
    public PlayerController _playerController;

    public void InitPlayerDecks(PlayerController playerController, DeckController defaultDeck, DeckController fractionDeck)
    {
        _playerController = playerController;
        _defaultDeck = new PlayerDeck(defaultDeck, _playerController);
        _fractionDeck = new PlayerDeck(fractionDeck, _playerController);
    }
}
