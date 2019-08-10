using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // UI
    public GameObject _playerHandPanel;
    public GameObject _cardPrefab;

    public PlayerData _playerData;

    public PlayerController(PlayerData playerData)
    {
        _playerData = playerData;
    }

    public void AddCardToHand(CardData cardData)
    {
        GameObject gameObject = Instantiate(_cardPrefab, _playerHandPanel.transform) as GameObject;
        gameObject.GetComponent<SmallCardController>()._card = cardData;
        gameObject.GetComponent<SmallCardController>()._playerId = _playerData.playerId;
    }
}
