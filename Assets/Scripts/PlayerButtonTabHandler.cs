using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerButtonTabHandler : MonoBehaviour
{
    public GameObject m_tabContent;
    public Text m_tabPanelLabel;
    public static List<GameObject> m_contentList = new List<GameObject>();

    public void ShowTabContent()
    {
        foreach (GameObject c in m_contentList)
        {
            c.SetActive(false);
        }
        m_tabContent.SetActive(true);

    }

}
