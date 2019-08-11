using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHandTabController : MonoBehaviour
{
    public GameObject _content;
    public Text _tabLabel;
    public static List<GameObject> _contentList = new List<GameObject>();

    public void ShowTabContent()
    {
        foreach (GameObject c in _contentList)
        {
            c.SetActive(false);
        }
        _content.SetActive(true);
        //GameHandler.instance.PlayersResourcesUpdate();
    }
}
