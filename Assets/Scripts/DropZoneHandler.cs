using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZoneHandler : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int actionType = 0;
    public int fractionId = 0;
    public int playerId = 0;
    public bool permDropZone = false;

    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log(eventData.pointerDrag.name + " was drop on " + gameObject.name);

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (IsAbleToDrop(eventData))
        {
            if (d != null)
            {
                CardHandler c = eventData.pointerDrag.GetComponent<CardHandler>();
                c.usedCard = true;
                d.m_parentToReturn = this.transform;
            }
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            return;
        }

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (IsAbleToDrop(eventData))
        {
            if (d != null)
            {
                d.m_placeholderParent = this.transform;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            return;
        }

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (IsAbleToDrop(eventData))
        {
            if (d != null)//&& d.m_placeholderParent == this.transform)
            {
                d.m_placeholderParent = d.m_parentToReturn;
            }
        }
    }

    public bool IsAbleToDrop(PointerEventData eventData)
    {
        //CardHandler d = eventData.pointerDrag.GetComponent<CardHandler>();
        //if (fractionId == 0 || fractionId == 0 || playerId == 0)
        //    return true;

        //if (d.fractionId != fractionId)
        //    return false;
        //if (d.actionType != actionType)
        //    return false;
        //if (d.playerId != playerId)
        //    return false;

        return true;
    }
}
