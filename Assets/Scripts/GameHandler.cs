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

    List<PlayerData> players;

    void Start()
    {
        //Debug.Log("GAME HANDLER START");
        //Debug.Log(GameDataHandler.instance.gameData.ToString());
        //Debug.Log(GameDataHandler.instance.gameState.ToString());
        //players = GameDataHandler.instance.gameState.players;

        players = new List<PlayerData>();
        players.Add(new PlayerData("name1", 1));
        players.Add(new PlayerData("name2", 2));
        players.Add(new PlayerData("name3", 3));

        GameDataHandler.instance.StartGame(players);

        TabPanelTabHandler.m_contentList = new List<GameObject>();

        foreach (PlayerData p in players)
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

            //if (players.IndexOf(p) == 0)
            //{
            //    tab.GetComponent<Toggle>().isOn = true;
            //    tab.GetComponent<TabPanelTabHandler>().ShowTabContent();
            //}

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
    }


    void Update()
    {

    }


    //////////////////////////////////////



    //// Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.X))
    //    { // save game state
    //        GameState gameState = new GameState();

    //        PlayerData player1 = new PlayerData("Player name 1", 1);
    //        PlayerData player2 = new PlayerData("Player name 2", 2);
    //        gameState.players.Add(player1);
    //        gameState.players.Add(player2);

    //        CardData card0 = new CardData();
    //        card0.cardName = "Card Name 0";
    //        CardData card1 = new CardData();
    //        card1.cardName = "Card Name 1";
    //        CardData card2 = new CardData();
    //        card2.cardName = "Card Name 2";

    //        DeckData deck0 = new DeckData(0);
    //        deck0.AddCard(card0);
    //        DeckData deck1 = new DeckData(1);
    //        deck1.AddCard(card1);
    //        DeckData deck2 = new DeckData(2);
    //        deck2.AddCard(card2);
    //        gameState.decks.Add(deck0);
    //        gameState.decks.Add(deck1);
    //        gameState.decks.Add(deck2);

    //        gameState.roundCounter = 0;
    //        gameState.playerIdTurn = 0;

    //        //DataHandler.SaveGameState(gameState);
    //    }

    //    if (Input.GetKeyDown(KeyCode.Z)) // save game data
    //    {

    //        gameData = new GameData();

    //        ResourcesData resource1 = new ResourcesData();
    //        resource1.resourcesId = 1;
    //        resource1.resourcesName = "resource 1";
    //        ResourcesData resource2 = new ResourcesData();
    //        resource2.resourcesId = 2;
    //        resource2.resourcesName = "resource 2";
    //        gameData.resources.Add(resource1);
    //        gameData.resources.Add(resource2);

    //        FractionData fraction1 = new FractionData();
    //        fraction1.fractionId = 1;
    //        fraction1.fractionName = "Fraction name 1";
    //        FractionData fraction2 = new FractionData();
    //        fraction2.fractionId = 2;
    //        fraction2.fractionName = "Fraction name 2";
    //        gameData.fractions.Add(fraction1);
    //        gameData.fractions.Add(fraction2);

    //        CardData card0 = new CardData();
    //        card0.cardName = "Card Name 0";
    //        CardData card1 = new CardData();
    //        card1.cardName = "Card Name 1";
    //        CardData card2 = new CardData();
    //        card2.cardName = "Card Name 2";

    //        DeckData deck0 = new DeckData(0);
    //        deck0.AddCard(card0);
    //        DeckData deck1 = new DeckData(1);
    //        deck1.AddCard(card1);
    //        DeckData deck2 = new DeckData(2);
    //        deck2.AddCard(card2);
    //        gameData.decks.Add(deck0);
    //        gameData.decks.Add(deck1);
    //        gameData.decks.Add(deck2);

    //        //DataHandler.SaveGameData(gameData);
    //    }

    //}

}