using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FullSizeCardHandler : MonoBehaviour, IPointerClickHandler
{

    public CardHandler m_card;
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

    public FullSizeCardHandler(CardHandler card)
    {
        m_card = card;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CardViewer.instance.HideFullSizeCard();
    }

    // Start is called before the first frame update
    void Start()
    {
        string path = Application.dataPath;

        m_id.text = "#" + m_card.m_card.cardId;
        m_name.text = m_card.m_card.cardName;
        m_description.text = m_card.m_card.description;
        m_fractionImage.sprite = IMG2Sprite.instance.LoadNewSprite(path + "/Sprites/Fractions/" + m_card.m_card.fractionType + ".png");
        m_cardType.sprite = IMG2Sprite.instance.LoadNewSprite(path + "/Sprites/Types/" + m_card.m_card.cardType + ".png");
        m_contract.sprite = IMG2Sprite.instance.LoadNewSprite(path + "/Sprites/Resources/" + m_card.m_card.contract + ".png");

        for (int i = 0; i < m_costs.Count; i++)
        {
            m_costs[i].gameObject.SetActive(false);
            m_costs[i].sprite = IMG2Sprite.instance.LoadNewSprite(path + "/Sprites/Resources/0.png");
        }
        for (int i = 0; i < m_gains.Count; i++)
        {
            m_gains[i].gameObject.SetActive(false);
            m_gains[i].sprite = IMG2Sprite.instance.LoadNewSprite(path + "/Sprites/Resources/0.png");
        }

        for (int i = 0; i < m_card.m_card.cost.Count; i++)
        {
            m_costs[i].gameObject.SetActive(true);
            m_costs[i].sprite = IMG2Sprite.instance.LoadNewSprite(path + "/Sprites/Resources/" + m_card.m_card.cost[i] + ".png");
        }
        for (int i = 0; i < m_card.m_card.gain.Count; i++)
        {
            m_gains[i].gameObject.SetActive(true);
            m_gains[i].sprite = IMG2Sprite.instance.LoadNewSprite(path + "/Sprites/Resources/" + m_card.m_card.gain[i] + ".png");
        }

        m_image.sprite = IMG2Sprite.instance.LoadNewSprite(path + "/Sprites/Screens/" + m_card.m_card.image + ".png");

        if(m_card.m_card.fractionType == 0)
        {
            m_contractBackground.color = Color.clear;
        }
    }

    // HAND

    public void BuildCard() {
        Debug.Log("BuildCard");
    }

    public void AddToContractsCard()
    {
        Debug.Log("BuildCard");
    }

    public void PlunderCard()
    {
        Debug.Log("PlunderCard");
    }

    // Board
    public void DestroyCard()
    {
        Debug.Log("DestroyCard");
    }

    public void TributeCard()
    {
        Debug.Log("TributeCard");
    }
}
