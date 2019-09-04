using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    #region SINGLETON
    private static MenuController _instance;
    public static MenuController instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<MenuController>();
            return _instance;
        }
    }
    #endregion

    public InputField m_HostName;
    public InputField m_PlayerName;
    public InputField m_ServerIp;
    public Text m_OwnIp;

    public GameObject m_HostOrJoinPanel;
    public GameObject m_HostPanel;
    public GameObject m_JoinPanel;
    
    public GameObject m_LobbyPanel;
    public GameObject m_ServerButtonPanel;

    public GameObject m_PlayerLobbyPanelPrefab;
    public Transform m_PlayerLobbyPanelParent;

    List<GameObject> m_playersPanel = new List<GameObject>();

    void Start() {
        m_OwnIp.text = "Server IP: " + IPManager.GetLocalIPAddress();
    }

    public void OnHostPanel()
    {
        m_HostPanel.SetActive(true);
        m_HostOrJoinPanel.SetActive(false);
    }

    public void OnJoinPanel()
    {
        m_JoinPanel.SetActive(true);
        m_HostOrJoinPanel.SetActive(false);
    }

    public void OnStartServer()
    {
        m_LobbyPanel.SetActive(true);
        m_ServerButtonPanel.SetActive(true);
        Server.instance.CreateSerwer(m_HostName.text);
    }

    public void OnBack()
    {
        //m_HostPanel.SetActive(false);
        //m_JoinPanel.SetActive(false);
        //m_LobbyPanel.SetActive(false);
        //m_ServerButtonPanel.SetActive(false);
        //m_HostOrJoinPanel.SetActive(true);
    }

    public void OnStartGame()
    {
        Server.instance.StartGame();
    }

    public void OnJoinToServer()
    {
        m_LobbyPanel.SetActive(true);
        m_ServerButtonPanel.SetActive(false);
        Server.instance.JoinToSerwer(m_ServerIp.text);
        //Server.instance.SetPlayerData(m_PlayerName.text);
    }

    public void OnSetPlayerName()
    {
        Server.instance.SetPlayerData(m_PlayerName.text);
    }

    public void UpdateLobby(Dictionary<int, PlayerConnectionData> lobby)
    {
        foreach (GameObject o in m_playersPanel)
        {
            Destroy(o);
        }
        foreach (KeyValuePair<int,PlayerConnectionData> p in lobby) {
            GameObject playerDataInLobby = Instantiate(m_PlayerLobbyPanelPrefab, m_PlayerLobbyPanelParent) as GameObject;
            playerDataInLobby.GetComponent<PlayerLobbyPanelController>().SetPlayerData(p.Value.playerId, p.Value.playerName);

            m_playersPanel.Add(playerDataInLobby);
        }

    }

}
