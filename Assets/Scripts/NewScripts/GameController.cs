using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // UI
    public GameObject _playerBoardPanelPrefab;
    public GameObject _playerHandPanelPrefab;
    public GameObject _playerBoardPanelContainer;
    public GameObject _playerHandPanelContainer;
    public ToggleGroup _playerBoardToggleGroup;

    public GameObject _playerBoardTabPrefab;
    public GameObject _playerHandTabPrefab;
    public GameObject _playerBoardTabContainer;
    public GameObject _playerHandTabContainer;
    public ToggleGroup _playerHandToggleGroup;

    public List<PlayerController> _playerControllers;
    public List<DeckController> _deckControllers;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("GAME CONTROLLER START");
        ///////////////////////////////////
        List<PlayerData> players;
        players = new List<PlayerData>();
        players.Add(new PlayerData(1, "name1", 1));
        players.Add(new PlayerData(2, "name2", 2));
        players.Add(new PlayerData(3, "name3", 3));
        GameDataController.instance.InitGameData(players);
        //////////////////////////////////



        InitPlayersData(GameDataController.instance.gameState.players);
        CreatePlayersPanel(GameDataController.instance.gameState.players);


    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitPlayersData(List<PlayerData> players)
    {
        Debug.Log("InitPlayersData");
        foreach (PlayerData p in players)
        {
            var resources = GameDataController.instance.gameData.fractions[p.fractionId].resourceGrowthMatrix;
            int i = 0;
            foreach (int r in resources)
            {
                //p.playerResourcesGrowth.Add(i, r);
                p.playerResourcesGrowth.Add(i, 10);
                p.playerResources.Add(i, 0);
                i++;
            }
        }
        Debug.Log(GameDataController.instance.gameState);
    }

    void CreatePlayersPanel(List<PlayerData> players)
    {
        PlayerBoardTabController._contentList = new List<GameObject>();
        PlayerHandTabController._contentList = new List<GameObject>();

        foreach (PlayerData p in players)
        {
            GameObject boardTab = Instantiate(_playerBoardTabPrefab, _playerBoardTabContainer.transform) as GameObject;
            GameObject boardContent = Instantiate(_playerBoardPanelPrefab, _playerBoardPanelContainer.transform) as GameObject;
            GameObject handTab = Instantiate(_playerHandTabPrefab, _playerHandTabContainer.transform) as GameObject;
            GameObject handContent = Instantiate(_playerHandPanelPrefab, _playerHandPanelContainer.transform) as GameObject;

            //boardContent
            boardContent.GetComponent<PlayerBoardPanelController>().InitBoardPanel(p);

            //handContent
            handContent.GetComponent<PlayerHandPanelController>().InitPlayerDecks(null, null, null);

            //boardTab
            boardTab.GetComponent<PlayerBoardTabController>()._tabLabel.text = p.name;
            boardTab.GetComponent<PlayerBoardTabController>()._content = boardContent;
            PlayerBoardTabController._contentList.Add(boardContent);

            // handTab
            handTab.GetComponent<PlayerHandTabController>()._tabLabel.text = p.name;
            handTab.GetComponent<PlayerHandTabController>()._content = handContent;
            PlayerHandTabController._contentList.Add(handContent);

            /////////////////////////////////////
            boardTab.GetComponent<Toggle>().group = _playerBoardToggleGroup;
            boardTab.GetComponent<Toggle>().isOn = false;
            handTab.GetComponent<Toggle>().group = _playerHandToggleGroup;
            handTab.GetComponent<Toggle>().isOn = false;

            boardContent.SetActive(true);
            handContent.SetActive(true);

        }

        foreach (PlayerHandTabController t in _playerBoardTabContainer.GetComponentsInChildren<PlayerHandTabController>())
        {
            t.GetComponent<Toggle>().isOn = true;
            t.ShowTabContent();
            break;
        }

        foreach (PlayerBoardTabController t in _playerHandTabContainer.GetComponentsInChildren<PlayerBoardTabController>())
        {
            t.GetComponent<Toggle>().isOn = true;
            t.ShowTabContent();
            break;
        }
    }

    void CreatePlayerController(List<PlayerData> players)
    {
        foreach (PlayerData p in players)
        {
            
        }
    }
}
