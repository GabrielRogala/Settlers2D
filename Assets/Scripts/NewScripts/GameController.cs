using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    //////////////////////////////////////////
    private static GameController _instance;
    public static GameController instance {
        get {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<GameController> ();
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
    void Start () {
        _deckControllers = new List<DeckController> ();
        _playerControllers = new List<PlayerController> ();

        Debug.Log ("GAME CONTROLLER START");
        ///////////////////////////////////
        List<PlayerData> players;
        players = new List<PlayerData> ();
        players.Add (new PlayerData (1, "name1", 1));
        players.Add (new PlayerData (2, "name2", 2));
        players.Add (new PlayerData (3, "name3", 3));
        GameDataController.instance.InitGameData (players);
        //////////////////////////////////

        InitDecks (GameDataController.instance.gameData.decks);
        InitPlayersData (GameDataController.instance.gameState.players);
        CreatePlayersPanel (GameDataController.instance.gameState.players);

    }

    // Update is called once per frame
    void Update () {

    }

    void InitDecks (List<DeckData> decks) {
        foreach (DeckData d in decks) {
            _deckControllers.Add (new DeckController (d));
        }
    }

    void InitPlayersData (List<PlayerData> players) {
        Debug.Log ("InitPlayersData");
        foreach (PlayerData p in players) {
            var resources = GameDataController.instance.gameData.fractions[p.fractionId].resourceGrowthMatrix;
            int i = 0;
            foreach (int r in resources) {
                //p.playerResourcesGrowth.Add(i, r);
                p.playerResourcesGrowth.Add (i, 10);
                p.playerResources.Add (i, 0);
                i++;
            }
        }
        Debug.Log (GameDataController.instance.gameState);
    }

    DeckController GetDeckControllerFromFractionId (int fractionId) {
        foreach (DeckController d in _deckControllers) {
            if (d._deckData.fractionId == fractionId) {
                return d;
            }
        }
        return null;
    }

    void CreatePlayersPanel (List<PlayerData> players) {
        PlayerBoardTabController._contentList = new List<GameObject> ();
        PlayerHandTabController._contentList = new List<GameObject> ();

        foreach (PlayerData p in players) {
            PlayerController playerController = new PlayerController (p);
            _playerControllers.Add (playerController);

            GameObject boardTab = Instantiate (_playerBoardTabPrefab, _playerBoardTabContainer.transform) as GameObject;
            GameObject boardContent = Instantiate (_playerBoardPanelPrefab, _playerBoardPanelContainer.transform) as GameObject;
            GameObject handTab = Instantiate (_playerHandTabPrefab, _playerHandTabContainer.transform) as GameObject;
            GameObject handContent = Instantiate (_playerHandPanelPrefab, _playerHandPanelContainer.transform) as GameObject;

            playerController._playerBoardPanel = boardContent;
            playerController._playerHandPanel = handContent;

            //boardContent
            boardContent.GetComponent<PlayerBoardPanelController> ().InitBoardPanel (playerController);

            //handContent
            handContent.GetComponent<PlayerHandPanelController> ().InitPlayerDecks (playerController,
                GetDeckControllerFromFractionId (0),
                GetDeckControllerFromFractionId (p.fractionId));

            //boardTab
            boardTab.GetComponent<PlayerBoardTabController> ()._tabLabel.text = p.name;
            boardTab.GetComponent<PlayerBoardTabController> ()._content = boardContent;
            PlayerBoardTabController._contentList.Add (boardContent);

            // handTab
            handTab.GetComponent<PlayerHandTabController> ()._tabLabel.text = p.name;
            handTab.GetComponent<PlayerHandTabController> ()._content = handContent;
            PlayerHandTabController._contentList.Add (handContent);

            /////////////////////////////////////
            boardTab.GetComponent<Toggle> ().group = _playerBoardToggleGroup;
            boardTab.GetComponent<Toggle> ().isOn = false;
            handTab.GetComponent<Toggle> ().group = _playerHandToggleGroup;
            handTab.GetComponent<Toggle> ().isOn = false;

            boardContent.SetActive (true);
            handContent.SetActive (true);

        }

        foreach (PlayerHandTabController t in _playerBoardTabContainer.GetComponentsInChildren<PlayerHandTabController> ()) {
            t.GetComponent<Toggle> ().isOn = true;
            t.ShowTabContent ();
            break;
        }

        foreach (PlayerBoardTabController t in _playerHandTabContainer.GetComponentsInChildren<PlayerBoardTabController> ()) {
            t.GetComponent<Toggle> ().isOn = true;
            t.ShowTabContent ();
            break;
        }
    }

    public void UpdateDecksCounter () {
        foreach (PlayerController p in _playerControllers) {
            p._playerHandPanel.GetComponent<PlayerHandPanelController> ()._defaultDeck.UpdateDeckCounter ();
            p._playerHandPanel.GetComponent<PlayerHandPanelController> ()._fractionDeck.UpdateDeckCounter ();
        }
    }
}