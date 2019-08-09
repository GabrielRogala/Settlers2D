﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTabPanelHandler : MonoBehaviour
{
    public GameObject m_playerInfoPanel;
    public GameObject m_resourcePanelPrefab;
    public PlayerData m_palayerData;

    public void InitResourcesPanel(PlayerData playerData)
    {
        Debug.Log("InitResourcesPanel "+ playerData.ToString());
        this.m_palayerData = playerData;
        List<ResourcesData> resources = GameDataHandler.instance.gameData.resources;

        foreach (ResourcesData r in resources)
        {
            if (r.resourcesId > 0)
            {
                GameObject resource = Instantiate(m_resourcePanelPrefab, m_playerInfoPanel.transform) as GameObject;
                resource.GetComponent<ResourcePanelHandler>().resourceId = r.resourcesId;
                resource.GetComponent<ResourcePanelHandler>().playerData = this.m_palayerData;
            }
        }
    }
}
