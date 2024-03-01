using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;

public class WebsocketLobby : MonoBehaviour
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

    public static WebsocketLobby Instance;
    WebSocketMessage messageTo = new WebSocketMessage();
    public WebSocket _websocket;
    [SerializeField] private string _url;
    [SerializeField] public string lobbyId;
    [SerializeField] public List<PlayerAPI> players;
    public PlayerAction _action;
    public bool isFound;

    private async void Awake()
    {
        Instance = this;
        if (_action.isHost)
        {
            lobbyId = GenerateRandomID();
        }
        Debug.Log(lobbyId);
        _websocket = new WebSocket(_url + "?lobbyId=" + lobbyId);
        _websocket.OnOpen += OnWebSocketOpen;
        _websocket.OnMessage += OnWebSocketMessage;
        _websocket.Connect();

        _websocket.ConnectAsync();

    }

    public string GenerateRandomID()
    {
        // ตัวอักษรภาษาอังกฤษพิมพ์ใหญ่
        string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        // สร้างตัวแปรเก็บ Lobby ID
        string lobbyID = "";

        // สุ่มตัวอักษร 3 ตัว
        for (int i = 0; i < 3; i++)
        {
            lobbyID += alphabet[UnityEngine.Random.Range(0, alphabet.Length)];
        }

        // สุ่มตัวเลข 3 ตัว
        for (int i = 0; i < 3; i++)
        {
            lobbyID += UnityEngine.Random.Range(0, 10).ToString();
        }

        // ส่ง Lobby ID กลับ
        return lobbyID;
    }

    private void OnWebSocketOpen(object sender, EventArgs e)
    {
        Debug.Log("WebSocket connection opened!");
        if (_action.isHost)
        {
            reqData("00", lobbyId, players[0]);
        }
        else if (_action.isJoin)
        {
            reqData("10", lobbyId, players[1]);
        }
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

    private ReceiveData resData(string json)
    {
        ReceiveData receiveData = JsonConvert.DeserializeObject<ReceiveData>(json);
        if(receiveData.type == "01")
        {
            LobbyData.Instance.UpdateLobby(receiveData.lobby.players.Count);
        }
        else if (receiveData.type == "11")
        {
            LobbyData.Instance.UpdateLobby(receiveData.lobby.players.Count);
        }
        return receiveData;
    }

    public void reqData(string type, string lobbyId, PlayerAPI player)
    {
        Dictionary<string, object> dataReq = new Dictionary<string, object>();
        Dictionary<string, object> playerData = new Dictionary<string, object>();
        if (type == "00")
        {
            Debug.Log(lobbyId);
            dataReq.Add("type", type);
            dataReq.Add("lobbyId", lobbyId);
            playerData.Add("id", player.id);
            playerData.Add("name", player.playerName);
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
        else if (type == "60")
        {
            dataReq.Add("type", type);

            string json = JsonConvert.SerializeObject(dataReq);
            _websocket.Send(json);
        }
    }

}