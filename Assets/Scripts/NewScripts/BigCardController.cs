using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Temporary object only for some actions performed
/// </summary>
public class BigCardController : CardDataViewer, IPointerClickHandler {
    public SmallCardController _cardController;

    public BigCardController (SmallCardController cardController, CardData card) : base (card) {
        _cardController = cardController;
    }

    public void InitData(SmallCardController cardController)
    {
        _cardController = cardController;
        _card = cardController._card;
        Debug.Log("check player turn");
        Debug.Log("playerId "+ cardController._playerId);
        if (!GameController.instance.IsPlayerTurn(cardController._playerId))
        {
            Debug.Log("Is not player turn");
            foreach (Button b in GetComponentsInChildren<Button>())
            {
                b.gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerClick (PointerEventData eventData) {
        CardViewer.instance.HideFullSizeCard();
        Debug.Log ("Hide big size card");
    }

    public void BuildCard () // OnButtonClick
    {
        _cardController.BuildCard ();
    }

    public void AddCardToContracts () // OnButtonClick
    {
        _cardController.AddCardToContracts ();
    }

    public void PlunderCard () // OnButtonClick
    {
        _cardController.PlunderCard ();
    }

    public void TributeCard () // Confirmation required to perform the action
    {
        _cardController.TributeCard ();
    }

    public void ExecuteAction () // OnButtonClick
    {
        _cardController.ExecuteAction ();
    }

    public void ExecuteTraitAction () // Confirmation required to perform the action
    {
        _cardController.ExecuteTraitAction ();
    }

}