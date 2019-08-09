using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputPlayerHandler : MonoBehaviour
{
    public Text m_label_UI;
    public InputField m_playerName_UI;
    public Dropdown m_fractionId_UI;

    public PlayerData GetPlayerData()
    {
        PlayerData pd = new PlayerData(0,m_playerName_UI.text,m_fractionId_UI.value);
        return pd;
    }

}
