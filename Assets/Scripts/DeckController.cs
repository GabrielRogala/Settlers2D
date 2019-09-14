using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckController : MonoBehaviour
{
    // UI
    public Image _cardBackground;
    public Text _cardCounter;

    public GameObject _cardPrefab;
    public GameObject _dropzone;

    public DeckManager _deckManager;
    public PlayerController _playerController;

    public void InitPlayerDeck(DeckManager deckManager, PlayerController playerController)
    {
        _deckManager = deckManager;
        _playerController = playerController;
        string path = Application.dataPath;
        _cardBackground.sprite = IMG2Sprite.instance.LoadNewSprite(path + "/StreamingAssets/Sprites/Fractions/" + _deckManager._deckData.fractionId + ".png");
        _cardCounter.text = _deckManager.GetDeckSize().ToString();
    }

    private void AddCardToHand(CardData cardData, int playerId, GameObject cardPrefab, GameObject dropzone)
    {
        if (cardData != null)
        {
            GameObject gameObject = Instantiate(_cardPrefab, _dropzone.transform) as GameObject;
            gameObject.GetComponent<SmallCardController>()._card = cardData;
            gameObject.GetComponent<SmallCardController>()._playerId = _playerController._playerData.playerId;
        }
    }

    public void UpdateDeckCounter()
    {
        _cardCounter.text = _deckManager.GetDeckSize().ToString();
    }

    public void DrawCardREQ()
    {
        GameController.instance.DrawCardREQ(_deckManager._deckData.fractionId);
    }

    public void DrawCardCFM(int cardId, int deckSize)
    {
        if (cardId > -1)
        {
            _cardCounter.text = deckSize.ToString();
            AddCardToHand(_deckManager.GetCardFromId(cardId), _playerController._playerData.playerId, _cardPrefab, _dropzone);

        }
    }

}