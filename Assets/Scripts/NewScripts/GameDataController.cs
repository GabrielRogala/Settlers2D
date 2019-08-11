using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataController
{
    //////////////////////////////////////////
    private static GameDataController _instance;
    public static GameDataController instance
    {
        get
        {
            if (_instance == null)
                _instance = new GameDataController();
            return _instance;
        }
    }
    //////////////////////////////////////////

    public GameData gameData;
    public GameState gameState;

    public void InitGameData(List<PlayerData> players)
    {
        gameData = DataController.LoadGameData();
        gameState = new GameState();
        gameState.players.AddRange(players);

        Debug.Log(gameData);
        Debug.Log(gameState);
    }
}
