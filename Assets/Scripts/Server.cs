using System;
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
    private int connectionId;
    private int hostId;
    private int webHostId;
    private byte error;

    private bool isStarted;
    private bool isServer = false;

    private List<string> playerList;

    #region MonoBehaviour
    void Start()
    {
        playerList = new List<string>();
    }
    void Update()
    {
        UpdateMessagePump();
    }
    #endregion

    #region ClientAction
    public void CreateSerwer()
    {
        DontDestroyOnLoad(gameObject);
        isServer = true;
        InitConnection();
    }

    public void JoinToSerwer(string ip)
    {
        serwerIp = ip;
        DontDestroyOnLoad(gameObject);
        isServer = false;
        InitConnection();
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
            hostId = NetworkTransport.AddHost(topo, 0);

#if !UNITY_WEBGL
            // standalone client
            connectionId = NetworkTransport.Connect(hostId, serwerIp, PORT, 0, out error);
            Debug.Log(string.Format("Connecting from standalone"));
#else
            // web client
            connectionId = NetworkTransport.Connect(hostId, serwerIp, WEB_PORT, 0, out error);
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
                    break;

                case NetworkEventType.DisconnectEvent:
                    Debug.Log(string.Format("User {0} has disconnected.", connectionId));
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

    #region OnData
    private void OnData(int connectionId, int chanelId, int recHostId, NetMsg msg)
    {
        switch (msg.OperationCode)
        {
            case NetOP.None:
                Debug.Log("Unexpected NetOP");
                break;
            case NetOP.SetPlayerData:
                OnSetPlayerData(connectionId, chanelId, recHostId, (Net_PlayerData)msg);
                break;
        }
    }

    private void OnSetPlayerData(int connectionId, int chanelId, int recHostId, Net_PlayerData msg)
    {
        Debug.Log(string.Format("Set Player Data {0}", msg.PlayerName));
        AddPlayer(msg.PlayerName);
    }

    #endregion

    private void AddPlayer(string playerName)
    {
        playerList.Add(playerName);
        foreach(string s in playerList)
        {
            Debug.Log(s);
        }
    }

    #region Send
    public void SendServer(NetMsg msg)
    {
        byte[] buffer = new byte[BYTE_SIZE];

        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg);

        NetworkTransport.Send(hostId, connectionId, reliableChannel, buffer, BYTE_SIZE, out error);

    }

    public void SendClient(int recHost, int connId, NetMsg msg)
    {
        byte[] buffer = new byte[BYTE_SIZE];

        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg);
        if(recHost == 0)
        {
            NetworkTransport.Send(hostId, connId, reliableChannel, buffer, BYTE_SIZE, out error);
        }
        else
        {
            NetworkTransport.Send(webHostId, connId, reliableChannel, buffer, BYTE_SIZE, out error);
        }
        

    }

    public void SetPlayerData(string playerName)
    {
        if (isServer) {
            AddPlayer(playerName);
        }
        else
        {
            Net_PlayerData pd = new Net_PlayerData(playerName);
            SendServer(pd);
        }
    }
    #endregion
}

public static class NetOP
{
    public const int None = 0;
    public const int SetPlayerData = 1;
}

[System.Serializable]
public class NetMsg
{
    public byte OperationCode;

    public NetMsg()
    {
        OperationCode = NetOP.None;
    }
}

[System.Serializable]
public class Net_PlayerData : NetMsg
{
    public Net_PlayerData(string name)
    {
        OperationCode = NetOP.SetPlayerData;
        PlayerName = name;
    }

    public string PlayerName;

}
