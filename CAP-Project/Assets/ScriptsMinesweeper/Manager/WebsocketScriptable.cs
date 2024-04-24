using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;

public class WebsocketScriptable : ScriptableObject
{
    public string URL;
    public string lobbyId;
    public int height;
    public int width;
    public int maxBomb;
    public int maxLadder;
    public int maxPlayers;
    private WebSocket websocket;
    WebSocketMessage messageTo = new WebSocketMessage();

    class WebSocketMessage
    {
        public ReceiveData receiveData { get; set; }
    }

    class ReceiveData
    {
        public int x { get; set; }
        public int y { get; set; }
        public string type { get; set; }
        public int playerIndex { get; set; }
        public int round { get; set; }
    }

    public WebsocketScriptable(string url)
    {
        this.URL = url;
    }

    public void ConnectWebsocket()
    {
        websocket = new WebSocket(this.URL);
        websocket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
        websocket.OnOpen += OnWebSocketOpen;
        websocket.OnMessage += OnWebSocketMessage;
        websocket.Connect();

        websocket.ConnectAsync();
        if (!websocket.IsAlive)
        {
            SceneManager.LoadScene("Main");
        }

    }

    public void checkLobby(string id)
    {
        reqData("20", id);
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
        if (receiveData.type == "21")
        {
            Debug.Log("Lobby is found");
            SceneManager.LoadScene("Lobby");
        }
        else
        {
            Debug.Log("Lobby not found");
        }
        return receiveData;
    }

    public void reqData(string type, int height, int width, int bomb, int ladder, int players)
    {
        Dictionary<string, object> dataReq = new Dictionary<string, object>();
        dataReq.Add("type", type);
        dataReq.Add("height", height);
        dataReq.Add("width", width);
        dataReq.Add("bomb", bomb);
        dataReq.Add("ladder", ladder);
        dataReq.Add("players", players);

        string json = JsonConvert.SerializeObject(dataReq);
        websocket.Send(json);
    }

    public void reqData(string type, string lobbyId)
    {
        Dictionary<string, object> dataReq = new Dictionary<string, object>();
        dataReq.Add("type", type);
        dataReq.Add("lobbyId", lobbyId);

        string json = JsonConvert.SerializeObject(dataReq);
        websocket.Send(json);
    }
}
