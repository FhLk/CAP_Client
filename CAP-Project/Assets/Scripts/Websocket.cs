using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;

public class WebsocketCLI : MonoBehaviour
{
    class WebSocketMessage
    {
        public SendData sendData { get; set; }
        public ReceiveData receiveData { get; set; }
    }

    public class SendData
    {
        public string id { get; set; }
        public Player player { get; set; }
        public string type { get; set; }

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

    public static WebsocketCLI Instance;
    WebSocketMessage messageTo = new WebSocketMessage();
    public WebSocket _websocket;
    [SerializeField] private string _url;
    [SerializeField] public string lobbyId;
    [SerializeField] public List<PlayerAPI> players;
    public PlayerAction _action;
    public bool isFound;

    private void Awake()
    {
        Instance = this;
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            lobbyId = GenerateRandomID();
            _websocket = new WebSocket(_url + "?lobbyId=" + lobbyId);
            _websocket.OnOpen += OnOpen;
            _websocket.OnMessage += OnMessage;
            _websocket.Connect();
        }
        else if (SceneManager.GetActiveScene().name == "Minesweeper")
        {
            if (_websocket != null)
            {
                _websocket.Close();
            }
            _websocket = new WebSocket(_url + "?lobbyId=" + lobbyId);
            _websocket.OnOpen += OnOpen;
            _websocket.OnMessage += OnMessage;
            _websocket.Connect();
        }
    }

    public void ConnectWebsocket(string id)
    {
        lobbyId = id;
        if (_websocket != null)
        {
            _websocket.Close();
        }
        _websocket = new WebSocket(_url + "?lobbyId=" + id);
        _websocket.OnOpen += OnOpen;
        _websocket.OnMessage += OnMessage;
        _websocket.Connect();
    }

    public string GenerateRandomID()
    {
        return "123";
    }

    private void OnOpen(object sender, EventArgs e)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            if (_action.isHost)
            {
                reqData("00", lobbyId, players[0]);
            }
            else if (_action.isJoin)
            {
                reqData("10", lobbyId, players[1]);
            }
        }
        else if (SceneManager.GetActiveScene().name == "Menu")
        {
            reqData("20", lobbyId, null);
        }
        throw new NotImplementedException();
    }
    private void OnMessage(object socket, MessageEventArgs message)
    {
        string data = message.Data;
        messageTo.receiveData = resData(data);
    }

    private ReceiveData resData(string json)
    {
        ReceiveData receiveData = JsonConvert.DeserializeObject<ReceiveData>(json);
        Debug.Log(receiveData.round);
        if (receiveData.type == "11")
        {
            LobbyData.Instance.UpdateLobby(receiveData.lobby.players.Count);
        }
        else if (receiveData.type == "21")
        {
            isFound = true;
            _websocket.Close();
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
            _action.playerTurn = receiveData.playerIndex;
            _action.round = receiveData.round;
            GameManager.Instance.ChangeState(GameState.NextPlayerTurn);
        }

        return receiveData;
    }

    public void reqData(string type, string lobbyId, PlayerAPI player)
    {
        Dictionary<string, object> dataReq = new Dictionary<string, object>();
        Dictionary<string, object> playerData = new Dictionary<string, object>();
        if (type == "00")
        {
            dataReq.Add("type", type);
            dataReq.Add("lobbyId", lobbyId);
            playerData.Add("id", player.id);
            playerData.Add("name", player.name);
            dataReq.Add("player", playerData);

            string json = JsonConvert.SerializeObject(dataReq);
            _websocket.Send(json);
        }
        else if (type == "10")
        {
            dataReq.Add("type", type);
            dataReq.Add("lobbyId", lobbyId);
            playerData.Add("id", player.id);
            playerData.Add("name", player.name);
            dataReq.Add("player", playerData);

            string json = JsonConvert.SerializeObject(dataReq);
            _websocket.Send(json);
        }
        else if (type == "20")
        {
            dataReq.Add("type", type);
            dataReq.Add("lobbyId", lobbyId);

            string json = JsonConvert.SerializeObject(dataReq);
            _websocket.Send(json);
        }
        else if(type == "60")
        {
            dataReq.Add("type", type);

            string json = JsonConvert.SerializeObject(dataReq);
            _websocket.Send(json);
        }
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
        public Item item;
    }

    private class Item
    {
    }

    public void reqDataInGame(string type, BasePlayer[] players, HexagonTile[,] board)
    {
        if (type == "50")
        {
            BoardData[,] b = new BoardData[Board_Cell.Instance.width, Board_Cell.Instance.height];
            foreach (HexagonTile cell in board) 
            {
                b[cell.x, cell.y] = new BoardData();
                b[cell.x,cell.y].destroy = !cell.isActive;
                b[cell.x,cell.y].item = new Item();
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
            _websocket.Send(json);
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
            _websocket.Send(json);
        }
    }

    public void reqDataInGame(string type, string lobbyId)
    {
        Dictionary<string, object> dataReq = new Dictionary<string, object>();
        if (type == "70")
        {
            dataReq.Add("type", type);
            dataReq.Add("lobbyId", lobbyId);
            dataReq.Add("round",_action.round);
            dataReq.Add("playerIndex", UnitManager.Instance.SelectedPlayer.indexPlayer);
            string json = JsonConvert.SerializeObject(dataReq);
            _websocket.Send(json);
        }
    }

}