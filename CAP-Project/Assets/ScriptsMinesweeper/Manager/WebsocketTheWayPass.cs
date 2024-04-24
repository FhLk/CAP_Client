using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;

public class WebSocketTheWayPass : WebsocketGame
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

        public Item[,] ladder {  get; set; } 
    }

    public static WebSocketTheWayPass Instance;
    private WebSocket ws;



    private async void Awake()
    {
        Instance = this;
        OpenConnection();
    }

    public void reqEndGame(string type)
    {
        Debug.Log("wow");
        Dictionary<string, object> dataReq = new Dictionary<string, object>();
        Dictionary<string, object> playerData = new Dictionary<string, object>();
        role.isWin = 1;
        UnitManager.Instance.SelectedPlayer.isWinner = role.isWin;
        winPlayer = GameManager.Instance.SelectedPlayer;
        dataReq.Add("type", type);
        playerData.Add("id", winPlayer.id);
        playerData.Add("name", winPlayer.playerName);
        dataReq.Add("player", playerData);
        string json = JsonConvert.SerializeObject(dataReq);
        ws.Send(json);
    }

}
