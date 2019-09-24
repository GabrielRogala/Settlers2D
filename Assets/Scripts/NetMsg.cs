using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class PlayerConnectionData
{
    public int connectionId;
    public int hostId;
    public int chanelId;
    public string playerName;
    public int playerId;
    public int fractionId;

    public PlayerConnectionData(int connectionId, int hostId, int chanelId, string playerName, int playerId, int fractionId)
    {
        this.connectionId = connectionId;
        this.hostId = hostId;
        this.chanelId = chanelId;
        this.playerName = playerName;
        this.playerId = playerId;
        this.fractionId = fractionId;
    }
}

public static class NetOP
{
    public const int NONE = 0;
    public const int SET_PLAYER_DATA = 1; // ATTR: NAME | send player name after connect
    public const int SET_PLAYER_ID = 2; // ATTR: playerId
    public const int UPDATE_LOBBY = 3; // ATTR: <playerId, playerName>
    public const int MESSAGE = 4;

    public const int START_GAME_REQ = 5; // ATTR: <playerid, playerName>
    public const int START_GAME_CFM = 6; // 

    public const int SET_PLAYER_ID_TURN = 7; // ATTR: playerId 

    public const int DRAW_CARD_REQ = 8; // ATTR: playerId, fractionId
    public const int DRAW_CARD_CFM = 9; // ATTR: cardId, fractionId

    public const int UPDATE_PLAYERS_RESOURCES = 10; // BTC | ATTR: <playerId, <resources>, <resourcesGrowth>>

    // card action
    public const int BUILD_CARD_REQ = 11; // ATTR: playerId, cardId, fractionId
    public const int BUILD_CARD_CFM = 12; // BTC | ATTR: playerId, cardId, fractionId
    public const int BUILD_CARD_REJ = 13;






    public const int PLOUNDER_CARD_REQ = 1; // ATTR: playerId, oponentId, cardId, fractionId
    public const int PLOUNDER_CARD_CFM = 1; // BTC | ATTR: playerId, oponentId, cardId, fractionId
    public const int PLOUNDER_CARD_REJ = 1;

    public const int TRIBUTE_CARD_REQ = 1; // ATTR: playerId, cardId, fractionId
    public const int TRIBUTE_CARD_CFM = 1; // BTC | ATTR: playerId, cardId, fractionId
    public const int TRIBUTE_CARD_REJ = 1;

    public const int CONTRACT_CARD_REQ = 1; // ATTR: playerId, cardId, fractionId
    public const int CONTRACT_CARD_CFM = 1; // BTC | ATTR: playerId, cardId, fractionId
    public const int CONTRACT_CARD_REJ = 1;

    public const int DO_ACTION_CARD_REQ = 1; // ATTR: playerId, cardId, fractionId
    // chose other card
    // chose player
    public const int DO_ACTION_CARD_CFM = 1; // BTC | ATTR: playerId, cardId, fractionId
    public const int DO_ACTION_CARD_REJ = 1;

    public const int CHANGE_WORKERS_TO_RESOURCE_REQ = 1; // ATTR: playerId, resourceId
    public const int CHANGE_WORKERS_TO_RESOURCE_CFM = 1; // BTC | ATTR: playerId, resourceId
    public const int CHANGE_WORKERS_TO_RESOURCE_REJ = 1;

}

[System.Serializable]
public class NetMsg
{
    public byte OperationCode;

    public NetMsg()
    {
        OperationCode = NetOP.NONE;
    }
}

[System.Serializable]
public class Net_Message : NetMsg
{
    public Net_Message(string message)
    {
        OperationCode = NetOP.MESSAGE;
        this.message = message;
    }
    public string message;
}

[System.Serializable]
public class Net_PlayerData : NetMsg
{
    public Net_PlayerData(int id, string name)
    {
        OperationCode = NetOP.SET_PLAYER_DATA;
        playerId = id;
        playerName = name;
    }
    public int playerId;
    public string playerName;
}

[System.Serializable]
public class Net_PlayerId : NetMsg
{
    public Net_PlayerId(int id)
    {
        OperationCode = NetOP.SET_PLAYER_ID;
        playerId = id;
    }
    public int playerId;
}

[System.Serializable]
public class Net_UpdateLobby : NetMsg
{
    public Net_UpdateLobby(List<PlayerConnectionData> players)
    {
        OperationCode = NetOP.UPDATE_LOBBY;
        PlayersData = players;
    }

    public List<PlayerConnectionData> PlayersData;

}

[System.Serializable]
public class Net_StartGameREQ : NetMsg
{
    public Net_StartGameREQ(List<PlayerConnectionData> players)
    {
        OperationCode = NetOP.START_GAME_REQ;
        PlayersData = players;
    }

    public List<PlayerConnectionData> PlayersData;
}

[System.Serializable]
public class Net_StartGameCFM : NetMsg
{
    public Net_StartGameCFM(int id)
    {
        OperationCode = NetOP.START_GAME_CFM;
        PlayerId = id;
    }
    public int PlayerId;
}

[System.Serializable]
public class Net_SetPlayerIdTurn : NetMsg
{
    public Net_SetPlayerIdTurn(int id)
    {
        OperationCode = NetOP.SET_PLAYER_ID_TURN;
        PlayerId = id;
    }
    public int PlayerId;
}

[System.Serializable]
public class Net_DrawCardREQ : NetMsg
{
    public Net_DrawCardREQ(int playerId, int deckId)
    {
        OperationCode = NetOP.DRAW_CARD_REQ;
        PlayerId = playerId;
        DeckId = deckId;
    }
    public int PlayerId;
    public int DeckId;
}

[System.Serializable]
public class Net_DrawCardCFM : NetMsg
{
    public Net_DrawCardCFM(int playerId, int deckId, int cardId, int deckSize)
    {
        OperationCode = NetOP.DRAW_CARD_CFM;
        PlayerId = playerId;
        DeckId = deckId;
        CardId = cardId;
        DeckSize = deckSize;
    }
    public int PlayerId;
    public int DeckId;
    public int CardId;
    public int DeckSize;
}

[System.Serializable]
public class Net_BuildCardREQ : NetMsg
{
    public Net_BuildCardREQ(int playerId, int deckId, int cardId)
    {
        OperationCode = NetOP.BUILD_CARD_REQ;
        PlayerId = playerId;
        DeckId = deckId;
        CardId = cardId;
    }

    public int PlayerId;
    public int DeckId;
    public int CardId;
}

[System.Serializable]
public class Net_BuildCardCFM : NetMsg
{
    public Net_BuildCardCFM(int playerId, int deckId, int cardId)
    {
        OperationCode = NetOP.BUILD_CARD_CFM;
        PlayerId = playerId;
        DeckId = deckId;
        CardId = cardId;
    }

    public int PlayerId;
    public int DeckId;
    public int CardId;
}

[System.Serializable]
public class Net_BuildCardREJ : NetMsg
{
    public Net_BuildCardREJ(int playerId, int deckId, int cardId)
    {
        OperationCode = NetOP.BUILD_CARD_REJ;
        PlayerId = playerId;
        DeckId = deckId;
        CardId = cardId;
    }

    public int PlayerId;
    public int DeckId;
    public int CardId;
}