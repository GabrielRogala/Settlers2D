using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoardPanelController : MonoBehaviour {
    // UI
    public List<GameObject> _fractionDropzones;
    public List<GameObject> _defaultDropzones;
    public GameObject _playerInfoPanel;
    public GameObject _contractsPanel;

    public PlayerController _playerController;

    public void InitBoardPanel (PlayerController playerController) {
        _playerController = playerController;
        if (_playerController != null) {
            foreach (GameObject dz in _fractionDropzones) {
                dz.GetComponent<DropzoneController> ()._fractionId = _playerController._playerData.fractionId;
                //dz.GetComponent<DropzoneController>()._actionType = _fractionDropzones.IndexOf(dz) + 1;
                dz.GetComponent<DropzoneController> ()._playerId = _playerController._playerData.playerId;
            }

            foreach (GameObject dz in _defaultDropzones) {
                dz.GetComponent<DropzoneController> ()._fractionId = _playerController._playerData.fractionId;
                //dz.GetComponent<DropzoneController>()._actionType = _defaultDropzones.IndexOf(dz) + 1;
                dz.GetComponent<DropzoneController> ()._playerId = _playerController._playerData.playerId;
            }

            _playerInfoPanel.GetComponent<PlayerResourcesController> ().InitResourcesPanel (_playerController._playerData);
        }

    }

    public void AddCardToBoard(SmallCardController card)
    {
        GameObject newParent;
        if (card._card.fractionType > 0)
        {
            newParent = _fractionDropzones[card._card.actionType - 1];
        }
        else
        {
            newParent = _defaultDropzones[card._card.actionType - 1];
        }
        card.transform.SetParent(newParent.transform);
    }

    public void AddCardToContractPanel (SmallCardController card) {
        card.transform.SetParent (_contractsPanel.transform);
        card.transform.Rotate (new Vector3 (0, 0, 180));
    }

    public void UpdatePlayerResources(){
        _playerInfoPanel.GetComponent<PlayerResourcesController>().UpdateResources();
    }
}