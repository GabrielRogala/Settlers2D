﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Server : MonoBehaviour
{
    #region SINGLETON
    private static Server _instance;
    public static Server instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<Server>();
            return _instance;
        }
    }
    #endregion

    private const int MAX_USER = 10;
    private const int PORT = 26000;
    private const int WEB_PORT = 26001;
    private const int BYTE_SIZE = 1024;
    private string serwerIp = "127.0.0.1";

    private byte reliableChannel;
    private byte error;
    private bool isStarted;
    private bool isServer = false;

    #region ServerData
    private int hostId = -1;
    private int webHostId = -1;
    Dictionary<int, int> m_ConnectionHostIds = new Dictionary<int, int>();
    Dictionary<int, PlayerConnectionData> m_PlayersConnectionData = new Dictionary<int, PlayerConnectionData>();
    private List<string> playerList = new List<string>();
    private int m_nextAvaliablePlayerId = 1;
    #endregion

    #region ClientData
    private PlayerConnectionData m_ConnectionData = new PlayerConnectionData(0, 0, 0, "", 0);
    private int m_playerId;
    #endregion

    #region MonoBehaviour
    void Start()
    {
    }
    void Update()
    {
        UpdateMessagePump();
    }
    #endregion

    #region Connection
    private void InitConnection()
    {
        NetworkTransport.Init();

        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.Reliable);

        HostTopology topo = new HostTopology(cc, MAX_USER);


        if (isServer)
        {
            // SERVER ONLY
            hostId = NetworkTransport.AddHost(topo, PORT, null);
            webHostId = NetworkTransport.AddWebsocketHost(topo, WEB_PORT, null);

            Debug.Log(string.Format("Opening connection on port {0} and webport {1}", PORT, WEB_PORT));

            isStarted = true;
        }
        else
        {
            // Client ONLY
            m_ConnectionData.hostId = NetworkTransport.AddHost(topo, 0);
#if !UNITY_WEBGL
            // standalone client
            m_ConnectionData.connectionId = NetworkTransport.Connect(m_ConnectionData.hostId, serwerIp, PORT, 0, out error);
            Debug.Log(string.Format("Connecting from standalone"));
#else
            // web client
            m_ConnectionData.connectionId = NetworkTransport.Connect(m_ConnectionData.hostId, serwerIp, WEB_PORT, 0, out error);
            Debug.Log(string.Format("Connecting from web"));
#endif
            Debug.Log(string.Format("Attempting to connect on {0}....", serwerIp));

            isStarted = true;
        }
    }
    private void Shutdown()
    {
        isStarted = false;
        NetworkTransport.Shutdown();
    }
    private void UpdateMessagePump()
    {
        if (!isStarted)
            return;
        if (isServer)
        {
            int recHostId;
            int connectionId;
            int chanelId;

            byte[] recBuffer = new byte[BYTE_SIZE];
            int dataSize;

            // NetworkEventType type = NetworkTransport.Receive(out recHostId, out connectionId, out chanelId, recBuffer, recBuffer.Length, out dataSize, out error);
            NetworkEventType type = NetworkTransport.Receive(out recHostId, out connectionId, out chanelId, recBuffer, BYTE_SIZE, out dataSize, out error);

            switch (type)
            {
                case NetworkEventType.Nothing:
                    break;

                case NetworkEventType.ConnectEvent:
                    Debug.Log(string.Format("User {0} has connected has connected throught host {1}.", connectionId, recHostId));
                    //m_ConnectionHostIds.Add(connectionId, recHostId);
                    int newPlayerId = GetNextAvaliableId();
                    m_PlayersConnectionData.Add(newPlayerId, new PlayerConnectionData(connectionId, recHostId, chanelId, "", newPlayerId));
                    SetPlayerId(m_PlayersConnectionData[newPlayerId], newPlayerId);
                    PrintPlayersList();
                    UpdateLobby();
                    break;

                case NetworkEventType.DisconnectEvent:
                    Debug.Log(string.Format("User {0} has disconnected.", connectionId));
                    m_PlayersConnectionData.Remove(connectionId);
                    break;
                case NetworkEventType.DataEvent:
                    BinaryFormatter formatter = new BinaryFormatter();
                    MemoryStream ms = new MemoryStream(recBuffer);
                    NetMsg msg = (NetMsg)formatter.Deserialize(ms);

                    OnData(connectionId, chanelId, recHostId, msg);

                    break;
                default:
                case NetworkEventType.BroadcastEvent:
                    Debug.Log("Unexcepted...");
                    break;
            }
        }
        else
        {
            int recHostId;
            int connectionId;
            int chanelId;

            byte[] recBuffer = new byte[BYTE_SIZE];
            int dataSize;

            // NetworkEventType type = NetworkTransport.Receive(out recHostId, out connectionId, out chanelId, recBuffer, recBuffer.Length, out dataSize, out error);
            NetworkEventType type = NetworkTransport.Receive(out recHostId, out connectionId, out chanelId, recBuffer, BYTE_SIZE, out dataSize, out error);

            switch (type)
            {
                case NetworkEventType.Nothing:
                    break;

                case NetworkEventType.ConnectEvent:
                    Debug.Log(string.Format("We have connected to the server."));
                    break;

                case NetworkEventType.DisconnectEvent:
                    Debug.Log(string.Format("We have disconnected"));
                    break;
                case NetworkEventType.DataEvent:
                    Debug.Log(string.Format("DataEvent"));
                    BinaryFormatter formatter = new BinaryFormatter();
                    MemoryStream ms = new MemoryStream(recBuffer);
                    NetMsg msg = (NetMsg)formatter.Deserialize(ms);

                    OnData(connectionId, chanelId, recHostId, msg);
                    break;
                default:
                case NetworkEventType.BroadcastEvent:
                    Debug.Log("Unexcepted...");
                    break;
            }
        }

    }
    #endregion

    #region Send
    public void SendToServer(NetMsg msg)
    {
        Debug.Log(string.Format("Sent to server | host:{0} conId:{1}", m_ConnectionData.hostId, m_ConnectionData.connectionId));
        byte[] buffer = new byte[BYTE_SIZE];

        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg);

        NetworkTransport.Send(m_ConnectionData.hostId, m_ConnectionData.connectionId, reliableChannel, buffer, BYTE_SIZE, out error);

    }
    public void SendToClient(int recHost, int connId, NetMsg msg)
    {
        Debug.Log(string.Format("Send to client | host:{0} conId:{1}", recHost, connId));
        byte[] buffer = new byte[BYTE_SIZE];

        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg);
        if (recHost == 0)
        {
            NetworkTransport.Send(hostId, connId, reliableChannel, buffer, BYTE_SIZE, out error);
        }
        else
        {
            NetworkTransport.Send(webHostId, connId, reliableChannel, buffer, BYTE_SIZE, out error);
        }


    }
    public void SendToAllClients(NetMsg msg)
    {

        PrintPlayersList();

        foreach (KeyValuePair<int, PlayerConnectionData> p in m_PlayersConnectionData)
        {
            if (p.Value.playerId != m_playerId)
            {
                Debug.Log(string.Format(" Server send message to player: {0}#{1} | conn:{2} host:{3} chan:{4}",
               p.Key, p.Value.playerName, p.Value.connectionId, p.Value.hostId, p.Value.chanelId));
                SendToClient(p.Value.hostId, p.Value.connectionId, msg);
            }

        }

        if (m_PlayersConnectionData.ContainsKey(m_playerId))
        {
            Debug.Log("Server internal message");
            OnData(0, 0, 0, msg);
        }
    }
    #endregion

    public void OnMatchCreate()
    {
        CreateSerwer();
    }

    public void OnMatchJoined()
    {
        JoinToSerwer(serwerIp);
    }

    public void OnSendToClient()
    {
        //Net_PlayerData pd = new Net_PlayerData("toClient");
        //if (m_ConnectionHostIds.ContainsKey(1))
        //{
        //    SendToClient(m_ConnectionHostIds[1], 1, pd);
        //}

    }

    public void OnSendToClients()
    {
        //Net_PlayerData pd = new Net_PlayerData("toClients");
        UpdateLobby();
    }

    public void OnSendToServer()
    {
        //Net_PlayerData pd = new Net_PlayerData("toSERVER");
        //SendToServer(pd);
    }

    #region ClientAction
    public void CreateSerwer()
    {
        DontDestroyOnLoad(gameObject);
        isServer = true;
        
        InitConnection();
        AddHostedPlayer();
        
    }
    public void JoinToSerwer(string ip)
    {
        serwerIp = ip;
        DontDestroyOnLoad(gameObject);
        isServer = false;
        InitConnection();
        PrintPlayerData();
    }
    public void SetPlayerData(string playerName)
    {
        m_ConnectionData.playerName = playerName;
        PrintPlayerData();
        Net_PlayerData pd = new Net_PlayerData(m_playerId, playerName);
        SendToServer(pd);
    }
    #endregion

    #region ServerAction
    public void SetPlayerId(PlayerConnectionData player, int id)
    {
        Net_PlayerId pi = new Net_PlayerId(id);
        SendToClient(player.hostId, player.connectionId, pi);
    }
    public void UpdateLobby()
    {
        List<PlayerConnectionData> players = new List<PlayerConnectionData>();
        players.AddRange(m_PlayersConnectionData.Values);

        Net_UpdateLobby ul = new Net_UpdateLobby(players);
        SendToAllClients(ul);
    }
    int GetNextAvaliableId()
    {
        return m_nextAvaliablePlayerId++;
    }
    void AddHostedPlayer()
    {
        m_playerId = GetNextAvaliableId();
        m_ConnectionData.playerId = m_playerId;
        m_ConnectionData.playerName = "SERVERADMIN";
        m_ConnectionData.hostId = -1;
        m_ConnectionData.connectionId = -1;
        m_ConnectionData.chanelId = -1;

        m_PlayersConnectionData.Add(m_ConnectionData.playerId, m_ConnectionData);
    }
    #endregion


    #region OnData
    void OnData(int connectionId, int chanelId, int recHostId, NetMsg msg)
    {
        switch (msg.OperationCode)
        {
            case NetOP.NONE:
                Debug.Log("Unexpected NetOP");
                break;
            case NetOP.SET_PLAYER_DATA:
                OnSetPlayerData(connectionId, chanelId, recHostId, (Net_PlayerData)msg);
                break;
            case NetOP.SET_PLAYER_ID:
                OnSetPlayerId(connectionId, chanelId, recHostId, (Net_PlayerId)msg);
                break;
            case NetOP.UPDATE_LOBBY:
                OnUpdateLobby(connectionId, chanelId, recHostId, (Net_UpdateLobby)msg);
                break;
        }
    }

    #region SERVER
    void OnSetPlayerData(int connectionId, int chanelId, int recHostId, Net_PlayerData msg)
    {
        Debug.Log(string.Format("Set Player Data {0}", msg.playerName));
        m_PlayersConnectionData[msg.playerId].playerName = msg.playerName;
        UpdateLobby();
    }
    #endregion

    #region CLINET
    void OnSetPlayerId(int connectionId, int chanelId, int recHostId, Net_PlayerId msg)
    {
        Debug.Log(string.Format("Set Player id {0}", msg.playerId));
        m_playerId = msg.playerId;
        m_ConnectionData.playerId = msg.playerId;
        //PrintPlayerData();
    }
    void OnUpdateLobby(int connectionId, int chanelId, int recHostId, Net_UpdateLobby msg)
    {
        Debug.Log("OnUpdateLobby:");
        foreach (PlayerConnectionData p in msg.PlayersData)
        {
            Debug.Log(string.Format("player: {0}#{1} | conn:{2} host:{3} chan:{4}",
                p.playerId, p.playerName, p.connectionId, p.hostId, p.chanelId));

            m_PlayersConnectionData[p.playerId] = p;
        }
    }
    #endregion

    #endregion



    void PrintPlayersList()
    {
        Debug.Log("LOBBY:");
        foreach (KeyValuePair<int, PlayerConnectionData> p in m_PlayersConnectionData)
        {
            Debug.Log(string.Format("player: {0}#{1} | conn:{2} host:{3} chan:{4}",
                p.Key, p.Value.playerName, p.Value.connectionId, p.Value.hostId, p.Value.chanelId));
        }
    }

    void PrintPlayerData()
    {
        Debug.Log(string.Format("player: {0}#{1} | conn:{2} host:{3} chan:{4}",
            m_playerId, m_ConnectionData.playerName,
            m_ConnectionData.connectionId, m_ConnectionData.hostId, m_ConnectionData.chanelId));
    }
}

