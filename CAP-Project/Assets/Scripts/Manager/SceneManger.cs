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
    [SerializeField] private Sprite unmute;
    [SerializeField] private Sprite mute;
    [SerializeField] private GameObject LIST;
    [SerializeField] private WebsocketLobby _websocket;
    public PlayerRole role;

    void Awake()
    {
        Instance = this;
    }

    public void Main()
    {
        SceneManager.LoadScene("Main");
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
        _websocket.reqStartGame("60");
        /*Transform[] childs = { LIST.transform.GetChild(1)};
        List<GameObject> list = new List<GameObject>();
        foreach (Transform child in childs)
        {
            if (child.childCount != 0)
            {
                list.Add(child.gameObject);
            }
        }
        if (list.Count == 1 && _websocket.role.isHost)
        {
           
        }*/
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
            role.isHost = false;
            role.isJoin = true;
            role.playerTurn = 0;
            role.lobbyId = lobbyID.text;
            //findLobbyById();
            SceneManager.LoadScene("Lobby");
        }
        else
        {
            Debug.Log("Please Enter Lobby ID.");
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
