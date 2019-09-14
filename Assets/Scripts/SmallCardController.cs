using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SmallCardController : CardDataViewer, IPointerClickHandler {
    public int _playerId;

    public SmallCardController (CardData card) : base (card) { }

    public void OnPointerClick (PointerEventData eventData) {
        CardViewerController.instance.ShowFullSizeCard(this);
    }

    #region CardAction
    public void BuildCard () {
        Debug.Log ("BuildCard");
        if(GameController.instance.BuildCard(_playerId,this)){
            switch (_card.actionType) {
                case 1:
                    { // production card
                        ExecuteProduction ();
                        ExecuteProductionAfterBuild ();
                        SubscribeToProduction ();
                        break;
                    }
                case 2: // trait card
                    {
                        ExecuteProductionAfterBuild ();
                        SubscribeToTraitAction ();
                        break;
                    }
                case 3: // action card
                    {
                        ExecuteProductionAfterBuild ();
                        break;
                    }
                default:
                    { break; }
            }   
        }

        
    }

    public void AddCardToContracts () {
        Debug.Log ("AddCardToContracts");
        GameController.instance.AddCardToContract(_playerId,this);
    }

    public void PlunderCard () {
        Debug.Log ("PlunderCard");
        GameController.instance.PlunderCard(_playerId,this);
    }

    public void TributeCard () {
        Debug.Log ("TributeCard");
        GameController.instance.TributeCard(_playerId,this);
    }

    public void ExecuteAction () {
        Debug.Log ("ExecuteAction");
    }

    public void ExecuteProduction () {
        Debug.Log ("ExecuteProduction");
    }

    public void ExecuteProductionAfterBuild () {
        Debug.Log ("ExecuteProductionAfterBuild");
    }

    public void ExecuteTraitAction () {
        Debug.Log ("ExecuteTraitAction");
    }
    #endregion

    private void SubscribeToTraitAction () {
        Debug.Log ("SubscribeToTraitAction");
    }

    private void SubscribeToProduction () {
        Debug.Log ("SubscribeToProduction");
    }
}