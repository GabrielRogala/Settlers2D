using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardHandler : MonoBehaviour
{
    public int actionType = 0;
    public int fractionId = 0;
    public int playerId = 0;
    public bool usedCard = false;

    public Text actionTypeTEXT;
    public Text fractionIdTEXT;
    public Text playerIdTEXT;

    // Start is called before the first frame update
    void Start()
    {
        actionTypeTEXT.text = "Action "+ actionType;
        fractionIdTEXT.text = "Fraction " + fractionId;
        playerIdTEXT.text = "P:" + playerId;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
