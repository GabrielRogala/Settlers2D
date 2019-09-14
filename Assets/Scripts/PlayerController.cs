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

    public void AddCardToBoard(SmallCardController card){
        _playerBoardPanel.GetComponent<PlayerBoardPanelController>().AddCardToBoard(card);
    }

    public void AddCardToContract(SmallCardController card){
        if(card._card.fractionType > 0){
            _playerBoardPanel.GetComponent<PlayerBoardPanelController>().AddCardToContractPanel(card);
        }
    }

    public void UpdateData() {
        UpdatePlayerResources();
        UpdateDecksSize();
    }

    public void UpdateDecksSize() {
        _playerHandPanel.GetComponent<PlayerHandPanelController>().UpdateDecksSize();
    }

    public void UpdatePlayerResources(){
        _playerBoardPanel.GetComponent<PlayerBoardPanelController>().UpdatePlayerResources();
    }

}