using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckController {
    public DeckData _deckData;

    public DeckController (DeckData deckData) {
        _deckData = deckData;
    }

    public CardData GetCard () {
        if (_deckData != null && _deckData.cards.Count > 0) {
            int cardId = getRandomCardId ();
            CardData drawedCard = _deckData.cards[cardId];
            _deckData.cards.RemoveAt (cardId);
            return drawedCard;
        }

        return null;
    }

    private int getRandomCardId () {
        return Random.Range (0, _deckData.cards.Count);
    }
}