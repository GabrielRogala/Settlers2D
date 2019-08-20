
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardViewer : MonoBehaviour {

    public GameObject m_fullSizeCardPrefab;
    public Transform m_parent;
    private GameObject fullSizeCard;
    private bool isShowed = false;

    private static CardViewer _instance;

    public static CardViewer instance {
        get {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<CardViewer> ();
            return _instance;
        }
    }

    public void ShowFullSizeCard (SmallCardController card) {
        if (isShowed) {
            Destroy (fullSizeCard);
        } else {
            isShowed = true;
        }
        fullSizeCard = Instantiate (m_fullSizeCardPrefab, m_parent) as GameObject;
        fullSizeCard.GetComponent<BigCardController> ().InitData(card);
    }

    public void HideFullSizeCard () {
        if (isShowed) {
            Destroy (fullSizeCard);
            isShowed = false;
        } else {

        }
    }
}