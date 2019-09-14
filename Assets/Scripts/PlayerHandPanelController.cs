using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandPanelController : MonoBehaviour {
    public DeckController _defaultDeck;
    public DeckController _fractionDeck;
    public PlayerController _playerController;

    public void InitPlayerDecks (PlayerController playerController, DeckManager defaultDeck, DeckManager fractionDeck) {
        _playerController = playerController;
        _defaultDeck.InitPlayerDeck (defaultDeck, _playerController);
        _fractionDeck.InitPlayerDeck (fractionDeck, _playerController);
    }

    public void UpdateDecksSize() {
        _defaultDeck.UpdateDeckCounter();
        _fractionDeck.UpdateDeckCounter();
    }
}