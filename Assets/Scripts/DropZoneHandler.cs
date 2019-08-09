using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropZoneHandler : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int actionType = 0;
    public int fractionId = 0;
    public int playerId = 0;
    public bool permDropZone = false;

    public Image image;
    public Color originalColor;
    public Color ableColor;
    public Color unableColor;

    public void OnDrop(PointerEventData eventData)
    {
        //if (eventData.pointerDrag.GetComponent<Draggable>() == null)
        //    return;

        //Debug.Log(eventData.pointerDrag.name + " was drop on " + gameObject.name);

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        CardHandler card = eventData.pointerDrag.GetComponent<CardHandler>();

        if (IsAbleToDrop(card))
        {
            if (d != null)
            {               
                if(playerId > 0 )
                {
                    if(GameHandler.instance.BuildCard(card, GameDataHandler.instance.gameState.players[playerId - 1]))
                    {
                        d.m_parentToReturn = this.transform;
                    }
                }
                
            }
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //if (eventData.selectedObject.GetType() != typeof(Draggable))
        //{
        //    return;
        //}

        //if (eventData.pointerDrag.GetType() != typeof(Draggable))
        //{
        //    return;
        //}

        if (eventData.pointerDrag == null)
        {
            return;
        }


        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (IsAbleToDrop(eventData.pointerDrag.GetComponent<CardHandler>()))
        {
            if (d != null)
            {
                d.m_placeholderParent = this.transform;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //if (eventData.pointerDrag.GetComponent<Draggable>() == null)
        //    return;

        //if (eventData.selectedObject.GetType() != typeof(Draggable))
        //{
        //    return;
        //}

        //if (eventData.pointerDrag.GetType() != typeof(Draggable))
        //{
        //    return;
        //}

        if (eventData.pointerDrag == null)
        {
            return;
        }

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        //if (IsAbleToDrop(eventData.pointerDrag.GetComponent<CardHandler>()))
        {
            if (d != null)//&& d.m_placeholderParent == this.transform)
            {
                d.m_placeholderParent = d.m_parentToReturn;
            }
        }
    }

    public bool IsAbleToDrop(CardHandler card) //TODO | ERROR is checked to ScrollView
    {
        if (actionType == 0 && fractionId == 0 && playerId == 0)
            return true;

        if (card.m_fractionId != fractionId)
            return false;
        if (card.m_actionType != actionType)
            return false;
        if (card.m_playerId != playerId)
            return false;

        return true;
    }

    public void MarkAbleDropzone() {
        originalColor = image.color;
        image.color = ableColor;
    }

    public void MarkUnableDropzone()
    {
        originalColor = image.color;
        image.color = unableColor;
    }

    public void UnarkDropzone() {
        image.color = originalColor;
    }
}
