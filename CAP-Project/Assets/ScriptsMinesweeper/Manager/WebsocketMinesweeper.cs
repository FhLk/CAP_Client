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
    public static WebSocketMinsweeper Instance;

    private void Awake()
    {
        Instance = this;
        OpenConnection();
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
        sendRequestEnd(json);
    }

}
