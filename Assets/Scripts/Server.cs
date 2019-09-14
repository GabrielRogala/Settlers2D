using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine.SceneManagement;

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
    public Dictionary<int, PlayerConnectionData> m_PlayersConnectionData = new Dictionary<int, PlayerConnectionData>();
    private List<int> m_PlayersConfirmation = new List<int>();
    private int m_nextAvaliablePlayerId = 1;
    private int m_playerIdTurn = 0;
    #endregion

    #region ClientData
    private PlayerConnectionData m_ConnectionData = new PlayerConnectionData(0, 0, 0, "", 0, 0);
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
                    OnConnectEvent(connectionId, recHostId, chanelId);
                    break;

                case NetworkEventType.DisconnectEvent:
                    Debug.Log(string.Format("User {0} has disconnected.", connectionId));
                    OnDisconnectEvent(connectionId);
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

        foreach (KeyValuePair<int, PlayerConnectionData> p in m_PlayersConnectionData)
        {
            if (p.Value.playerId != m_ConnectionData.playerId)
            {
                Debug.Log(string.Format(" Server send message to player: {0}#{1} F:{5}| conn:{2} host:{3} chan:{4}",
                            p.Key, p.Value.playerName, p.Value.connectionId, p.Value.hostId, p.Value.chanelId, p.Value.fractionId));
                SendToClient(p.Value.hostId, p.Value.connectionId, msg);
            }

        }

        if (m_PlayersConnectionData.ContainsKey(m_ConnectionData.playerId))
        {
            Debug.Log("Server internal message");
            OnData(0, 0, 0, msg);
        }
    }
    #endregion

    #region ClientAction
    public void CreateSerwer(string hostName)
    {
        DontDestroyOnLoad(gameObject);
        isServer = true;
        
        InitConnection();
        AddHostedPlayer(hostName);
        
    }
    public void JoinToSerwer(string ip)
    {
        serwerIp = ip;
        DontDestroyOnLoad(gameObject);
        isServer = false;
        InitConnection();
    }
    public void SetPlayerData(string playerName)
    {
        m_ConnectionData.playerName = playerName;
        Net_PlayerData pd = new Net_PlayerData(m_playerId, playerName);
        SendToServer(pd);
    }
    public void RunGameScene()
    {
        List<PlayerData> players = new List<PlayerData>();
        foreach(KeyValuePair<int,PlayerConnectionData> p in m_PlayersConnectionData)
        {
            players.Add(new PlayerData(p.Value.playerId, p.Value.playerName, p.Value.fractionId));
        }
        GameDataController.instance.InitGameData(players);
        SceneManager.LoadScene("Scenes/GameScene");
    }
    
    public void DrawCard(int deckId){
        Net_DrawCardREQ msg = new Net_DrawCardREQ(m_playerId,deckId);
        SendToServer(msg);
    }
    #endregion

    #region ServerAction
    private void OnConnectEvent(int connectionId,int recHostId, int chanelId) {
        int newPlayerId = GetNextAvaliableId();
        m_PlayersConnectionData.Add(newPlayerId, new PlayerConnectionData(connectionId, recHostId, chanelId, "", newPlayerId, UnityEngine.Random.Range(1, 4)));
        SetPlayerId(m_PlayersConnectionData[newPlayerId], newPlayerId);
        UpdateLobby();
    }
    private void OnDisconnectEvent(int connectionId) {
        m_PlayersConnectionData.Remove(connectionId);
        UpdateLobby();
    }
    public void SetPlayerId(PlayerConnectionData player, int id)
    {
        Net_PlayerId pi = new Net_PlayerId(id);
        SendToClient(player.hostId, player.connectionId, pi);
    }
    private int GetRandomPlayerId(){
        List<int> keys = new List<int>();
        keys.AddRange(m_PlayersConnectionData.Keys);
        int randVal = UnityEngine.Random.Range(0,keys.Count);
        return keys[randVal];

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
    void AddHostedPlayer(string hostname)
    {
        m_playerId = GetNextAvaliableId();
        m_ConnectionData.playerId = m_playerId;
        m_ConnectionData.playerName = hostname;
        m_ConnectionData.hostId = -1;
        m_ConnectionData.connectionId = -1;
        m_ConnectionData.chanelId = -1;
        m_ConnectionData.fractionId = UnityEngine.Random.Range(1, 4);

        m_PlayersConnectionData.Add(m_ConnectionData.playerId, m_ConnectionData);
    }
    public void StartGame() {
        m_PlayersConfirmation.Clear();
        m_PlayersConfirmation.Add(m_ConnectionData.playerId);

        List<PlayerConnectionData> players = new List<PlayerConnectionData>();
        players.AddRange(m_PlayersConnectionData.Values);

        Net_StartGameREQ sg = new Net_StartGameREQ(players);
        SendToAllClients(sg);
    }
    bool IsAllPlayersConfirm()
    {
        foreach (KeyValuePair<int, PlayerConnectionData> p in m_PlayersConnectionData)
        {
            if (!m_PlayersConfirmation.Contains(p.Value.playerId))
            {
                return false;
            }

        }
        return true;
    }
    void SetPlayerIdTurn(int id){
        List<PlayerConnectionData> players = new List<PlayerConnectionData>();
        players.AddRange(m_PlayersConnectionData.Values);

        Net_SetPlayerIdTurn ul = new Net_SetPlayerIdTurn(id);
        SendToAllClients(ul);
    }
    public void SetNextPlayerId(){
        m_playerIdTurn = (m_playerIdTurn+1)%m_PlayersConnectionData.Count;
        SetPlayerIdTurn(m_PlayersConnectionData[m_playerIdTurn].playerId);
    }
    
    bool IsAbleToDraw(int playerId, int deckId) {
        return true;
    }

    int GetRandomCardFromDeck(int deckId) {
        return GameController.instance.GetRandomCardFromDeck(deckId);
    }

    int GetDeckActualSize(int deckId) {
        return GameController.instance._deckManager[deckId]._availableCardsIdList.Count;
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
            case NetOP.MESSAGE:
                OnMessage(connectionId, chanelId, recHostId, (Net_Message)msg);
                break;
            case NetOP.START_GAME_REQ:
                OnStartGameREQ(connectionId, chanelId, recHostId, (Net_StartGameREQ)msg);
                break;
            case NetOP.START_GAME_CFM:
                OnStartGameCFM(connectionId, chanelId, recHostId, (Net_StartGameCFM)msg);
                break;
            case NetOP.SET_PLAYER_ID_TURN:
                OnSetPlayerIdTurn(connectionId, chanelId, recHostId, (Net_SetPlayerIdTurn)msg);
                break;
            case NetOP.DRAW_CARD_REQ:
                OnDrawCardREQ(connectionId, chanelId, recHostId, (Net_DrawCardREQ)msg);
                break;
            case NetOP.DRAW_CARD_CFM:
                OnDrawCardCFM(connectionId, chanelId, recHostId, (Net_DrawCardCFM)msg);
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
    void OnStartGameCFM(int connectionId, int chanelId, int recHostId, Net_StartGameCFM msg)
    {
        if (!m_PlayersConfirmation.Contains(msg.PlayerId))
        {
            m_PlayersConfirmation.Add(msg.PlayerId);
        }

        if (IsAllPlayersConfirm()) {
            RunGameScene();
            m_playerIdTurn = GetRandomPlayerId();
            SetPlayerIdTurn(m_PlayersConnectionData[m_playerIdTurn].playerId);
        }
        
    }
    void OnDrawCardREQ(int connectionId, int chanelId, int recHostId, Net_DrawCardREQ msg) {
        if (IsAbleToDraw(msg.PlayerId,msg.DeckId)) {
            int cardId = GetRandomCardFromDeck(msg.DeckId);
            Net_DrawCardCFM dc = new Net_DrawCardCFM(msg.PlayerId, msg.DeckId, cardId,GetDeckActualSize(msg.DeckId));
            SendToClient(recHostId, connectionId, dc);
        }
    }
    #endregion

    #region CLINET
    void OnMessage(int connectionId, int chanelId, int recHostId, Net_Message msg)
    {
        if (isServer)
            return;
        Debug.Log(string.Format("MESSAGE {0}", msg.message));
    }
    void OnSetPlayerId(int connectionId, int chanelId, int recHostId, Net_PlayerId msg)
    {
        if (isServer)
            return;
        Debug.Log(string.Format("Set Player id {0}", msg.playerId));
        m_playerId = msg.playerId;
        m_ConnectionData.playerId = msg.playerId;
        //PrintPlayerData();
    }
    void OnUpdateLobby(int connectionId, int chanelId, int recHostId, Net_UpdateLobby msg)
    {
        //if (isServer)
        //    return;
        //Debug.Log("OnUpdateLobby:");
        foreach (PlayerConnectionData p in msg.PlayersData)
        {
            //Debug.Log(string.Format("player: {0}#{1} | conn:{2} host:{3} chan:{4}",
            //    p.playerId, p.playerName, p.connectionId, p.hostId, p.chanelId));

            m_PlayersConnectionData[p.playerId] = p;

            if (p.playerId == m_ConnectionData.playerId) {
                m_ConnectionData.fractionId = p.fractionId;
            }
        }

        MenuController.instance.UpdateLobby(m_PlayersConnectionData);
    }
    void OnStartGameREQ(int connectionId, int chanelId, int recHostId, Net_StartGameREQ msg)
    {
        Net_StartGameCFM sg = new Net_StartGameCFM(m_ConnectionData.playerId);
        if (isServer){
            OnStartGameCFM(0,0,0,sg);
            return;
        }
        SendToServer(sg);
        RunGameScene();
    }

    void OnSetPlayerIdTurn(int connectionId, int chanelId, int recHostId, Net_SetPlayerIdTurn msg){

        GameController.SetPlayerIdTurn(msg.PlayerId);
    }
    void OnDrawCardCFM(int connectionId, int chanelId, int recHostId, Net_DrawCardCFM msg) { 
        Debug.Log(string.Format("P{0} D{1} C{2} S{3}",msg.PlayerId,msg.DeckId,msg.CardId,msg.DeckSize));
    }

    #endregion

    #endregion



    public int GetOwnPlayerId() {
        return m_ConnectionData.playerId;
    }

    public int GetOwnFractionId()
    {
        return m_ConnectionData.fractionId;
    }

}