using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
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

        public string messages { get; set; }
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
    public PlayerRole role;
    public bool isFound;

    private async void Awake()
    {
        Instance = this;
        if (role.isHost)
        {
            lobbyId = GenerateRandomID();
            role.lobbyId = lobbyId;
        }
        else if (role.isJoin)
        {
            lobbyId = role.lobbyId;
        }
        _websocket = new WebSocket(_url + "?lobbyId=" + lobbyId);
        _websocket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
        _websocket.OnOpen += OnWebSocketOpen;
        _websocket.OnMessage += OnWebSocketMessage;
        _websocket.Connect();

        _websocket.ConnectAsync();

    }

    public string GenerateRandomID()
    {
        string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        string lobbyID = "";

        for (int i = 0; i < 3; i++)
        {
            lobbyID += alphabet[UnityEngine.Random.Range(0, alphabet.Length)];
        }

        for (int i = 0; i < 3; i++)
        {
            lobbyID += UnityEngine.Random.Range(0, 10).ToString();
        }

        return lobbyID;
    }

    private void OnWebSocketOpen(object sender, EventArgs e)
    {
        Debug.Log("WebSocket connection opened!");
        if (role.isHost)
        {
            role.playerName = players[0].playerName;
            role.id = players[0].id;
            reqData("00", lobbyId, players[0]);
        }
        else if (role.isJoin)
        {
            role.playerName = players[1].playerName;
            role.id = players[1].id;
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
        await Task.Delay(0); // Simulate some processing time
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
        if (receiveData.type == "01")
        {
            role.lobbyId = receiveData.lobby.id;
            LobbyData.Instance.UpdateLobby(receiveData.lobby.players.Count);
            ChatManager.instance.SendChatMessage("Creating Lobby " + lobbyId, "System");
        }
        else if (receiveData.type == "11")
        {
            role.lobbyId = receiveData.lobby.id;
            LobbyData.Instance.UpdateLobby(receiveData.lobby.players.Count);
            //ChatManager.instance.SendChatMessage("Player Join Lobby", "System");
        }
        else if (receiveData.type == "61")
        {
            if (role._game1 && role.isJoin)
            {
                SceneManager.LoadScene("Minesweeper");
            }
            else if (role._game2 && role.isJoin)
            {
                SceneManager.LoadScene("Thewaypass");
            }
        }
        else if (receiveData.type == "-21")
        {
            if (role.isHost)
            {
                ChatManager.instance.SendChatMessage("Player Leave Lobby", "System");
                LobbyData.Instance.UpdateLobby(receiveData.lobby.players.Count);
            }
            else if (role.isJoin)
            {
                SceneManager.LoadScene("Menu");
            }
        }
        else if (receiveData.type == "-2")
        {
            //SceneManager.LoadScene("Menu");
        }
        else if(receiveData.type == "-31")
        {
            ChatManager.instance.AddMessage(receiveData.messages);
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
    }

    public void reqStartGame(string type)
    {
        Dictionary<string, object> dataReq = new Dictionary<string, object>();

        dataReq.Add("type", type);

        string json = JsonConvert.SerializeObject(dataReq);
        _websocket.Send(json);
    }

    public void reqLeaveLobby()
    {
        if (role.isHost)
        {
            _websocket.Close();
            SceneManager.LoadScene("Menu");
        }
        else
        {
            Dictionary<string, object> dataReq = new Dictionary<string, object>();
            Dictionary<string, object> playerData = new Dictionary<string, object>();
            dataReq.Add("type", "-20");
            dataReq.Add("lobbyId", lobbyId);
            playerData.Add("id", role.id);
            playerData.Add("name", role.name);
            dataReq.Add("player", playerData);

            string json = JsonConvert.SerializeObject(dataReq);
            _websocket.Send(json);
        }
    }

    public void reqChat(string messages)
    {
        Dictionary<string, object> dataReq = new Dictionary<string, object>();
        dataReq.Add("type", "-30");
        dataReq.Add("messages", messages);

        string json = JsonConvert.SerializeObject(dataReq);
        _websocket.Send(json);
    }
}