#region SERIALIZABLE
[System.Serializable]
public class PlayerConnectionData
{
    public int connectionId;
    public int hostId;
    public int chanelId;
    public string playerName;
    public int playerId;

    public PlayerConnectionData(int connectionId, int hostId, int chanelId, string playerName, int playerId)
    {
        this.connectionId = connectionId;
        this.hostId = hostId;
        this.chanelId = chanelId;
        this.playerName = playerName;
        this.playerId = playerId;
    }
}

public static class NetOP
{
    public const int NONE = 0;
    public const int SET_PLAYER_DATA = 1; // ATTR: NAME | send player name after connect
    public const int SET_PLAYER_ID = 2; // BTC | ATTR: playerId
    public const int UPDATE_LOBBY = 3; // BTC | ATTR: <playerName>

    public const int START_GAME_REQ = 1; // BTC |
    public const int START_GAME_CFM = 1;
    public const int SET_FRACTION_ID = 1; // ATTR: fractionId

    public const int DRAW_CARD_REQ = 1; // ATTR: playerId, fractionId
    public const int DRAW_CARD_CFM = 1; // ATTR: cardId, fractionId

    public const int UPDATE_PLAYERS_RESOURCES = 1; // BTC | ATTR: <playerId, <resources>, <resourcesGrowth>>

    // card action
    public const int BUILD_CARD_REQ = 1; // ATTR: playerId, cardId, fractionId
    public const int BUILD_CARD_CFM = 1; // BTC | ATTR: playerId, cardId, fractionId
    public const int BUILD_CARD_REJ = 1;

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
#endregion
