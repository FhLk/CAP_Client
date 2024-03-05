using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;

public class WebSocketGame : MonoBehaviour
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

    public static WebSocketGame Instance;
    WebSocketMessage messageTo = new WebSocketMessage();
    private WebSocket ws;
    [SerializeField] private string url; // Replace with your server URL
    [SerializeField] private string lobbyId;
    public WinnerData win;
    public PlayerRole role;

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
        public Item item;
    }

    private class Item
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
        messageTo.receiveData = resData(data);
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

    private ReceiveData resData(string json)
    {
        ReceiveData receiveData = JsonConvert.DeserializeObject<ReceiveData>(json);
        if (receiveData.type == "31")
        {
            Board_Cell.Instance.removeCell(receiveData.x, receiveData.y);
        }
        else if (receiveData.type == "51")
        {
            GameManager.Instance.ChangeState(GameState.SpawnPlayer);
            GameManager.Instance.ChangeState(GameState.PlayerTurn);
        }
        else if (receiveData.type == "71")
        {
            role.playerTurn = receiveData.playerIndex;
            role.round = receiveData.round;
            GameManager.Instance.ChangeState(GameState.NextPlayerTurn);
        }
        else if (receiveData.type == "81")
        {
            win.id = receiveData.player.id;
            win.playerName = receiveData.player.name;

            SceneManager.LoadScene("End");
        }


        return receiveData;
    }

    public void reqBoard(string type, HexagonTile[,] board)
    {
        BoardData[,] b = new BoardData[Board_Cell.Instance.width, Board_Cell.Instance.height];

        foreach (HexagonTile cell in board)
        {
            b[cell.x, cell.y] = new BoardData();
            b[cell.x, cell.y].destroy = !cell.isActive;
            b[cell.x, cell.y].item = new Item();
        }
        string json = JsonConvert.SerializeObject(new GameData
        {
            type = type,
            board = b,
            round = GameManager.Instance._r
        });
        ws.Send(json);
    }
    public void reqCell(string type, int x, int y)
    {
        Dictionary<string, object> dataReq = new Dictionary<string, object>();
        dataReq.Add("type", type);
        dataReq.Add("playerIndex", UnitManager.Instance.SelectedPlayer.indexPlayer);
        dataReq.Add("x", x);
        dataReq.Add("y", y);
        string json = JsonConvert.SerializeObject(dataReq);
        ws.Send(json);
    }

    public void reqNextPlayer(string type)
    {
        Dictionary<string, object> dataReq = new Dictionary<string, object>();
        dataReq.Add("type", type);
        dataReq.Add("round", role.round);
        dataReq.Add("playerIndex", UnitManager.Instance.SelectedPlayer.indexPlayer);
        string json = JsonConvert.SerializeObject(dataReq);
        ws.Send(json);
    }

    public void reqEndGame(string type)
    {
        Dictionary<string, object> dataReq = new Dictionary<string, object>();
        Dictionary<string, object> playerData = new Dictionary<string, object>();
        dataReq.Add("type", type);
        playerData.Add("id",role.id);
        playerData.Add("name",role.playerName);
        dataReq.Add("player",playerData);
        string json = JsonConvert.SerializeObject(dataReq);
        ws.Send(json);
    }
}
