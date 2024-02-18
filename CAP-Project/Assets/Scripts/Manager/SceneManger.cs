using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;

public class SceneManger : MonoBehaviour
{
    [SerializeField] private GameObject sound;
    [SerializeField] private Sprite unmute;
    [SerializeField] private Sprite mute;
    [SerializeField] private GameObject LIST;
    [SerializeField] private InputField lobbyID;
    [SerializeField] private WebsocketCLI _websocket;
    public PlayerAction _action;

    public void Main()
    {
        SceneManager.LoadScene("Main");
    }

    public void Menu()
    {
        _action.isJoin = false;
        _action.isHost = false;
        _action.playerTurn = -1;
        SceneManager.LoadScene("Menu");
    }

    public void Game(Button btn)
    {
        Transform[] childs = { LIST.transform.GetChild(1)};
        List<GameObject> list = new List<GameObject>();
        foreach (Transform child in childs)
        {
            if (child.childCount != 0)
            {
                list.Add(child.gameObject);
            }
        }
        if (list.Count == 1 && WebsocketCLI.Instance._action.isHost)
        {
            WebsocketCLI.Instance.reqData("60","123",null);
            SceneManager.LoadScene("Minesweeper");
        }
        else if (WebsocketCLI.Instance._action.isJoin)
        {
            btn.GetComponentInChildren<Text>().text = "Unready";
        }
    }

    public void Lobby()
    {
        _action.isHost = true;
        _action.playerTurn = 0;
        _action.isJoin = false;
        SceneManager.LoadScene("Lobby");
    }

    public void JoinLobby()
    {
        if (lobbyID.text != "")
        {
            _websocket.ConnectWebsocket(lobbyID.text);
            StartCoroutine(LoadSceneAsync());
        }
        else
        {
            Debug.Log("Please Enter Lobby ID.");
        }
    }

    IEnumerator LoadSceneAsync()
    {
        yield return new WaitForSeconds(1f);
        if (_websocket.isFound)
        {
            _action.isHost = false;
            _action.isJoin = true;
            _action.playerTurn = 0;
            SceneManager.LoadScene("Lobby");
        }
    }

    public void LeaderBoard()
    {
        SceneManager.LoadScene("LeaderBoard");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void soundOnOff()
    {
        if(this.sound.GetComponent<SpriteRenderer>().sprite.name == "Unmute")
        {
            this.sound.GetComponent<SpriteRenderer> ().sprite = mute;
            this.sound.GetComponent<Image>().sprite = mute;
        }
        else
        {
            this.sound.GetComponent<SpriteRenderer>().sprite = unmute;
            this.sound.GetComponent<Image>().sprite = unmute;
        }
    }
}
