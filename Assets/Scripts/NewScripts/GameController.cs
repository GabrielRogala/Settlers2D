using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //////////////////////////////////////////
    private static GameController _instance;
    public static GameController instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<GameController>();
            return _instance;
        }
    }
    //////////////////////////////////////////

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    
}
