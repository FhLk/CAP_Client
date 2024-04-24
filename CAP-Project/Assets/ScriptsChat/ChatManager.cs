using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public static ChatManager instance;

    [SerializeField] ChatMessage chatMessagePrefab;
    [SerializeField] CanvasGroup chatContent;
    [SerializeField] InputField chatInput;
    [SerializeField] private PlayerRole role;

    private void Awake()
    {
        ChatManager.instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendChatMessage(chatInput.text, role.playerName);
            chatInput.text = "";
        } 
    }

    public void SendChatMessage(string _message, string _fromWho = null)
    {
        if (string.IsNullOrWhiteSpace(_message)) return;
        string S = _fromWho + " > " + _message;
        SendChatMessageServerRpc(S);
    }

    public void AddMessage(string msg)
    {
        ChatMessage CM = Instantiate(chatMessagePrefab, chatContent.transform);
        CM.SetText(msg);
    }

    void SendChatMessageServerRpc(string message)
    {
        WebsocketLobby.Instance.reqChat(message);
    }
}
