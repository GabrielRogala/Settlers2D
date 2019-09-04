using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataController {
    ////////////////////////////////////////////
    //private static DataController _instance;
    //public static DataController instance
    //{
    //    get
    //    {
    //        if (_instance == null)
    //            _instance = GameObject.FindObjectOfType<DataController>();
    //        return _instance;
    //    }
    //}
    ////////////////////////////////////////////

    private static string _fileNameGameData = "GameData.json";
    private static string _fileNameGameState = "GameState.json";
    private static string _dataPath = Application.dataPath;

    public static GameData LoadGameData () {

        #if UNITY_EDITOR
            string filePath = Path.Combine(Application.streamingAssetsPath, _fileNameGameData);
#elif UNITY_IOS
            string filePath = Path.Combine (Application.streamingAssetsPath + "/Raw", _fileNameGameData);
#elif UNITY_ANDROID
            string filePath = Path.Combine ("jar:file://" + Application.streamingAssetsPath + "!assets/", _fileNameGameData);
#endif

        if (File.Exists (filePath)) {

            #if UNITY_EDITOR || UNITY_IOS
                string dataAsJson = File.ReadAllText(filePath, System.Text.Encoding.GetEncoding("Windows-1250"));

            #elif UNITY_ANDROID
                WWW reader = new WWW (filePath);
                while (!reader.isDone) {
                }
                string dataAsJson = reader.text;
            #endif

            GameDataWrapper gameDataWrapper = JsonUtility.FromJson<GameDataWrapper> (dataAsJson);
            // Debug.Log(gameDataWrapper.gameData.ToString());
            return gameDataWrapper.gameData;
        } else {
            Debug.Log ("File " + filePath + " does not exist");
            return null;
        }
    }

    public static void SaveGameData (GameData gameData) {
        //GameDataWrapper gameDataWrapper = new GameDataWrapper ();
        //gameDataWrapper.gameData = gameData;
        //string dataAsJson = JsonUtility.ToJson (gameDataWrapper);
        //string filePath = _dataPath + "/" + _fileNameGameData;
        //System.IO.File.WriteAllText (filePath, dataAsJson);
    }

    public static GameState LoadGameState () {
#if UNITY_EDITOR
        string filePath = Path.Combine(Application.streamingAssetsPath, _fileNameGameState);

#elif UNITY_IOS
        string filePath = Path.Combine (Application.streamingAssetsPath + "/Raw", _fileNameGameState);
 
#elif UNITY_ANDROID
        string filePath = Path.Combine ("jar:file://" + Application.streamingAssetsPath + "!assets/", _fileNameGameState);
 
#endif

        if (File.Exists (filePath)) {

#if UNITY_EDITOR || UNITY_IOS
            string dataAsJson = File.ReadAllText(filePath, System.Text.Encoding.GetEncoding("Windows-1250"));

#elif UNITY_ANDROID
                WWW reader = new WWW (filePath);
                while (!reader.isDone) {
                }
                string dataAsJson = reader.text;
#endif
            GameStateWrapper gameStateWrapper = JsonUtility.FromJson<GameStateWrapper> (dataAsJson);
            return gameStateWrapper.gameState;
        } else {
            Debug.Log ("File " + filePath + " does not exist");
            return null;
        }
    }

    public static void SaveGameState (GameState gameData) {
        //GameStateWrapper gameStateWrapper = new GameStateWrapper ();
        //gameStateWrapper.gameState = gameData;
        //string dataAsJson = JsonUtility.ToJson (gameStateWrapper);
        //string filePath = _dataPath + "/" + _fileNameGameState;
        //System.IO.File.WriteAllText (filePath, dataAsJson);
    }
}

[System.Serializable]
public class GameDataWrapper {
    public GameData gameData;
}

[System.Serializable]
public class GameStateWrapper {
    public GameState gameState;
}

[System.Serializable]
public class GameData {
    public List<ResourcesData> resources;
    public List<FractionData> fractions;
    public List<DeckData> decks;

    public GameData () {
        this.resources = new List<ResourcesData> ();
        this.fractions = new List<FractionData> ();
        this.decks = new List<DeckData> ();
    }

    public override string ToString () {
        string str = "\n---GAMEDATA---\n";
        str += "\n---ResourcesData---\n";
        foreach (ResourcesData r in resources) {
            str += r.ToString ();
            str += "---------------------\n";
        }
        str += "---------------------\n";

        str += "\n---FractionData---\n";
        foreach (FractionData f in fractions) {
            str += f.ToString ();
            str += "---------------------\n";
        }
        str += "---------------------\n";

        str += "\n---DeckData---\n";
        foreach (DeckData d in decks) {
            str += d.ToString ();
            str += "---------------------\n";
        }
        str += "---------------------\n";

        return str;
    }
}

[System.Serializable]
public class GameState {
    public List<PlayerData> players;
    public List<DeckData> decks;
    public List<CardData> removedCards;
    public int roundCounter;
    public int playerIdTurn;

    public GameState () {
        this.players = new List<PlayerData> ();
        this.decks = new List<DeckData> ();
        this.removedCards = new List<CardData> ();
        this.roundCounter = 0;
        this.playerIdTurn = 0;
    }

