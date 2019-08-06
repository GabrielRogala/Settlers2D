using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    public Transform m_parentToReturn = null;
    public Transform m_placeholderParent = null;

    private Transform m_temporatyCardHolder;

    GameObject m_placeholder = null;
    LayoutElement m_layout = null;

    public void OnBeginDrag(PointerEventData eventData)
    {
        //CardHandler c = eventData.pointerDrag.GetComponent<CardHandler>();

        //if (c.usedCard) {
        //    return;
        //}

        m_placeholder = new GameObject();
        m_placeholder.transform.SetParent(this.transform.parent);

        if (m_layout == null)
        {
            m_layout = m_placeholder.AddComponent<LayoutElement>();
        }

        m_placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());

        m_parentToReturn = this.transform.parent;
        m_placeholderParent = m_parentToReturn;

        m_temporatyCardHolder = this.transform.parent.parent;
        while (m_temporatyCardHolder.parent != null)
        {
            m_temporatyCardHolder = m_temporatyCardHolder.parent;
        }


        this.transform.SetParent(m_temporatyCardHolder);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //CardHandler c = eventData.pointerDrag.GetComponent<CardHandler>();

        //if (c.usedCard)
        //{
        //    return;
        //}

        this.transform.position = eventData.position;

        if (m_placeholder.transform.parent != m_placeholderParent)
        {
            m_placeholder.transform.SetParent(m_placeholderParent);
        }

        int newSiblingIndex = m_placeholderParent.childCount;
        for (int i = 0; i < m_placeholderParent.childCount; i++)
        {
            if (this.transform.position.x < m_placeholderParent.GetChild(i).position.x)
            {
                newSiblingIndex = i;
                if (m_placeholder.transform.GetSiblingIndex() < newSiblingIndex)
                {
                    newSiblingIndex--;
                }
                break;
            }

        }

        m_placeholder.transform.SetSiblingIndex(newSiblingIndex);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //CardHandler c = eventData.pointerDrag.GetComponent<CardHandler>();

        //if (c.usedCard)
        //{
        //    return;
        //}

        this.transform.SetParent(m_parentToReturn);
        this.transform.SetSiblingIndex(m_placeholder.transform.GetSiblingIndex());
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        Destroy(m_placeholder);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("click :" + eventData.ToString());
        //CardViewer.instance.ShowFullSizeCard(this.GetComponent<CardHandler>().m_card);
    }

}
