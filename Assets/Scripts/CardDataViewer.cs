using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDataViewer : MonoBehaviour {
    // UI
    public Text _id;
    public Text _name;
    public Text _description;
    public Image _cardType;
    public Image _fractionImage;
    public Image _contract;
    public Image _contractBackground;
    public List<Image> _costs;
    public List<Image> _gains;
    public Image _image;

    public CardData _card;

    // Start is called before the first frame update
    void Start () {
        string path = Application.dataPath;

        _id.text = "#" + _card.cardId;
        _name.text = _card.cardName;
        _description.text = _card.description;
        _fractionImage.sprite = IMG2Sprite.instance.LoadNewSprite (path + "/StreamingAssets/Sprites/Fractions/" + _card.fractionType + ".png");
        _cardType.sprite = IMG2Sprite.instance.LoadNewSprite (path + "/StreamingAssets/Sprites/Types/" + _card.cardType + ".png");
        _contract.sprite = IMG2Sprite.instance.LoadNewSprite (path + "/StreamingAssets/Sprites/Resources/" + _card.contract + ".png");

        for (int i = 0; i < _costs.Count; i++) {
            _costs[i].gameObject.SetActive (false);
        }

        for (int i = 0; i < _gains.Count; i++) {
            _gains[i].gameObject.SetActive (false);
        }

        for (int i = 0; i < _card.cost.Count; i++) {
            _costs[i].gameObject.SetActive (true);
            _costs[i].sprite = IMG2Sprite.instance.LoadNewSprite (path + "/StreamingAssets/Sprites/Resources/" + _card.cost[i] + ".png");
        }

        for (int i = 0; i < _card.gain.Count; i++) {
            _gains[i].gameObject.SetActive (true);
            _gains[i].sprite = IMG2Sprite.instance.LoadNewSprite (path + "/StreamingAssets/Sprites/Resources/" + _card.gain[i] + ".png");
        }

        _image.sprite = IMG2Sprite.instance.LoadNewSprite (path + "/StreamingAssets/Sprites/Screens/" + _card.image + ".png");

        if (_card.fractionType == 0) {
            _contractBackground.color = Color.clear;
        }
    }

    // Update is called once per frame
    void Update () {

    }

    public CardDataViewer (CardData card) {
        this._card = card;
    }
}