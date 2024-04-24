using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;

public class WebsocketGame : MonoBehaviour
{
    private WebSocket ws;
    [SerializeField] public string url; // Replace with your server URL
    [SerializeField] public string lobbyId;
    public WinnerData win;
    public PlayerRole role;
    public BasePlayer winPlayer;

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

        public Item[,] ladder { get; set; }

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

    public async void OpenConnection()
    {
        ws = new WebSocket(url + "?lobbyId=" + role.lobbyId);
        ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
        ws.ConnectAsync();
        ws.OnOpen += OnWebSocketOpen;
        ws.OnMessage += OnWebSocketMessage;
        ws.OnError += OnWebSocketError;
        ws.OnClose += OnWebSocketClose;
    }

    private void OnWebSocketOpen(object sender, EventArgs e)
    {
        Debug.Log("WebSocket connection opened!");
        if (role._game2)
        {
            TheWayPass.Instance.stateRequest();
        }
        //reqLadder("90", BoardTheWayPass.Instance.height, BoardTheWayPass.Instance.width, BoardTheWayPass.Instance.numLadders);
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
        await Task.Delay(0); // Simulate some processing time
        string data = message;
        if (role._game1)
        {
            resDataOnMinesweeper(data);
        }
        else 
        {
            resDataOnTheWayPass(data);
        }
        
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

    public void DisconncetGame()
    {
        Dictionary<string, object> dataReq = new Dictionary<string, object>();
        dataReq.Add("type", "-1");
        string json = JsonConvert.SerializeObject(dataReq);
        ws.Send(json);
    }

    public void endGame()
    {
        SceneManager.LoadScene("End");
    }

    public void reqBoard(string type, Tile[,] board)
    {
        BoardData[,] b = new BoardData[board.GetLength(0), board.GetLength(1)];

        foreach (Tile cell in board)
        {
            b[cell.x, cell.y] = new BoardData();
            b[cell.x, cell.y].destroy = false;
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

    public void reqCell(string type, int x, int y, int tile_type)
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

    public void reqLadder(string type, int height, int width, int numLadders)
    {
        Dictionary<string, object> dataReq = new Dictionary<string, object>();
        dataReq.Add("type", type);
        dataReq.Add("height", height);
        dataReq.Add("width", width);
        dataReq.Add("bomb", 0);
        dataReq.Add("ladder", numLadders);
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
        dataReq.Add("ladder", 0);
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

    private void resDataOnTheWayPass(string json)
    {
        ReceiveData receiveData = JsonConvert.DeserializeObject<ReceiveData>(json);
        if (receiveData.type == "51")
        {
            GameManager.Instance.ChangeStateOnTheWayPass(GameState.SpawnPlayer);
            GameManager.Instance.ChangeStateOnTheWayPass(GameState.PlayerTurn);
        }
        else if (receiveData.type == "31")
        {
            if (role.isJoin && receiveData.playerIndex == 0)
            {
                TheWayPass.Instance.board[receiveData.x, receiveData.y].gameObject.GetComponent<HexagonWalk>().SetUnit(UnitManager.Instance.SelectedPlayer);
            }
            else if (role.isHost && receiveData.playerIndex == 1)
            {
                TheWayPass.Instance.board[receiveData.x, receiveData.y].gameObject.GetComponent<HexagonWalk>().SetUnit(UnitManager.Instance.SelectedPlayer);
            }
        }
        else if (receiveData.type == "91")
        {
            TheWayPass.Instance.ladder = receiveData.ladder;
            TheWayPass.Instance.stateDefind();
            GameManager.Instance.ChangeStateOnTheWayPass(GameState.ReqToServer);
        }
        else if (receiveData.type == "71")
        {
            role.playerTurn = receiveData.playerIndex;
            role.round = receiveData.round;
            GameManager.Instance.ChangeStateOnTheWayPass(GameState.NextPlayerTurn);
        }
        else if (receiveData.type == "81")
        {
            Debug.Log("Game End");
            win.id = receiveData.player.id;
            win.playerName = receiveData.player.name;

            SceneManager.LoadScene("End");
        }
        else if (receiveData.type == "-2")
        {
            Debug.Log("Player Disconnect");
            ws.Close();
            SceneManager.LoadScene("Main");
        }
    }

    private void resDataOnMinesweeper(string json)
    {
        ReceiveData receiveData = JsonConvert.DeserializeObject<ReceiveData>(json);
        if (receiveData.type == "31")
        {
            if (receiveData.tile_type != 1)
            {
                Minesweeper.Instance.removeCell(Minesweeper.Instance.board[receiveData.x, receiveData.y]);
            }
            else if (receiveData.tile_type == 1)
            {
                Minesweeper.Instance.disableCell(Minesweeper.Instance.board[receiveData.x, receiveData.y]);
            }
        }
        else if (receiveData.type == "51")
        {
            GameManager.Instance.ChangeStateOnMinesweeper(GameState.SpawnPlayer);
            GameManager.Instance.ChangeStateOnMinesweeper(GameState.PlayerTurn);
        }
        else if (receiveData.type == "71")
        {
            role.playerTurn = receiveData.playerIndex;
            role.round = receiveData.round;
            GameManager.Instance.ChangeStateOnMinesweeper(GameState.NextPlayerTurn);
        }
        else if (receiveData.type == "81")
        {
            Debug.Log("Game End");
            win.id = receiveData.player.id;
            win.playerName = receiveData.player.name;

            Invoke("endGame", 2);

        }
        if (receiveData.type == "91")
        {
            Minesweeper.Instance.bombs = receiveData.bomb;
            Minesweeper.Instance.stateDefind();
            GameManager.Instance.ChangeStateOnMinesweeper(GameState.ReqToServer);
        }
    }

}
