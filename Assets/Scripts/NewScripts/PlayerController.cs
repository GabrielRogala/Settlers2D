using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController {
    // UI
    public GameObject _playerHandPanel;
    public GameObject _playerBoardPanel;
    public PlayerData _playerData;

    public PlayerController (PlayerData playerData) {
        _playerData = playerData;
    }

}