    public override string ToString () {
        string str = "\n---GameState---\n";
        str += "Round: " + roundCounter + "\n";
        str += "Player turn: " + playerIdTurn + "\n";
        str += "\n---PlayersData---\n";
        foreach (PlayerData p in players) {
            str += p.ToString ();
            str += "---------------------\n";
        }
        str += "---------------------\n";

        str += "\n---DeckData---\n";
        foreach (DeckData d in decks) {
            str += d.ToString ();
            str += "---------------------\n";
        }
        str += "---------------------\n";

        str += "\n---RemovedCards---\n";
        foreach (CardData c in removedCards) {
            str += c.ToString ();
            str += "---------------------\n";
        }
        str += "---------------------\n";

        return str;
    }
}

[System.Serializable]
public class PlayerData {
    public int playerId;
    public string name;
    public int fractionId;
    public Dictionary<int, int> playerResources;
    public Dictionary<int, int> playerResourcesGrowth;
    public List<CardData> cardsInHand;
    public List<CardData> cardsInBoard;
    public List<CardData> cardsInContract;

    public PlayerData (int playerId, string name, int fractionId) {
        this.playerId = playerId;
        this.name = name;
        this.fractionId = fractionId;

        playerResources = new Dictionary<int, int> ();
        playerResourcesGrowth = new Dictionary<int, int> ();
        cardsInHand = new List<CardData> ();
        cardsInBoard = new List<CardData> ();
        cardsInContract = new List<CardData> ();

        Debug.Log ("CREATE PLAYER : " + ToString ());
    }

    public override string ToString () {
        string str = "\n---PlayerData---\n";
        str += "Player: #" + playerId + " " + name + "\n";
        str += "Fraction: " + fractionId + "\n";
        str += "\n---RESOURCES---\n";
        foreach (var pair in playerResources) {
            str += pair.Key + " : " + pair.Value + " | ";
        }
        str += "---------------------\n";
        str += "\n---CardsInHand---\n";
        foreach (CardData c in cardsInHand) {
            str += c.ToString ();
            str += "---------------------\n";
        }
        str += "---------------------\n";

        str += "\n---CardsInBoard---\n";
        foreach (CardData c in cardsInBoard) {
            str += c.ToString ();
            str += "---------------------\n";
        }
        str += "---------------------\n";

        str += "\n---CardsInContract---\n";
        foreach (CardData c in cardsInContract) {
            str += c.ToString ();
            str += "---------------------\n";
        }
        str += "---------------------\n";

        return str;
    }

    public void ProductResources () {
        foreach (ResourcesData r in GameDataController.instance.gameData.resources) {
            playerResources[r.resourcesId] += playerResourcesGrowth[r.resourcesId];
        }
    }
}

[System.Serializable]
public class DeckData {
    public int fractionId;
    public List<CardData> cards;

    public DeckData () {
        this.fractionId = 0;
        this.cards = new List<CardData> ();
    }

    public DeckData (int fractionId) {
        this.fractionId = fractionId;
        this.cards = new List<CardData> ();
    }

    public DeckData (int fractionId, List<CardData> cards) {
        this.fractionId = fractionId;
        this.cards = cards;
    }

    public void AddCard (CardData card) {
        cards.Add (card);
    }

    public override string ToString () {
        string str = "\n---DeckData---\n";
        str += "Fraction: " + fractionId + "\n";
        str += "\n---Cards---\n";
        foreach (CardData c in cards) {
            str += c.ToString ();
            str += "---------------------\n";
        }
        str += "---------------------\n";

        return str;
    }
}

[System.Serializable]
public class FractionData {
    public int fractionId;
    public string fractionName;
    public List<int> resourceGrowthMatrix;
    public List<int> remainingResourcesMatrix;

    public FractionData () {
        fractionId = 0;
        fractionName = "New Fraction";
        resourceGrowthMatrix = new List<int> { 0 };
        remainingResourcesMatrix = new List<int> { 0 };
    }

    public override string ToString () {
        string str = "\n---FractionData---\n";
        str += "Fraction Id  : " + fractionId + "\n";
        str += "Fraction name: " + fractionName + "\n";
        str += "resourceGrowthMatrix: [ ";
        foreach (int i in resourceGrowthMatrix) {
            str += i + " ";
        }
        str += "]\n";
        str += "remainingResourcesMatrix: [ ";
        foreach (int i in remainingResourcesMatrix) {
            str += i + " ";
        }
        str += "]\n";
        return str;
    }
}

[System.Serializable]
public class ResourcesData {
    public int resourcesId;
    public string resourcesName;

    public ResourcesData () {
        resourcesId = 0;
        resourcesName = "New Resource";
    }

    public override string ToString () {
        string str = "\n---ResourcesData---\n";
        str += "Resource Id  : " + resourcesId + "\n";
        str += "Resource name: " + resourcesName + "\n";
        return str;
    }
}

[System.Serializable]
public class CardData {
    public int cardId;
    public string cardName;
    public string description;
    public int cardType;
    public int fractionType;
    public int actionType; // production, trait, action
    public int contract;
    public List<int> cost;
    public List<int> gain;
    public int image;

    public CardData () {
        cardId = 0;
        cardName = "New Card";
        description = ".......";
        cardType = 0;
        fractionType = 0;
        actionType = 0;
        contract = 0;
        cost = new List<int> { 0 };
        gain = new List<int> { 0 };
        image = 0;
    }

    public override string ToString () {
        string str = "#" +
            cardId + " " +
            cardName + " <" +
            description + "> " +
            cardType + " " +
            fractionType + " " +
            actionType + " " +
            contract + " ";
        str += "[ ";
        foreach (int c in cost) {
            str += c + " ";
        }
        str += "] [ ";
        foreach (int g in gain) {
            str += g + " ";
        }
        str += "] " + image;

        return str;
    }
}