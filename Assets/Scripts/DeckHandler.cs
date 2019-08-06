﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckHandler : MonoBehaviour
{
    public Transform m_dropZone;
    public GameObject m_cardPrefab;
    public List<CardData> m_deck;
    public int m_fractionId = 0;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Deck INIT");
        m_deck = new List<CardData>();

        foreach (DeckData deck in GameDataHandler.instance.gameData.decks)
        {
            if(deck.fractionId == m_fractionId)
            {
                m_deck.AddRange(deck.cards);
            }
        }
    }

    public void DrawCard(int typeOfDeck)
    {
        if (m_deck.Count > 0)
        {
            int cardId = getRandomCardId();
            CardData drawedCard = GetCardFromId(getRandomCardId());

            GameObject gameObject = Instantiate(m_cardPrefab, m_dropZone) as GameObject;
            gameObject.GetComponent<CardHandler>().m_card = drawedCard;
        }
    }

    public CardData GetCardFromId(int id)
    {
        if (m_deck.Count > 0)
        {
            CardData cadrToReturn = m_deck[id];
            m_deck.RemoveAt(id);
            return cadrToReturn;
        }
        return null;
    }

    public int getRandomCardId()
    {
        return Random.Range(0, m_deck.Count);
    }
}
