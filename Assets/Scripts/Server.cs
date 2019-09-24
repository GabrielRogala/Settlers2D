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
    private string _serwerIp = "127.0.0.1";

    private byte _reliableChannel;
    private byte _error;
    private bool _isStarted;
    private bool _isServer = false;

    #region ServerData
    private int _hostId = -1;
    private int _webHostId = -1;
    public Dictionary<int, PlayerConnectionData> _playersConnectionData = new Dictionary<int, PlayerConnectionData>();
    private List<int> _playersConfirmation = new List<int>();
    private int _nextAvaliablePlayerId = 1;
    private int _playerIdTurn = 0;
    #endregion

    #region ClientData
    private PlayerConnectionData _connectionData = new PlayerConnectionData(0, 0, 0, "", 0, 0);
    private int _playerId;
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
        _reliableChannel = cc.AddChannel(QosType.Reliable);

        HostTopology topo = new HostTopology(cc, MAX_USER);


        if (_isServer)
        {
            // SERVER ONLY
            _hostId = NetworkTransport.AddHost(topo, PORT, null);
            _webHostId = NetworkTransport.AddWebsocketHost(topo, WEB_PORT, null);

            Debug.Log(string.Format("Opening connection on port {0} and webport {1}", PORT, WEB_PORT));

            _isStarted = true;
        }
        else
        {
            // Client ONLY
            _connectionData.hostId = NetworkTransport.AddHost(topo, 0);
#if !UNITY_WEBGL
            // standalone client
            _connectionData.connectionId = NetworkTransport.Connect(_connectionData.hostId, _serwerIp, PORT, 0, out _error);
            Debug.Log(string.Format("Connecting from standalone"));
#else
            // web client
            m_ConnectionData.connectionId = NetworkTransport.Connect(m_ConnectionData.hostId, serwerIp, WEB_PORT, 0, out error);
            Debug.Log(string.Format("Connecting from web"));
#endif
            Debug.Log(string.Format("Attempting to connect on {0}....", _serwerIp));

            _isStarted = true;
        }
    }
    private void Shutdown()
    {
        _isStarted = false;
        NetworkTransport.Shutdown();
    }
    private void UpdateMessagePump()
    {
        if (!_isStarted)
            return;
        if (_isServer)
        {
            int recHostId;
            int connectionId;
            int chanelId;

            byte[] recBuffer = new byte[BYTE_SIZE];
            int dataSize;

            // NetworkEventType type = NetworkTransport.Receive(out recHostId, out connectionId, out chanelId, recBuffer, recBuffer.Length, out dataSize, out error);
            NetworkEventType type = NetworkTransport.Receive(out recHostId, out connectionId, out chanelId, recBuffer, BYTE_SIZE, out dataSize, out _error);

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
            NetworkEventType type = NetworkTransport.Receive(out recHostId, out connectionId, out chanelId, recBuffer, BYTE_SIZE, out dataSize, out _error);

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
        Debug.Log(string.Format("Sent to server | host:{0} conId:{1}", _connectionData.hostId, _connectionData.connectionId));
        byte[] buffer = new byte[BYTE_SIZE];

        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg);


        if (_isServer)
        {
            OnData(-1,-1,-1,msg);
        }
        else
        {
            NetworkTransport.Send(_connectionData.hostId, _connectionData.connectionId, _reliableChannel, buffer, BYTE_SIZE, out _error);
        }

    }
    public void SendToClient(int recHost, int connId, NetMsg msg)
    {
        Debug.Log(string.Format("Send to client | host:{0} conId:{1}", recHost, connId));
        byte[] buffer = new byte[BYTE_SIZE];

        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg);
        if (connId == -1) {
            OnData(-1, -1, -1, msg);
        }
        else
        { 
            if (recHost == 0)
            {
                NetworkTransport.Send(_hostId, connId, _reliableChannel, buffer, BYTE_SIZE, out _error);
            }
            else
            {
                NetworkTransport.Send(_webHostId, connId, _reliableChannel, buffer, BYTE_SIZE, out _error);
            }
        }
    }
    public void SendToAllClients(NetMsg msg)
    {

        foreach (KeyValuePair<int, PlayerConnectionData> p in _playersConnectionData)
        {
            if (p.Value.playerId != _connectionData.playerId)
            {
                Debug.Log(string.Format(" Server send message to player: {0}#{1} F:{5}| conn:{2} host:{3} chan:{4}",
                            p.Key, p.Value.playerName, p.Value.connectionId, p.Value.hostId, p.Value.chanelId, p.Value.fractionId));
                SendToClient(p.Value.hostId, p.Value.connectionId, msg);
            }

        }

        if (_playersConnectionData.ContainsKey(_connectionData.playerId))
        {
            Debug.Log("Server internal message");
            OnData(-1, -1, -1, msg);
        }
    }
    #endregion

    #region ClientAction
    public void CreateSerwer(string hostName)
    {
        DontDestroyOnLoad(gameObject);
        _isServer = true;
        
        InitConnection();
        AddHostedPlayer(hostName);
    }
    public void JoinToSerwer(string ip)
    {
        _serwerIp = ip;
        DontDestroyOnLoad(gameObject);
        _isServer = false;
        InitConnection();
    }
    public void SetPlayerData(string playerName)
    {
        _connectionData.playerName = playerName;
        Net_PlayerData pd = new Net_PlayerData(_playerId, playerName);
        SendToServer(pd);
    }
    public void RunGameScene()
    {
        List<PlayerData> players = new List<PlayerData>();
        foreach(KeyValuePair<int,PlayerConnectionData> p in _playersConnectionData)
        {
            players.Add(new PlayerData(p.Value.playerId, p.Value.playerName, p.Value.fractionId));
        }
        GameDataController.instance.InitGameData(players);
        SceneManager.LoadScene("Scenes/GameScene");
    } 
    public void DrawCard(int deckId){
        Net_DrawCardREQ msg = new Net_DrawCardREQ(_playerId,deckId);
        SendToServer(msg);
    }
    public void BuildCardREQ(int playerId, int deckId, int cardId)
    {
        Net_BuildCardREQ msg = new Net_BuildCardREQ(playerId, deckId, cardId);
        SendToServer(msg);
    }
    #endregion

    #region ServerAction
    private void OnConnectEvent(int connectionId,int recHostId, int chanelId) {
        int newPlayerId = GetNextAvaliableId();
        _playersConnectionData.Add(newPlayerId, new PlayerConnectionData(connectionId, recHostId, chanelId, "", newPlayerId, UnityEngine.Random.Range(1, 4)));
        SetPlayerId(_playersConnectionData[newPlayerId], newPlayerId);
        UpdateLobby();
    }
    private void OnDisconnectEvent(int connectionId) {
        _playersConnectionData.Remove(connectionId);
        UpdateLobby();
    }
    public void SetPlayerId(PlayerConnectionData player, int id)
    {
        Net_PlayerId pi = new Net_PlayerId(id);
        SendToClient(player.hostId, player.connectionId, pi);
    }
    private int GetRandomPlayerId(){
        List<int> keys = new List<int>();
        keys.AddRange(_playersConnectionData.Keys);
        int randVal = UnityEngine.Random.Range(0,keys.Count);
        return keys[randVal];
    }
    public void UpdateLobby()
    {
        List<PlayerConnectionData> players = new List<PlayerConnectionData>();
        players.AddRange(_playersConnectionData.Values);

        Net_UpdateLobby ul = new Net_UpdateLobby(players);
        SendToAllClients(ul);
    }
    int GetNextAvaliableId()
    {
        return _nextAvaliablePlayerId++;
    }
    void AddHostedPlayer(string hostname)
    {
        _playerId = GetNextAvaliableId();
        _connectionData.playerId = _playerId;
        _connectionData.playerName = hostname;
        _connectionData.hostId = -1;
        _connectionData.connectionId = -1;
        _connectionData.chanelId = -1;
        _connectionData.fractionId = UnityEngine.Random.Range(1, 4);

        _playersConnectionData.Add(_connectionData.playerId, _connectionData);
    }
    public void StartGame() {
        _playersConfirmation.Clear();
        _playersConfirmation.Add(_connectionData.playerId);

        List<PlayerConnectionData> players = new List<PlayerConnectionData>();
        players.AddRange(_playersConnectionData.Values);

        Net_StartGameREQ sg = new Net_StartGameREQ(players);
        SendToAllClients(sg);
    }
    bool IsAllPlayersConfirm()
    {
        foreach (KeyValuePair<int, PlayerConnectionData> p in _playersConnectionData)
        {
            if (!_playersConfirmation.Contains(p.Value.playerId))
            {
                return false;
            }

        }
        return true;
    }
    void SetPlayerIdTurn(int id){
        List<PlayerConnectionData> players = new List<PlayerConnectionData>();
        players.AddRange(_playersConnectionData.Values);

        Net_SetPlayerIdTurn ul = new Net_SetPlayerIdTurn(id);
        SendToAllClients(ul);
    }
    public void SetNextPlayerId(){
        _playerIdTurn = (_playerIdTurn+1)%_playersConnectionData.Count;
        SetPlayerIdTurn(_playersConnectionData[_playerIdTurn].playerId);
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
            case NetOP.BUILD_CARD_REQ:
                OnBuildCardREQ(connectionId, chanelId, recHostId, (Net_BuildCardREQ)msg);
                break;
            case NetOP.BUILD_CARD_CFM:
                OnBuildCardCFM(connectionId, chanelId, recHostId, (Net_BuildCardCFM)msg);
                break;
            case NetOP.BUILD_CARD_REJ:
                OnBuildCardREJ(connectionId, chanelId, recHostId, (Net_BuildCardREJ)msg);
                break;

        }
    }

    #region SERVER
    void OnSetPlayerData(int connectionId, int chanelId, int recHostId, Net_PlayerData msg)
    {
        Debug.Log(string.Format("Set Player Data {0}", msg.playerName));
        _playersConnectionData[msg.playerId].playerName = msg.playerName;
        UpdateLobby();
    }
    void OnStartGameCFM(int connectionId, int chanelId, int recHostId, Net_StartGameCFM msg)
    {
        if (!_playersConfirmation.Contains(msg.PlayerId))
        {
            _playersConfirmation.Add(msg.PlayerId);
        }

        if (IsAllPlayersConfirm()) {
            RunGameScene();
            _playerIdTurn = GetRandomPlayerId();
            SetPlayerIdTurn(_playersConnectionData[_playerIdTurn].playerId);
        }
        
    }
    void OnDrawCardREQ(int connectionId, int chanelId, int recHostId, Net_DrawCardREQ msg) {
        if (IsAbleToDraw(msg.PlayerId,msg.DeckId)) {
            int cardId = GetRandomCardFromDeck(msg.DeckId);
            Net_DrawCardCFM dc = new Net_DrawCardCFM(msg.PlayerId, msg.DeckId, cardId,GetDeckActualSize(msg.DeckId));
            //SendToClient(recHostId, connectionId, dc);
            SendToAllClients(dc);
        }
    }

    void OnBuildCardREQ(int connectionId, int chanelId, int recHostId, Net_BuildCardREQ msg)
    {
        if (GameController.instance.IsAbleToBuild(msg.PlayerId, msg.DeckId, msg.CardId))
        {
            Net_BuildCardCFM bc = new Net_BuildCardCFM(msg.PlayerId, msg.DeckId, msg.CardId);
            SendToAllClients(bc);
        } else {
            Net_BuildCardREJ bc = new Net_BuildCardREJ(msg.PlayerId, msg.DeckId, msg.CardId);
            SendToAllClients(bc);
        }
    }
    #endregion

    #region CLINET
    void OnMessage(int connectionId, int chanelId, int recHostId, Net_Message msg)
    {
        if (_isServer)
            return;
        Debug.Log(string.Format("MESSAGE {0}", msg.message));
    }
    void OnSetPlayerId(int connectionId, int chanelId, int recHostId, Net_PlayerId msg)
    {
        if (_isServer)
            return;
        Debug.Log(string.Format("Set Player id {0}", msg.playerId));
        _playerId = msg.playerId;
        _connectionData.playerId = msg.playerId;
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

            _playersConnectionData[p.playerId] = p;

            if (p.playerId == _connectionData.playerId) {
                _connectionData.fractionId = p.fractionId;
            }
        }

        MenuController.instance.UpdateLobby(_playersConnectionData);
    }
    void OnStartGameREQ(int connectionId, int chanelId, int recHostId, Net_StartGameREQ msg)
    {
        Net_StartGameCFM sg = new Net_StartGameCFM(_connectionData.playerId);
        if (_isServer){
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
        if (msg.PlayerId != _connectionData.playerId)
        {
            GameController.instance.UpdateDeckSize(msg.DeckId, msg.DeckSize);
        }
        else
        {
            GameController.instance.DrawCardCFM(msg.PlayerId, msg.DeckId, msg.CardId, msg.DeckSize);
        }
    }

    void OnBuildCardCFM(int connectionId, int chanelId, int recHostId, Net_BuildCardCFM msg)
    {
        Debug.Log("OnBuildCardCFM");
        GameController.instance.BuildCardCFM(msg.PlayerId, msg.DeckId, msg.CardId);
    }

    void OnBuildCardREJ(int connectionId, int chanelId, int recHostId, Net_BuildCardREJ msg)
    {
        Debug.Log("OnBuildCardREJ");
    }

    #endregion

    #endregion



    public int GetOwnPlayerId() {
        return _connectionData.playerId;
    }

    public int GetOwnFractionId()
    {
        return _connectionData.fractionId;
    }

}