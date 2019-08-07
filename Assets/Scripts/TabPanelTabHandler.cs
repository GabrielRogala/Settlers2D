using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabPanelTabHandler : MonoBehaviour
{
    public GameObject m_tabContent;
    public Text m_tabPanelLabel;
    public static List<GameObject> m_contentList = new List<GameObject>();

    public void ShowTabContent()
    {
        foreach(GameObject c in m_contentList)
        {
            c.SetActive(false);
        }
        m_tabContent.SetActive(true);

    }

    void Start()
    {
        
    }

    //public static void AddContent(GameObject contents)
    //{
    //    m_contentList.Add(contents);
    //} 

    public override string ToString()
    {
        return m_tabContent.name + " : " + m_tabPanelLabel.text;
    }
}
