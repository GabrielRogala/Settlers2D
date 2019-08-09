using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardHandler : MonoBehaviour
{
    public int m_actionType = 0;
    public int m_fractionId = 0;
    public int m_playerId = 0;
    public bool m_usedCard = false;

    public CardData m_card = new CardData();
    public Text m_id;
    public Text m_name;
    public Text m_description;
    public Image m_cardType;
    public Image m_fractionImage;
    public Image m_contract;
    public Image m_contractBackground;
    public List<Image> m_costs;
    public List<Image> m_gains;
    public Image m_image;

    public CardHandler(CardData card)
    {
        m_card = card;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_actionType = m_card.actionType;
        m_fractionId = m_card.fractionType;
        //m_playerId = 0;

        string path = Application.dataPath;

        m_id.text = "#" + m_card.cardId;
        m_name.text = m_card.cardName;
        m_description.text = m_card.description;
        m_fractionImage.sprite = IMG2Sprite.instance.LoadNewSprite(path + "/Sprites/Fractions/" + m_card.fractionType + ".png");
        m_cardType.sprite = IMG2Sprite.instance.LoadNewSprite(path + "/Sprites/Types/" + m_card.cardType + ".png");
        m_contract.sprite = IMG2Sprite.instance.LoadNewSprite(path + "/Sprites/Resources/" + m_card.contract + ".png");

        for (int i = 0; i < m_costs.Count; i++)
        {
            m_costs[i].sprite = IMG2Sprite.instance.LoadNewSprite(path + "/Sprites/Resources/0.png");
        }
        for (int i = 0; i < m_gains.Count; i++)
        {
            m_gains[i].sprite = IMG2Sprite.instance.LoadNewSprite(path + "/Sprites/Resources/0.png");
        }

        for (int i = 0; i < m_card.cost.Count; i++)
        {
            m_costs[i].sprite = IMG2Sprite.instance.LoadNewSprite(path + "/Sprites/Resources/" + m_card.cost[i] + ".png");
        }
        for (int i = 0; i < m_card.gain.Count; i++)
        {
            m_gains[i].sprite = IMG2Sprite.instance.LoadNewSprite(path + "/Sprites/Resources/" + m_card.gain[i] + ".png");
        }

        m_image.sprite = IMG2Sprite.instance.LoadNewSprite(path + "/Sprites/Screens/" + m_card.image + ".png");

        if (m_card.fractionType == 0)
        {
            m_contractBackground.color = Color.clear;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildActionExecute()
    {
        Debug.Log("BuildAction : " +" | "+m_playerId +" | " + m_card.ToString());
    }

    public void ProductionActionExecute()
    {
        Debug.Log("ProductionAction : " + m_card.ToString());
    }

    public void ActionExecute()
    {
        Debug.Log("Action : " + m_card.ToString());
    }

    public void TraitActionExecute()
    {
        Debug.Log("TraitActionExecute : " + m_card.ToString());
    }
}
