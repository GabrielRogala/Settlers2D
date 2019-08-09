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

    public GameData gameData;
    public GameState gameState;

    private GameDataHandler()
    {
        gameData = new GameData();
        gameState = new GameState();
    }

    public void StartGame(List<PlayerData> players) {
        gameData = new GameData();
        gameState = new GameState();
        
        gameData = DataHandler.instance.LoadGameData();
        gameState.players.AddRange(players);

        Debug.Log(gameData);
        Debug.Log(gameState);
    }

}