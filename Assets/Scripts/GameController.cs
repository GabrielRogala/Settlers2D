using System;
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
        //List<PlayerData> players;
        //players = new List<PlayerData>();
        //players.Add(new PlayerData(1, "name1", 1));
        //players.Add(new PlayerData(2, "name2", 2));
        //players.Add(new PlayerData(3, "name3", 3));
        //GameDataController.instance.InitGameData(players);
        //////////////////////////////////

        InitDecks (GameDataController.instance.gameData.decks);
        InitPlayersData (GameDataController.instance.gameState.players);
        CreatePlayersPanel (GameDataController.instance.gameState.players);

    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartGame();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            ChangePlayersTurn();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            NextRound();
        }
    }

    void StartGame(){
        ChangePlayersTurn();
        ProductionStage();
        UpdatePlayersResources();
    }

    void NextRound(){
        ProductionStage();
        UpdatePlayersResources();
    }

    void ChangePlayersTurn(){
        if(GameDataController.instance.gameState.playerIdTurn == _playerControllers.Count){
            GameDataController.instance.gameState.playerIdTurn = 1; // first player Id
        }else{
            GameDataController.instance.gameState.playerIdTurn++;
        }
        Debug.Log("Player #"+GameDataController.instance.gameState.playerIdTurn + " turn");
        Debug.Log(_playerControllers[GameDataController.instance.gameState.playerIdTurn-1]._playerData.ToString());
    }

    public bool IsPlayerTurn(int playerId){
        return playerId == GameDataController.instance.gameState.playerIdTurn;
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
                p.playerResourcesGrowth.Add(i, r);
                //p.playerResourcesGrowth.Add (i, 10);
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

    public void UpdatePlayersResources(){
        foreach(PlayerController p in _playerControllers){
            p.UpdatePlayerResources();
        }
    }

    void ProductionStage() {
        foreach(PlayerController p in _playerControllers)
        {
            foreach(ResourcesData r in GameDataController.instance.gameData.resources)
            {
                p._playerData.playerResources[r.resourcesId] += p._playerData.playerResourcesGrowth[r.resourcesId];
            }
        }
    }

    public bool IsAbleToBuild(int playerId, SmallCardController card){
                
        // Dictionary<int, int> cost = new Dictionary<int, int>();
        // foreach(int resource in card._card.cost)
        // {
        //     if (cost.ContainsKey(resource)) {
        //         cost[resource]++;
        //     }
        //     else
        //     {
        //         cost.Add(resource, 1);
        //     }
        // }

        // bool enoughtCountOfResources = true;

        // foreach(KeyValuePair<int,int> kvp in cost)
        // {
        //     if (_playerControllers[playerId]._playerData.playerResources[kvp.Key] < kvp.Value)
        //     {
        //         enoughtCountOfResources = false;
        //     }

        // }

        // return enoughtCountOfResources;
        return true;
    }

    /// ChangePlayersTurn
    public bool BuildCard(int playerId, SmallCardController card)
    {
        Debug.Log("BuildCard " + playerId + " | "+ card._card.ToString());
        if (IsAbleToBuild(playerId,card))
        {
            foreach(int resource in card._card.cost)
            {
                _playerControllers[playerId-1]._playerData.playerResources[resource]--;
            }
            _playerControllers[playerId-1]._playerData.cardsInBoard.Add(card._card);
            _playerControllers[playerId-1]._playerData.cardsInHand.Remove(card._card);

            _playerControllers[playerId-1].AddCardToBoard(card);
            CardViewer.instance.HideFullSizeCard();
            UpdatePlayersResources();
            ChangePlayersTurn();
            return true;     
        }

        ChangePlayersTurn();
        return false;
    }

    public void IncreasePlayerResourceInGrowthMatrix(int playerId, int resourceId, int value){
        _playerControllers[playerId-1]._playerData.playerResourcesGrowth[resourceId] += value;
    }

    public void IncreasePlayerResource(int playerId, int resourceId, int value){
        _playerControllers[playerId-1]._playerData.playerResources[resourceId] += value;
    }

    ///ChangePlayersTurn
    public void PlunderCard(int playerId, SmallCardController card){
        IncreasePlayerResource(playerId,4,-1); // cost 1 PILLAGE
        foreach(int r in card._card.gain){
            IncreasePlayerResource(playerId,r,1);
        }
        CardViewer.instance.HideFullSizeCard();
        Destroy(card.gameObject);
        UpdatePlayersResources();
        ChangePlayersTurn();
    }
    
    public void TributeCard(int playerId, SmallCardController card){
        IncreasePlayerResource(playerId,9,1); // gain 1 FOUNDATION
        CardViewer.instance.HideFullSizeCard();
        Destroy(card.gameObject);
        UpdatePlayersResources();
    }

    ///ChangePlayersTurn
    public void AddCardToContract(int playerId, SmallCardController card)
    {
        if(card._card.fractionType > 0){
            _playerControllers[playerId-1].AddCardToContract(card);
            IncreasePlayerResource(playerId,6, -1); // cost 1 FOOD
            IncreasePlayerResource(playerId,card._card.contract,1);
            IncreasePlayerResourceInGrowthMatrix(playerId,card._card.contract,1);
            CardViewer.instance.HideFullSizeCard();
            UpdatePlayersResources();
            ChangePlayersTurn();
        }
    }

    internal static void SetPlayerIdTurn(int playerId)
    {
        GameDataController.instance.gameState.playerIdTurn = playerId;
    }
}