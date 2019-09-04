using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLobbyPanelController : MonoBehaviour
{
    public Text m_PlayerID;
    public Text m_PlayerName;

    public void SetPlayerData(int id, string name) {
        Debug.Log(string.Format("{0}{1}", id, name));
        m_PlayerID.text = id.ToString();
        m_PlayerName.text = name;
    }
}
