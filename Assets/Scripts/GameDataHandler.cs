using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataHandler
{
    private static GameDataHandler _instance;
    public static GameDataHandler instance
    {
        get
        {
            if (_instance == null)
                _instance = new GameDataHandler();
            return _instance;
        }
    }
    //////////////////////////////////////

    public GameData gameData;
    public GameState gameState;

    private GameDataHandler()
    {
        gameData = new GameData();
        gameState = new GameState();
    }

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

    public void StartGame(List<PlayerData> players) {
        gameData = new GameData();
        gameState = new GameState();
        
        gameData = DataHandler.instance.LoadGameData();
        gameState.players.AddRange(players);

        //Debug.Log(gameData);
        //Debug.Log(gameState);
    }

}