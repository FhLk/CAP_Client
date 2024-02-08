using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        public string type { get; set; }
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
        public bool shield;
    }

    public static WebsocketCLI Instance;
    WebSocketMessage messageTo = new WebSocketMessage();
    public WebSocket _websocket;
    [SerializeField] private string _url;
    [SerializeField] public string lobbyId;
    [SerializeField] public List<PlayerAPI> players;
    public PlayerAction _action;
    private int index = 1;
    public bool isFound;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if(SceneManager.GetActiveScene().name == "Lobby")
        {
            lobbyId = GenerateRandomID();
            _websocket = new WebSocket(_url + "?lobbyId=" + lobbyId);
            _websocket.OnOpen += OnOpen;
            _websocket.OnMessage += OnMessage;
            _websocket.Connect();
        }
    }

    public void ConnectWebsocket(string id)
    {
        lobbyId = id;
        if(_websocket != null)
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
        Debug.Log("Client connected");
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            if (_action.isHost)
            {
                reqData("00", lobbyId, players[0]);
            }
            else if(_action.isJoin)
            {
                Debug.Log("wow");
                reqData("10", lobbyId, players[1]);
            }
        }
        else if(SceneManager.GetActiveScene().name == "Menu")
        {
            reqData("09", lobbyId, null);
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
        Debug.Log(receiveData.type);
        if (receiveData.type == "11")
        {
            Debug.Log("Join");
            LobbyData.Instance.updateLobby(receiveData.lobby.players.Count);
        }
        else if(receiveData.type == "19")
        {
            Debug.Log("Lobby");
            isFound = true;
            _websocket.Close();
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
        else if(type == "10")
        {
            dataReq.Add("type", type);
            dataReq.Add("lobbyId", lobbyId);
            playerData.Add("id", player.id);
            playerData.Add("name", player.name);
            dataReq.Add("player", playerData);

            string json = JsonConvert.SerializeObject(dataReq);
            _websocket.Send(json);
        }
        else if (type == "09")
        {
            dataReq.Add("type", type);
            dataReq.Add("lobbyId", lobbyId);

            string json = JsonConvert.SerializeObject(dataReq);
            _websocket.Send(json);
        }
    }

    public void addPlayer()
    {
        if(index < 4)
        {
            reqData("10", lobbyId, players[index++]);
        }

    }

    public void kickPlayer()
    {
        if (index > 1)
        {
            //reqData("10", lobbyId, players[index++]);
        }

    }
}