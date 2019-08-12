using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SmallCardController : CardDataViewer, IPointerClickHandler {
    public int _playerId;

    public SmallCardController (CardData card) : base (card) { }

    public void OnPointerClick (PointerEventData eventData) {
        Debug.Log ("Show big size card");
        CardViewer.instance.ShowFullSizeCard(this);
    }

    public void BuildCard () {
        Debug.Log ("BuildCard");
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

    public void AddCardToContracts () {
        Debug.Log ("AddCardToContracts");
    }

    public void PlunderCard () {
        Debug.Log ("PlunderCard");
    }

    public void TributeCard () {
        Debug.Log ("TributeCard");
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

    public void SubscribeToTraitAction () {
        Debug.Log ("SubscribeToTraitAction");
    }

    public void SubscribeToProduction () {
        Debug.Log ("SubscribeToProduction");
    }
}