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
        CardViewerController.instance.HideFullSizeCard();
        Debug.Log ("Hide big size card");
    }

    #region OnButtonAction
    public void OnBuildCard ()
    {
        _cardController.BuildCard ();
    }

    public void OnAddCardToContracts ()
    {
        _cardController.AddCardToContracts ();
    }

    public void OnPlunderCard ()
    {
        _cardController.PlunderCard ();
    }

    public void OnTributeCard () // Confirmation required to perform the action
    {
        _cardController.TributeCard ();
    }

    public void OnExecuteAction () // OnButtonClick
    {
        _cardController.ExecuteAction ();
    }

    public void OnExecuteTraitAction () // Confirmation required to perform the action
    {
        _cardController.ExecuteTraitAction ();
    }
    #endregion
}