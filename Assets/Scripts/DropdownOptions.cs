using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownOptions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Dropdown d = this.GetComponent<Dropdown>();
        List<Dropdown.OptionData> dropdownOptionsList = new List<Dropdown.OptionData>();

        List<FractionData> fractions = DataHandler.instance.LoadGameData().fractions;

        foreach(FractionData f in fractions){
            Dropdown.OptionData option = new Dropdown.OptionData(f.fractionName);
            dropdownOptionsList.Add(option);
        }
        d.AddOptions(dropdownOptionsList);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
