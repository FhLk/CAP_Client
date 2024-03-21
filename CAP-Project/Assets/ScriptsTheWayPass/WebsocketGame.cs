using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;

public class WebSocketGameTheWayPass : MonoBehaviour
{
    class WebSocketMessage
    {
        public ReceiveData receiveData { get; set; }
    }

    class ReceiveData
    {
        public Lobby lobby { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string type { get; set; }
        public int playerIndex { get; set; }

        public Player player { get; set; }

        public int round { get; set; }

        public List<Item> bomb { get; set; }
    }



    public class Lobby
    {
        public string id;
        public List<Player> players;
    }

    public class Player
    {
        public string id;
        public string name;
        public bool pending;
        public int hearts;
        public int shield;
    }

    public static WebSocketGameTheWayPass Instance;
    WebSocketMessage messageTo = new WebSocketMessage();
    private WebSocket ws;
    [SerializeField] private string url; // Replace with your server URL
    [SerializeField] private string lobbyId;
    public WinnerData win;
    public PlayerRole role;
    private BasePlayerTheWayPass winPlayer;

    private class GameData
    {
        public string type;
        public int round;
        public BoardData[,] board;
        public Player[] players;
    }
    private class BoardData
    {
        public bool destroy;
        public ItemData item;
    }

    private class ItemData
    {
    }

    private async void Awake()
    {
        Instance = this;
        if (WebsocketLobby.Instance != null)
        {
            lobbyId = WebsocketLobby.Instance.lobbyId;
        }
        ws = new WebSocket(url + "?lobbyId=" + lobbyId);
        ws.OnOpen += OnWebSocketOpen;
        ws.OnMessage += OnWebSocketMessage;
        ws.OnError += OnWebSocketError;
        ws.OnClose += OnWebSocketClose;
        ws.ConnectAsync();
    }

    private void OnWebSocketOpen(object sender, EventArgs e)
    {
        Debug.Log("WebSocket connection opened!");
        //reqGenerateBoard("90",Board_Cell.Instance.height,Board_Cell.Instance.width);
    }

    private async void OnWebSocketMessage(object sender, MessageEventArgs e)
    {
        Debug.Log("Received message: " + e.Data);

        // Process the received data asynchronously
        await ProcessMessageAsync(e.Data);
    }

    private async Task ProcessMessageAsync(string message)
    {
        // Perform asynchronous operations on the message data here
        await Task.Delay(1000); // Simulate some processing time
        string data = message;
        resData(data);
        Debug.Log("Message processing completed.");
    }

    private void OnWebSocketError(object sender, ErrorEventArgs e)
    {
        Debug.LogError("WebSocket error: " + e.Message);

    }

    private void OnWebSocketClose(object sender, CloseEventArgs e)
    {
        Debug.Log("WebSocket connection closed.");
    }

    private void OnDestroy()
    {
        if (ws != null)
        {
            ws.Close();
            ws = null;
        }
    }

    private void resData(string json)
    {
        ReceiveData receiveData = JsonConvert.DeserializeObject<ReceiveData>(json);
        if (receiveData.type == "51")
        {
            GameMangerTheWayPass.Instance.ChangeState(GameStateTheWayPass.SpawnPlayer);
            GameMangerTheWayPass.Instance.ChangeState(GameStateTheWayPass.PlayerTurn);
        }
        else if (receiveData.type == "31")
        {
            if (role.isJoin && receiveData.playerIndex == 0)
            {
                BoardTheWayPass.Instance.board[receiveData.x, receiveData.y].SetUnit(UnitManagerTheWayPass.Instance.SelectedPlayer);
            }
            else if (role.isHost && receiveData.playerIndex == 1)
            {
                BoardTheWayPass.Instance.board[receiveData.x, receiveData.y].SetUnit(UnitManagerTheWayPass.Instance.SelectedPlayer);
            }
        }
        else if (receiveData.type == "71")
        {
            role.playerTurn = receiveData.playerIndex;
            role.round = receiveData.round;
            GameMangerTheWayPass.Instance.ChangeState(GameStateTheWayPass.NextPlayerTurn);
        }
    }

    public void reqBoard(string type, Tile[,] board)
    {
        BoardData[,] b = new BoardData[BoardTheWayPass.Instance.width, BoardTheWayPass.Instance.height];

        foreach (Tile cell in board)
        {
            b[cell.x, cell.y] = new BoardData();
            b[cell.x, cell.y].item = new ItemData();
        }
        string json = JsonConvert.SerializeObject(new GameData
        {
            type = type,
            board = b,
            round = GameMangerTheWayPass.Instance._r
        });
        ws.Send(json);
    }
    public void reqCell(string type, int x, int y, int tile_type)
    {
        Dictionary<string, object> dataReq = new Dictionary<string, object>();
        dataReq.Add("type", type);
        dataReq.Add("playerIndex", UnitManagerTheWayPass.Instance.SelectedPlayer.indexPlayer);
        dataReq.Add("x", x);
        dataReq.Add("y", y);
        dataReq.Add("tile_type", tile_type);
        string json = JsonConvert.SerializeObject(dataReq);
        ws.Send(json);
    }

    public void reqNextPlayer(string type)
    {
        Dictionary<string, object> dataReq = new Dictionary<string, object>();
        dataReq.Add("type", type);
        dataReq.Add("round", role.round);
        dataReq.Add("playerIndex", UnitManagerTheWayPass.Instance.SelectedPlayer.indexPlayer);
        string json = JsonConvert.SerializeObject(dataReq);
        ws.Send(json);
    }

    public void reqEndGame(string type)
    {
        Dictionary<string, object> dataReq = new Dictionary<string, object>();
        Dictionary<string, object> playerData = new Dictionary<string, object>();
        role.isWin = 1;
        UnitManagerTheWayPass.Instance.SelectedPlayer.isWinner = role.isWin;
        var listTest = UnitManagerTheWayPass.Instance.playerList;
        foreach (var player in listTest)
        {
            if (player.isWinner != 1)
            {
                winPlayer = player;
            }
        }
        dataReq.Add("type", type);
        playerData.Add("id", winPlayer.id);
        playerData.Add("name", winPlayer.playerName);
        dataReq.Add("player", playerData);
        string json = JsonConvert.SerializeObject(dataReq);
        ws.Send(json);
    }

    public void reqBomb(string type, int height, int width, int numBombs)
    {
        Dictionary<string, object> dataReq = new Dictionary<string, object>();
        dataReq.Add("type", type);
        dataReq.Add("height", height);
        dataReq.Add("width", width);
        dataReq.Add("bomb", numBombs);
        string json = JsonConvert.SerializeObject(dataReq);
        ws.Send(json);
    }
}
