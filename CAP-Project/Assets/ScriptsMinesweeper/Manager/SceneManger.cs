using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;

public class SceneManger : MonoBehaviour
{
    public static SceneManger Instance;
    [SerializeField] private GameObject sound;
    [SerializeField] private GameObject _Title;
    [SerializeField] private Sprite unmute;
    [SerializeField] private Sprite mute;
    [SerializeField] private GameObject LIST;
    [SerializeField] private WebsocketLobby _websocket;
    [SerializeField] private Sprite title_1;
    [SerializeField] private Sprite title_2;
    public PlayerRole role;
    [SerializeField] private string URL;

    void Awake()
    {
        try
        {
            Instance = this;
            if (role._game1)
            {
                _Title.gameObject.GetComponent<SpriteRenderer>().sprite = title_1;
            }
            else if (role._game2)
            {
                _Title.gameObject.GetComponent<SpriteRenderer>().sprite = title_2;
            }
        }
        catch { }

    }

    public void Main()
    {
        SceneManager.LoadScene("Main");
    }

    public void ChooseGame1()
    {
        role._game1 = true;
        role._game2 = false;
        Menu();
    }

    public void ChooseGame2()
    {
        role._game1 = false;
        role._game2 = true;
        Menu();
    }

    public void Menu()
    {
        role.isJoin = false;
        role.isHost = false;
        role.playerTurn = -1;
        SceneManager.LoadScene("Menu");
    }

    public void Game(Button btn)
    {
        if (role._game1)
        {
            SceneManager.LoadScene("Minesweeper");
        }
        else if (role._game2)
        {
            SceneManager.LoadScene("Thewaypass");
        }
        _websocket.reqStartGame("60");
    }

    public void Lobby()
    {
        role.isHost = true;
        role.playerTurn = 0;
        role.isJoin = false;
        SceneManager.LoadScene("Lobby");
    }

    public void JoinLobby(InputField lobbyID)
    {
        if (lobbyID.text != "")
        {
            WebsocketScriptable ws = new WebsocketScriptable(URL + "?lobbyId=" + lobbyID.text);
            role.isHost = false;
            role.isJoin = true;
            role.playerTurn = 0;
            role.lobbyId = lobbyID.text;
            ws.ConnectWebsocket();
            ws.checkLobby(lobbyID.text);
        }
        else
        {
            Debug.Log("Please Enter Lobby ID.");
        }
    }

    public void CreateContent()
    {
        SceneManager.LoadScene("Create");
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
