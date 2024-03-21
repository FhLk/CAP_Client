using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<Item> bomb { get; set; }

        public int tile_type { get; set; }
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
    private WebSocket ws;
    [SerializeField] private string url; // Replace with your server URL
    [SerializeField] private string lobbyId;
    public WinnerData win;
    public PlayerRole role;
    private BasePlayer winPlayerTest;

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
        if (receiveData.type == "31")
        {
            if (receiveData.tile_type != 1)
            {
                BoardMinesweeper.Instance.removeCell(BoardMinesweeper.Instance.board[receiveData.x, receiveData.y]);
            }
            else if(receiveData.tile_type == 1)
            {
                BoardMinesweeper.Instance.disableCell(BoardMinesweeper.Instance.board[receiveData.x, receiveData.y]);
            }
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
            Debug.Log("Game End");
            win.id = receiveData.player.id;
            win.playerName = receiveData.player.name;

            SceneManager.LoadScene("End");
        }
        if (receiveData.type == "91")
        {
            BoardMinesweeper.Instance.bombs = receiveData.bomb;
            BoardMinesweeper.Instance.stateDefind();
            GameManager.Instance.ChangeState(GameState.ReqToServer);
        }
    }

    public void reqBoard(string type, HexagonTile[,] board)
    {
        BoardData[,] b = new BoardData[BoardMinesweeper.Instance.width, BoardMinesweeper.Instance.height];

        foreach (HexagonTile cell in board)
        {
            b[cell.x, cell.y] = new BoardData();
            b[cell.x, cell.y].destroy = !cell.isActive;
            b[cell.x, cell.y].item = new ItemData();
        }
        string json = JsonConvert.SerializeObject(new GameData
        {
            type = type,
            board = b,
            round = GameManager.Instance._r
        });
        ws.Send(json);
    }
    public void reqCell(string type, int x, int y,int tile_type)
    {
        Dictionary<string, object> dataReq = new Dictionary<string, object>();
        dataReq.Add("type", type);
        dataReq.Add("playerIndex", UnitManager.Instance.SelectedPlayer.indexPlayer);
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
        dataReq.Add("playerIndex", UnitManager.Instance.SelectedPlayer.indexPlayer);
        string json = JsonConvert.SerializeObject(dataReq);
        ws.Send(json);
    }

    public void reqEndGame(string type)
    {
        Debug.Log("wow");
        Dictionary<string, object> dataReq = new Dictionary<string, object>();
        Dictionary<string, object> playerData = new Dictionary<string, object>();
        role.isWin = 1;
        UnitManager.Instance.SelectedPlayer.isWinner = role.isWin;
        var listTest = GameManager.Instance.listPlayer;
        foreach (var player in listTest)
        {
            if (player.isWinner != 1)
            {
                winPlayerTest = player;
            }
        }
        dataReq.Add("type", type);
        playerData.Add("id", winPlayerTest.id);
        playerData.Add("name", winPlayerTest.playerName);
        dataReq.Add("player",playerData);
        string json = JsonConvert.SerializeObject(dataReq);
        ws.Send(json);
    }

    public void reqBomb(string type,int height, int width,int numBombs)
    {
        Dictionary<string, object> dataReq = new Dictionary<string, object>();
        dataReq.Add("type", type);
        dataReq.Add("height", height);
        dataReq.Add("width", width);
        dataReq.Add("bomb",numBombs);
        string json = JsonConvert.SerializeObject(dataReq);
        ws.Send(json);
    }
}
