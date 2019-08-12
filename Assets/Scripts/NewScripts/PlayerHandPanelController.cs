using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandPanelController : MonoBehaviour {
    public PlayerDeckController _defaultDeck;
    public PlayerDeckController _fractionDeck;
    public PlayerController _playerController;

    public void InitPlayerDecks (PlayerController playerController, DeckController defaultDeck, DeckController fractionDeck) {
        _playerController = playerController;
        _defaultDeck.InitPlayerDeck (defaultDeck, _playerController);
        _fractionDeck.InitPlayerDeck (fractionDeck, _playerController);
    }
}