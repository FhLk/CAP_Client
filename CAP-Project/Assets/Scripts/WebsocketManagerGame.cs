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
    private WebSocket ws;
    private string url = "ws://localhost:80/ws/lobby?lobby=123"; // Replace with your server URL
    [SerializeField] public List<PlayerAPI> players;

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

    private async void Start()
    {
        ws = new WebSocket(url);
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
        if (receiveData.type == "11")
        {
            LobbyData.Instance.UpdateLobby(receiveData.lobby.players.Count);
        }
        else if (receiveData.type == "21")
        {
            ws.Close();
        }
        else if (receiveData.type == "31")
        {
            Board_Cell.Instance.removeCell(receiveData.x, receiveData.y);
        }
        else if (receiveData.type == "61")
        {
            SceneManager.LoadScene("Minesweeper");
        }
        else if (receiveData.type == "71")
        {
            //_action.playerTurn = receiveData.playerIndex;
            //_action.round = receiveData.round;
            //GameManager.Instance.ChangeState(GameState.NextPlayerTurn);
        }

        return receiveData;
    }

    public void reqDataInGame(string type, BasePlayer[] players, HexagonTile[,] board)
    {
        if (type == "50")
        {
            BoardData[,] b = new BoardData[Board_Cell.Instance.width, Board_Cell.Instance.height];

            foreach (HexagonTile cell in board)
            {
                b[cell.x, cell.y] = new BoardData();
                b[cell.x, cell.y].destroy = !cell.isActive;
                b[cell.x, cell.y].item = new Item();
            }

            Player[] p = new Player[] {
                new Player { id = players[0].id, name = players[0].playerName },
                new Player { id =  players[1].id, name = players[1].playerName }
            };
            string json = JsonConvert.SerializeObject(new GameData
            {
                type = type,
                board = b,
                round = GameManager.Instance._r,
                players = p
            });
            ws.Send(json);
        }
    }
    public void reqDataInGame(string type, int x, int y)
    {
        Dictionary<string, object> dataReq = new Dictionary<string, object>();
        if (type == "30")
        {
            dataReq.Add("type", type);
            dataReq.Add("playerIndex", UnitManager.Instance.SelectedPlayer.indexPlayer);
            dataReq.Add("x", x);
            dataReq.Add("y", y);
            string json = JsonConvert.SerializeObject(dataReq);
            //_websocket.Send(json);
        }
    }

    public void reqDataInGame(string type, string lobbyId)
    {
        Dictionary<string, object> dataReq = new Dictionary<string, object>();
        if (type == "70")
        {
            dataReq.Add("type", type);
            dataReq.Add("lobbyId", lobbyId);
            //dataReq.Add("round", _action.round);
            dataReq.Add("playerIndex", UnitManager.Instance.SelectedPlayer.indexPlayer);
            string json = JsonConvert.SerializeObject(dataReq);
            ws.Send(json);
        }
    }
}
