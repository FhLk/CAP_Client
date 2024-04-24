using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateSetting : MonoBehaviour
{
    WebsocketScriptable websocketScriptable;
    [SerializeField] private string URL;
    [SerializeField] private InputField height;
    [SerializeField] private InputField width;
    [SerializeField] private InputField bomb;
    [SerializeField] private InputField ladder;
    [SerializeField] private InputField players;

    private void Start()
    {
        OpenConnect();
    }
    public void OpenConnect()
    {
        websocketScriptable = new WebsocketScriptable(this.URL);
        websocketScriptable.ConnectWebsocket();
    }

    public void sendRequest()
    {
        websocketScriptable.reqData("40", int.Parse(this.height.text), int.Parse(this.width.text), int.Parse(this.bomb.text), int.Parse(this.ladder.text), int.Parse(this.players.text));
    }
    
}
