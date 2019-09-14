using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckManager {
    public DeckData _deckData;
    public List<int> _availableCardsIdList = new List<int>();

    public DeckManager (DeckData deckData) {
        _deckData = deckData;
        for(int i = 0; i < _deckData.cards.Count;i++){
            _availableCardsIdList.Add(i);
        }
    }

    public CardData GetCardFromId (int cardId) {
        CardData drawedCard = _deckData.cards[cardId];
        return drawedCard;
    }

    public int GetRandomCardIdFromDeck(){
        if (_availableCardsIdList != null && _availableCardsIdList.Count > 0) {
            int cardId = GetRandomCardId ();
            _availableCardsIdList.RemoveAt (cardId);
            return cardId;
        }
        return -1;
    }

    public int GetDeckSize() {
        return _availableCardsIdList.Count;
    }

    private int GetRandomCardId () {
        return Random.Range (0, _availableCardsIdList.Count);
    }
}