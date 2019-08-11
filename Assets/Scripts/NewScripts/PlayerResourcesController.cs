using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResourcesController : MonoBehaviour
{
    // UI
    public GameObject _resourceContainer;
    public GameObject _resourcePanelPrefab;

    public void InitResourcesPanel(PlayerData playerData)
    {
        List<ResourcesData> resources = GameDataController.instance.gameData.resources;

        foreach (ResourcesData r in resources)
        {
            if (r.resourcesId > 0)
            {
                GameObject resource = Instantiate(_resourcePanelPrefab, _resourceContainer.transform) as GameObject;
                resource.GetComponent<ResourcePanelController>().InitResourcePanel(playerData, r.resourcesId);
            }
        }
    }
}
