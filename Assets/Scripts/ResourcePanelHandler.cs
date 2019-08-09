using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePanelHandler : MonoBehaviour
{
    public Image resourceImage;
    public Text resourceCount;
    public Text resourceGrowth;

    public int resourceId = 0;
    public PlayerData playerData;

    void Start()
    {
        string path = Application.dataPath;
        resourceImage.sprite = IMG2Sprite.instance.LoadNewSprite(path + "/Sprites/Resources/" + resourceId + ".png");
    }

    void Update()
    {
        
    }

    public void UpdatePanelData()
    {
        if (playerData != null)
        {
            resourceCount.text = playerData.playerResources[resourceId].ToString();
            resourceGrowth.text = "+" + playerData.playerResourcesGrowth[resourceId].ToString();
        }
    }
}
