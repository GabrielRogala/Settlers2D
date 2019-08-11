using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoardPanelController : MonoBehaviour
{
    // UI
    public List<GameObject> _fractionDropzones;
    public List<GameObject> _defaultDropzones;
    public GameObject _playerInfoPanel;
    public GameObject _contractsPanel;

    public PlayerData _playerData;

    public void InitBoardPanel(PlayerData playerData)
    {
        _playerData = playerData;
        if (_playerData != null)
        {
            foreach (GameObject dz in _fractionDropzones) {
                dz.GetComponent<DropzoneController>()._fractionId = _playerData.fractionId;
                //dz.GetComponent<DropzoneController>()._actionType = _fractionDropzones.IndexOf(dz) + 1;
                dz.GetComponent<DropzoneController>()._playerId = _playerData.playerId;
            }

            foreach (GameObject dz in _defaultDropzones)
            {
                dz.GetComponent<DropzoneController>()._fractionId = _playerData.fractionId;
                //dz.GetComponent<DropzoneController>()._actionType = _defaultDropzones.IndexOf(dz) + 1;
                dz.GetComponent<DropzoneController>()._playerId = _playerData.playerId;
            }

            _playerInfoPanel.GetComponent<PlayerResourcesController>().InitResourcesPanel(_playerData);
        }

    }

    public void AddCardToContractPanel(GameObject card)
    {
        card.transform.SetParent(_contractsPanel.transform);
        card.transform.Rotate(new Vector3(0, 0, 180));
    }
}
