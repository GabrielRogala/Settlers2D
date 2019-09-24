using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    #region SINGLETON
    private static GameController _instance;
    public static GameController instance {
        get {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<GameController> ();
            return _instance;
        }
    }
    #endregion

    #region UI
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
    #endregion
    
    public List<PlayerController> _playerControllers;
    public List<DeckManager> _deckManager;

    private int _playerId = 0;
    private int _fractionId = 0;

    public PlayerController _ownPlayerControllers;

    #region MonoBehaviour
    void Start () {
        Debug.Log ("GAME CONTROLLER START");

        _deckManager = new List<DeckManager> ();
        _playerControllers = new List<PlayerController> ();  

        _playerId = Server.instance.GetOwnPlayerId();
        _fractionId = Server.instance.GetOwnFractionId();

        InitDecks (GameDataController.instance.gameData.decks);
        InitPlayersData (GameDataController.instance.gameState.players);
        CreatePlayersPanel (GameDataController.instance.gameState.players);
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.P)) // PASS
        {
            ChangePlayersTurn();
        }
    }
    #endregion

    #region INIT
    void InitDecks(List<DeckData> decks)
    {
        foreach (DeckData d in decks)
        {
            _deckManager.Add(new DeckManager(d));
        }
    }

    void InitPlayersData (List<PlayerData> players) {
        foreach (PlayerData p in players) {
            var resources = GameDataController.instance.gameData.fractions[p.fractionId].resourceGrowthMatrix;
            int i = 0;
            foreach (int r in resources) {
                p.playerResourcesGrowth.Add(i, r);
                p.playerResources.Add (i, 0);
                i++;
            }
        }
        Debug.Log (GameDataController.instance.gameState);
    }

    void CreatePlayersPanel (List<PlayerData> players) {
        PlayerBoardTabController._contentList = new List<GameObject> ();
        PlayerHandTabController._contentList = new List<GameObject> ();

        foreach (PlayerData p in players) {
            PlayerController playerController = new PlayerController (p);
            _playerControllers.Add (playerController);

            if (p.playerId == _playerId)
                _ownPlayerControllers = playerController;

            GameObject boardTab = Instantiate (_playerBoardTabPrefab, _playerBoardTabContainer.transform) as GameObject;
            GameObject boardContent = Instantiate (_playerBoardPanelPrefab, _playerBoardPanelContainer.transform) as GameObject;

            playerController._playerBoardPanel = boardContent;

            boardContent.GetComponent<PlayerBoardPanelController> ().InitBoardPanel (playerController);

            boardTab.GetComponent<PlayerBoardTabController> ()._tabLabel.text = p.name;
            boardTab.GetComponent<PlayerBoardTabController> ()._content = boardContent;
            PlayerBoardTabController._contentList.Add (boardContent);

            boardTab.GetComponent<Toggle> ().group = _playerBoardToggleGroup;
            boardTab.GetComponent<Toggle> ().isOn = false;

            boardContent.SetActive (true);
        }

        foreach (PlayerHandTabController t in _playerBoardTabContainer.GetComponentsInChildren<PlayerHandTabController> ()) {
            t.GetComponent<Toggle> ().isOn = true;
            t.ShowTabContent ();
            break;
        }

        GameObject handContent = Instantiate(_playerHandPanelPrefab, _playerHandPanelContainer.transform) as GameObject;
        _ownPlayerControllers._playerHandPanel = handContent;
        Debug.Log(GetDeckControllerFromFractionId(0)._deckData.ToString());
        Debug.Log(GetDeckControllerFromFractionId(_fractionId)._deckData.ToString());
        handContent.GetComponent<PlayerHandPanelController>().InitPlayerDecks(_ownPlayerControllers,
            GetDeckControllerFromFractionId(0),
            GetDeckControllerFromFractionId(_fractionId));
    }
    #endregion

    #region GameTurnCTRL
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
            GameDataController.instance.gameState.playerIdTurn = 1;
        }else{
            GameDataController.instance.gameState.playerIdTurn++;
        }
        Debug.Log("Player #"+GameDataController.instance.gameState.playerIdTurn + " turn");
        Debug.Log(_playerControllers[GameDataController.instance.gameState.playerIdTurn-1]._playerData.ToString());
    }

    #endregion

    public bool IsPlayerTurn(int playerId){
        return playerId == GameDataController.instance.gameState.playerIdTurn;
    }

    DeckManager GetDeckControllerFromFractionId (int fractionId) {
        foreach (DeckManager d in _deckManager) {
            if (d._deckData.fractionId == fractionId) {
                return d;
            }
        }
        return null;
    }

    #region GameUICTRL
    public void UpdateDecksCounter () {
        //foreach (PlayerController p in _playerControllers) {
        //    p._playerHandPanel.GetComponent<PlayerHandPanelController> ()._defaultDeck.UpdateDeckCounter ();
        //    p._playerHandPanel.GetComponent<PlayerHandPanelController> ()._fractionDeck.UpdateDeckCounter ();
        //}
    }

    public void UpdatePlayersResources(){
        foreach(PlayerController p in _playerControllers){
            p.UpdatePlayerResources();
        }
    }
    #endregion

    #region CardActions
    public void DrawCardREQ(int fractionId) {
        Server.instance.DrawCard(fractionId);
    }

    public void DrawCardCFM(int playerId, int deckId, int cardId, int deckSize)
    {
        _playerControllers[playerId - 1].AddCardToHand(deckId,cardId,deckSize);
    }

    public bool IsAbleToBuild(int playerId, int deckId, int cardId)
    {
        CardData card = _deckManager[deckId].GetCardFromId(cardId);
        Debug.Log(card.ToString());
        //Dictionary<int, int> cost = new Dictionary<int, int>();
        //foreach (int resource in card.cost)
        //{
        //    if (cost.ContainsKey(resource))
        //    {
        //        cost[resource]++;
        //    }
        //    else
        //    {
        //        cost.Add(resource, 1);
        //    }
        //}

        //bool enoughtCountOfResources = true;

        //foreach (KeyValuePair<int, int> kvp in cost)
        //{
        //    if (_playerControllers[playerId]._playerData.playerResources[kvp.Key] < kvp.Value)
        //    {
        //        enoughtCountOfResources = false;
        //    }

        //}

        //return enoughtCountOfResources;
        return true;
    }

    public void BuildCardREQ(int playerId, SmallCardController card)
    {
        Debug.Log("BuildCard " + playerId + " | " + card._card.ToString());
        Server.instance.BuildCardREQ(playerId,card._card.fractionType, card._card.cardId);
    }

    public void BuildCardCFM(int playerId, int deckId, int cardId)
    {
        CardData card = _deckManager[deckId].GetCardFromId(cardId);
        Debug.Log("BuildCard " + playerId + " | "+ card.ToString());

        foreach(SmallCardController cardCtrl in _playerControllers[playerId]._playerHandPanel.GetComponentsInChildren<SmallCardController>())
        {
            Debug.Log("Check card | " + cardCtrl._card.ToString());
            if (cardCtrl._card.cardId == card.cardId && cardCtrl._card.fractionType == card.fractionType)
            {
                _playerControllers[playerId-1].AddCardToBoard(cardCtrl);
            }
        }

        CardViewerController.instance.HideFullSizeCard();
    }

    public void PlunderCard(int playerId, SmallCardController card){
        IncreasePlayerResource(playerId,4,-1); // cost 1 PILLAGE
        foreach(int r in card._card.gain){
            IncreasePlayerResource(playerId,r,1);
        }
        CardViewerController.instance.HideFullSizeCard();
        Destroy(card.gameObject);
        UpdatePlayersResources();
        ChangePlayersTurn();
    }
    
    public void TributeCard(int playerId, SmallCardController card){
        IncreasePlayerResource(playerId,9,1); // gain 1 FOUNDATION
        CardViewerController.instance.HideFullSizeCard();
        Destroy(card.gameObject);
        UpdatePlayersResources();
    }

    public void AddCardToContract(int playerId, SmallCardController card)
    {
        if(card._card.fractionType > 0){
            _playerControllers[playerId-1].AddCardToContract(card);
            IncreasePlayerResource(playerId,6, -1); // cost 1 FOOD
            IncreasePlayerResource(playerId,card._card.contract,1);
            IncreasePlayerResourceInGrowthMatrix(playerId,card._card.contract,1);
            CardViewerController.instance.HideFullSizeCard();
            UpdatePlayersResources();
            ChangePlayersTurn();
        }
    }
    #endregion

    #region DEPRICATED
    void ProductionStage() {
        foreach(PlayerController p in _playerControllers)
        {
            foreach(ResourcesData r in GameDataController.instance.gameData.resources)
            {
                p._playerData.playerResources[r.resourcesId] += p._playerData.playerResourcesGrowth[r.resourcesId];
            }
        }
    }

    public void IncreasePlayerResourceInGrowthMatrix(int playerId, int resourceId, int value){
        _playerControllers[playerId-1]._playerData.playerResourcesGrowth[resourceId] += value;
    }

    public void IncreasePlayerResource(int playerId, int resourceId, int value){
        _playerControllers[playerId-1]._playerData.playerResources[resourceId] += value;
    }

    internal static void SetPlayerIdTurn(int playerId)
    {
        GameDataController.instance.gameState.playerIdTurn = playerId;
    }
    #endregion

    #region SERVER_HOST_ACTION
    public int GetRandomCardFromDeck(int deckId){
        return _deckManager[deckId].GetRandomCardIdFromDeck();
    }

    internal void UpdateDeckSize(int deckId, int deckSize)
    {
        _ownPlayerControllers.UpdateDecksSize(deckId,deckSize);
    }

    #endregion
}