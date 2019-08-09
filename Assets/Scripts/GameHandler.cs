using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    private static GameHandler _instance;
    public static GameHandler instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<GameHandler>();
            return _instance;
        }
    }

    public GameObject m_gameTabParent;
    public GameObject m_gameContentParent;
    public GameObject m_gameTabPrefab;
    public GameObject m_gameContentPrefab;
    public ToggleGroup m_toggleGroup;

    List<PlayerData> m_players;

    private bool isGamePanelReady = false;

    void Start()
    {
        Debug.Log("GAME HANDLER START");

        //Debug.Log(GameDataHandler.instance.gameData.ToString());
        //Debug.Log(GameDataHandler.instance.gameState.ToString());
        //m_players = GameDataHandler.instance.gameState.players;


        /////////////////////////////////////
        m_players = new List<PlayerData>();
        m_players.Add(new PlayerData(1, "name1", 1));
        m_players.Add(new PlayerData(2, "name2", 2));
        m_players.Add(new PlayerData(3, "name3", 3));
        GameDataHandler.instance.StartGame(m_players);
        /////////////////////////////////////

        TabPanelTabHandler.m_contentList = new List<GameObject>();

        foreach (PlayerData p in m_players)
        {
            GameObject tab = Instantiate(m_gameTabPrefab, m_gameTabParent.transform) as GameObject;
            GameObject content = Instantiate(m_gameContentPrefab, m_gameContentParent.transform) as GameObject;

            tab.GetComponent<TabPanelTabHandler>().m_tabPanelLabel.text = p.name;
            tab.GetComponent<TabPanelTabHandler>().m_tabContent = content;

            Debug.Log(tab.GetComponent<TabPanelTabHandler>().ToString());
            tab.GetComponent<Toggle>().group = m_toggleGroup;
            tab.GetComponent<Toggle>().isOn = false;

            content.SetActive(false);
            TabPanelTabHandler.m_contentList.Add(content);


            foreach (DropZoneHandler d in content.GetComponentsInChildren<DropZoneHandler>()) {
                d.playerId = p.playerId;
            }

            content.GetComponent<PlayerTabPanelHandler>().InitResourcesPanel(p);
        }

        foreach (Toggle t in m_gameContentParent.GetComponentsInChildren<Toggle>())
        {
            t.isOn = true;
        }

        foreach (TabPanelTabHandler t in m_gameTabParent.GetComponentsInChildren<TabPanelTabHandler>())
        {
            t.ShowTabContent();
        }

        DeckHandler[] decks = FindObjectsOfType<DeckHandler>();
        foreach(DeckHandler d in decks)
        {
            d.InitDeckData();
        }

        InitPlayersResources(m_players);
        isGamePanelReady = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartGame();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            NextRound();
        }

    }

    private void NextRound()
    {
        ProductPlayersResources(m_players);
        PlayersResourcesUpdate();
    }

    private void StartGame()
    {
        ProductPlayersResources(m_players);
        PlayersResourcesUpdate();
    }

    public void InitPlayersResources(List<PlayerData> players)
    {
        foreach (PlayerData p in players) {
            //Debug.Log(p.name + " : " + GameDataHandler.instance.gameData.fractions[p.fractionId].fractionName);
            var resources = GameDataHandler.instance.gameData.fractions[p.fractionId].resourceGrowthMatrix;
            int i = 0;
            foreach (int r in resources)
            {
                //Debug.Log(GameDataHandler.instance.gameData.resources[i].resourcesName + ": "+ r);
                //p.playerResourcesGrowth.Add(i,r);
                p.playerResourcesGrowth.Add(i, 10);

                p.playerResources.Add(i, 0);
                i++;
            }

        }
    }

    public void ProductPlayersResources(List<PlayerData> players)
    {
        foreach (PlayerData p in players)
        {
            p.ProductResources();
        }
    }

    public bool IsPlayerTurn(PlayerData player)
    {
        return GameDataHandler.instance.gameState.playerIdTurn == GameDataHandler.instance.gameState.players.IndexOf(player);
    }

    public bool BuildCard(CardHandler card, PlayerData player)
    {

        Dictionary<int, int> cost = new Dictionary<int, int>();
        foreach(int resource in card.m_card.cost)
        {
            if (cost.ContainsKey(resource)) {
                cost[resource]++;
            }
            else
            {
                cost.Add(resource, 1);
            }
        }

        bool enoughtCountOfResources = true;

        foreach(KeyValuePair<int,int> kvp in cost)
        {
            Debug.Log("BUILD : "+ GameDataHandler.instance.gameData.resources[kvp.Key].resourcesName+ "  "+ player.playerResources[kvp.Key]+" < " + kvp.Value);
            if (player.playerResources[kvp.Key] < kvp.Value)
            {
                enoughtCountOfResources = false;
            }

        }

        if (enoughtCountOfResources)
        {
            foreach(KeyValuePair<int, int> kvp in cost)
            {
                player.playerResources[kvp.Key] -= kvp.Value;
            }
            card.BuildActionExecute();
            player.cardsInBoard.Add(card.m_card);
            player.cardsInHand.Remove(card.m_card);
        }

        PlayersResourcesUpdate();
        return enoughtCountOfResources;
    }

    public void PlayersResourcesUpdate()
    {
        if (isGamePanelReady)
        {
            var resources = m_gameContentParent.GetComponentsInChildren<ResourcePanelHandler>();

            foreach (ResourcePanelHandler r in resources)
            {
                r.UpdatePanelData();
            }
        }
    }

}