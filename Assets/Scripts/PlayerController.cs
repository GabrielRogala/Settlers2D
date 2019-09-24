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

    public void AddCardToHand(int deckId, int cardId, int deckSize) {
        if (deckId>0)
        {
            _playerHandPanel.GetComponent<PlayerHandPanelController>()._fractionDeck.DrawCardCFM(cardId,deckSize);
        }
        else
        {
            _playerHandPanel.GetComponent<PlayerHandPanelController>()._defaultDeck.DrawCardCFM(cardId, deckSize);
        }
        
    }

    public void UpdateData() {
        UpdatePlayerResources();
        //UpdateDecksSize();
    }

    public void UpdateDecksSize(int deckId, int deckSize) {
        if (deckId == _playerData.fractionId)
            _playerHandPanel.GetComponent<PlayerHandPanelController>()._fractionDeck.UpdateDeckCounter(deckSize);

        if(deckId == 0)
            _playerHandPanel.GetComponent<PlayerHandPanelController>()._defaultDeck.UpdateDeckCounter(deckSize);
    }

    public void UpdatePlayerResources(){
        _playerBoardPanel.GetComponent<PlayerBoardPanelController>().UpdatePlayerResources();
    }

}