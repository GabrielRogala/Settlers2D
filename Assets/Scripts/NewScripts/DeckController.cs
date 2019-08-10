using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckController : MonoBehaviour
{
    // UI
    public Image _cardBackground;
    public Text _cardCounter;

    public DeckData _deckData;

    public DeckController(DeckData deckData)
    {
        _deckData = deckData;
        string path = Application.dataPath;
        _cardBackground.sprite = IMG2Sprite.instance.LoadNewSprite(path + "/Sprites/Fractions/" + _deckData.fractionId + ".png");
        _cardCounter.text = _deckData.cards.Count.ToString();
    }

    public CardData GetCard(PlayerController player)
    {
        if(_deckData != null && _deckData.cards.Count > 0)
        {
            int cardId = getRandomCardId();
            CardData drawedCard = _deckData.cards[cardId];
            _deckData.cards.RemoveAt(cardId);
            _cardCounter.text = _deckData.cards.Count.ToString();

            return drawedCard;
        }

        return null;
    }

    private int getRandomCardId()
    {
        return Random.Range(0, _deckData.cards.Count);
    }
}
