using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController {
    // UI
    public GameObject _playerHandPanel;
    public GameObject _playerBoardPanel;
    public PlayerData _playerData;

    public PlayerController (PlayerData playerData) {
        _playerData = playerData;
    }

    public void UpdatePlayerResources(){
        _playerBoardPanel.GetComponent<PlayerBoardPanelController>().UpdatePlayerResources();
    }

    public void AddCardToBoard(SmallCardController card){
        GameObject newParent;
        if(card._card.fractionType > 0){
            newParent = _playerBoardPanel.GetComponent<PlayerBoardPanelController>()._fractionDropzones[card._card.actionType -1];
        }else{
            newParent = _playerBoardPanel.GetComponent<PlayerBoardPanelController>()._defaultDropzones[card._card.actionType -1];
        }
        Debug.Log("AddCardToBoard | "+_playerData.name+ " | "+ card._card.ToString());
        card.transform.SetParent(newParent.transform);
    }
    public void AddCardToContract(SmallCardController card){

        if(card._card.fractionType > 0){
            _playerBoardPanel.GetComponent<PlayerBoardPanelController>().AddCardToContractPanel(card.gameObject);
        }

    }
}