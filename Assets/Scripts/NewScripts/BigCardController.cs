using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Temporary object only for some actions performed
/// </summary>
public class BigCardController : CardDataViewer, IPointerClickHandler
{
    SmallCardController _cardController;

    public BigCardController(SmallCardController cardController, CardData card) : base(card)
    {
        _cardController = cardController;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Hide big size card");
    }

    public void BuildCard() // OnButtonClick
    {
        _cardController.BuildCard();
    }

    public void AddCardToContracts() // OnButtonClick
    {
        _cardController.AddCardToContracts();
    }

    public void PlunderCard() // OnButtonClick
    {
        _cardController.PlunderCard();
    }

    public void TributeCard() // Confirmation required to perform the action
    {
        _cardController.TributeCard();
    }

    public void ExecuteAction() // OnButtonClick
    {
        _cardController.ExecuteAction();
    }

    public void ExecuteTraitAction() // Confirmation required to perform the action
    {
        _cardController.ExecuteTraitAction();
    }

}
