
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardViewerController : MonoBehaviour {

    public GameObject _fullSizeCardPrefab;
    public Transform _parent;
    private GameObject _fullSizeCard;
    private bool _isShowed = false;

    private static CardViewerController _instance;

    public static CardViewerController instance {
        get {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<CardViewerController> ();
            return _instance;
        }
    }

    public void ShowFullSizeCard (SmallCardController card) {
        if (_isShowed) {
            Destroy (_fullSizeCard);
        } else {
            _isShowed = true;
        }
        _fullSizeCard = Instantiate (_fullSizeCardPrefab, _parent) as GameObject;
        _fullSizeCard.GetComponent<BigCardController> ().InitData(card);
    }

    public void HideFullSizeCard () {
        if (_isShowed) {
            Destroy (_fullSizeCard);
            _isShowed = false;
        } else {

        }
    }
}