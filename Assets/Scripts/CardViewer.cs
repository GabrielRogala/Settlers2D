using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardViewer : MonoBehaviour
{

    public GameObject m_fullSizeCardPrefab;
    public Transform m_parent;
    private GameObject fullSizeCard;
    private bool isShowed = false;

    private static CardViewer _instance;

    public static CardViewer instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<CardViewer>();
            return _instance;
        }
    }

    public void ShowFullSizeCard(CardData card)
    {
        if (isShowed)
        {
            Destroy(fullSizeCard);
            fullSizeCard = Instantiate(m_fullSizeCardPrefab, m_parent) as GameObject;
            fullSizeCard.GetComponent<FullSizeCardHandler>().m_card = card;
        }
        else
        {
            fullSizeCard = Instantiate(m_fullSizeCardPrefab, m_parent) as GameObject;
            fullSizeCard.GetComponent<FullSizeCardHandler>().m_card = card;
            isShowed = true;
        }
    }

    public void HideFullSizeCard()
    {
        if (isShowed)
        {
            Destroy(fullSizeCard);
            isShowed = false;
        }
        else
        {

        }
    }
}
