using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class InputPlayerButtonHandler : MonoBehaviour
{
    public GameObject m_playerInputPrefab;
    public int m_playerCounter = 1;
    private const int MAX_PLAYERS_COUNT = 4;

    public void AddPlayerInput()
    {
        Debug.Log("AddPlayerInput");
        GameObject gameObject;
        if (m_playerCounter < MAX_PLAYERS_COUNT)
        {
            gameObject = Instantiate(m_playerInputPrefab, this.transform.parent) as GameObject;
            gameObject.transform.SetParent(this.transform.parent);
            m_playerCounter++;
            gameObject.GetComponent<InputPlayerHandler>().m_label_UI.text = "Player #" + m_playerCounter;
        }

    }
    public void StartGame()
    {
        Debug.Log("START");

        var foundObjects = FindObjectsOfType<InputPlayerHandler>();
        List<PlayerData> players = new List<PlayerData>();
        foreach(InputPlayerHandler iph in foundObjects)
        {
            PlayerData pd = new PlayerData(iph.m_playerName_UI.text, iph.m_fractionId_UI.value);
            players.Add(pd);
        }
        
        players.Reverse(); // first created last founded

        GameDataHandler.instance.StartGame(players);

        SceneManager.LoadScene("GameScene");
    }
}
