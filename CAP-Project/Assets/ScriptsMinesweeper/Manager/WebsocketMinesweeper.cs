using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;

public class WebSocketMinsweeper : WebsocketGame
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

        public Item[,] ladder { get; set; }
    }

    public static WebSocketMinsweeper Instance;
    private WebSocket ws;

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
        Minesweeper.Instance.stateRequest(Minesweeper.Instance.numBombs);
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
                Minesweeper.Instance.removeCell(Minesweeper.Instance.board[receiveData.x, receiveData.y]);
            }
            else if(receiveData.tile_type == 1)
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


    public void reqEndGame(string type)
    {
        Dictionary<string, object> dataReq = new Dictionary<string, object>();
        Dictionary<string, object> playerData = new Dictionary<string, object>();
        role.isWin = 1;
        UnitManager.Instance.SelectedPlayer.isWinner = role.isWin;
        var listTest = GameManager.Instance.listPlayer;
        foreach (var player in listTest)
        {
            if (player.isWinner != 1)
            {
                winPlayer = player;
            }
        }
        dataReq.Add("type", type);
        playerData.Add("id", winPlayer.id);
        playerData.Add("name", winPlayer.playerName);
        dataReq.Add("player", playerData);
        string json = JsonConvert.SerializeObject(dataReq);
        ws.Send(json);
    }

}
