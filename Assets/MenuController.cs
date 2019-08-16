using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public InputField playerName;
    public InputField serwerIp;

    public void OnStartSerwer()
    {
        Server.instance.SetPlayerData(playerName.text);
    }

    public void OnCreateSerwer()
    {
        Server.instance.CreateSerwer();
    }

    public void OnJoinToSerwer()
    {
        Server.instance.JoinToSerwer(serwerIp.text);
        Server.instance.SetPlayerData(playerName.text);
    }


}
