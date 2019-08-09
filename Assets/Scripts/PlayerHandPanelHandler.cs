using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandPanelHandler : MonoBehaviour
{
    public GameObject m_dropzone;
    public GameObject m_fractionDeck;
    public GameObject m_defaultDeck;

    public void InitDeckData(int playerId, int fractionId)
    {
        m_fractionDeck.GetComponent<DeckHandler>().m_fractionId = fractionId;
        m_fractionDeck.GetComponent<DeckHandler>().m_playerId = playerId;
        m_defaultDeck.GetComponent<DeckHandler>().m_playerId = playerId;
    }

}
