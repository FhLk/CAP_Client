using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private WebSocket _websocket;
    [SerializeField] private string _url;
    [SerializeField] public string lobbyId;
    [SerializeField] public PlayerAPI player;
    Dictionary<string, object> dataReq = new Dictionary<string, object>();
    Dictionary<string, object> playerData = new Dictionary<string, object>();

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _websocket = new WebSocket(_url + "?lobbyId=" + lobbyId);
        _websocket.OnOpen += OnOpen;
        _websocket.OnMessage += OnMessage;
        _websocket.Connect();

    }

    private void OnOpen(object sender, EventArgs e)
    {
        Debug.Log("Client connected");
        reqData("00", lobbyId, player);
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
        //Debug.Log(receiveData.type);
        if (receiveData.type == "11")
        {
            LobbyData.Instance.updateLobby(receiveData.lobby.players.Count);
            //lobby.updateLobby(receiveData.lobby.players.Count);
        }
        return receiveData;
    }

    public void reqData(string type, string lobbyId, PlayerAPI player)
    {
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
    }
}