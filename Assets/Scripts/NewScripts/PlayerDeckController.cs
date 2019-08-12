using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDeckController : MonoBehaviour {
    // UI
    public Image _cardBackground;
    public Text _cardCounter;

    public GameObject _cardPrefab;
    public GameObject _dropzone;

    public DeckController _deckController;
    public PlayerController _playerController;

    public void InitPlayerDeck (DeckController deckController, PlayerController playerController) {
        _deckController = deckController;
        _playerController = playerController;
        string path = Application.dataPath;
        _cardBackground.sprite = IMG2Sprite.instance.LoadNewSprite (path + "/Sprites/Fractions/" + _deckController._deckData.fractionId + ".png");
        _cardCounter.text = _deckController._deckData.cards.Count.ToString ();
    }

    public void AddCardToHand (CardData cardData) {
        if (cardData != null) {
            GameObject gameObject = Instantiate (_cardPrefab, _dropzone.transform) as GameObject;
            gameObject.GetComponent<SmallCardController> ()._card = cardData;
            gameObject.GetComponent<SmallCardController> ()._playerId = _playerController._playerData.playerId;
        }
    }

    public void DrawCard () {
        AddCardToHand (_deckController.GetCard ());
        GameController.instance.UpdateDecksCounter ();
    }

    public void UpdateDeckCounter () {
        _cardCounter.text = _deckController._deckData.cards.Count.ToString ();
    }

}