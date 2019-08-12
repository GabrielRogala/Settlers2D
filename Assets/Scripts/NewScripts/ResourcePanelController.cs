using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePanelController : MonoBehaviour {
    // UI
    public Image _image;
    public Text _count;
    public Text _growth;

    public int _resourceId = 0;
    public PlayerData _playerData;

    public void InitResourcePanel (PlayerData playerData, int resourceId) {
        _playerData = playerData;
        _resourceId = resourceId;

        string path = Application.dataPath;
        _image.sprite = IMG2Sprite.instance.LoadNewSprite (path + "/Sprites/Resources/" + _resourceId + ".png");
        UpdatePanelData ();
    }

    public void UpdatePanelData () {
        if (_playerData != null) {
            _count.text = _playerData.playerResources[_resourceId].ToString ();
            _growth.text = "+" + _playerData.playerResourcesGrowth[_resourceId].ToString ();
        }
    }
}