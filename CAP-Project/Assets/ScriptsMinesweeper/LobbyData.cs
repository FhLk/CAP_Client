using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyData : MonoBehaviour
{
    public static LobbyData Instance;
    public string id;
    int[] listPlayer = { 1, 2, 3 };
    private int count = 0;
    public Text countList;
    public GameObject LISTDisplay;
    public Text LobbyID;
    public Sprite slot_1;
    public Sprite slot_2;
    public Sprite empty;
    private bool isJoin;
    private bool isHost;
    [SerializeField] private Button startBTN;
    
    private void Awake()
    {
        Instance = this;
    }
    public void Start()
    {
        id = WebsocketLobby.Instance.lobbyId;
        LobbyID.text = id;
        countList.text = count + "/2";
        LobbyInterface();
    }

    public void UpdateLobby(int index)
    {
        disenableGameobject();
        count = index;
        countList.text = (count) + "/2";
        for (int i = 0; i < count; i++) 
        {
            enableGameobject(i);
        }
    }

    private void enableGameobject(int i)
    {
        Transform childToChange = LISTDisplay.transform.GetChild(i);
        if (i == 0)
        {
            childToChange.gameObject.GetComponent<SpriteRenderer>().sprite = slot_1;
        }
        else if (i == 1)
        {
            childToChange.gameObject.GetComponent<SpriteRenderer>().sprite = slot_2;
        }
        Transform secondChild = childToChange.transform.GetChild(0);
        secondChild.GetComponent<Text>().text = childToChange.gameObject.GetComponent<PlayerAPI>().playerName;
    }

    private void disenableGameobject()
    {
        for (int i = 0;i < LISTDisplay.transform.childCount;i++)
        {
            Transform childToChange = LISTDisplay.transform.GetChild(i);
            childToChange.gameObject.GetComponent<SpriteRenderer>().sprite = empty;
        }
    }

    private void LobbyInterface()
    {
        isHost = WebsocketLobby.Instance.role.isHost;
        isJoin = WebsocketLobby.Instance.role.isJoin;
        if (isHost)
        {
            startBTN.GetComponentInChildren<Text>().text = "Start";
        }
        else if (isJoin)
        {
            startBTN.GetComponentInChildren<Text>().text = "Waiting Host";
            startBTN.GetComponentInChildren<Text>().color = Color.black;
            startBTN.interactable = false;
        }
        else
        {
            startBTN.GetComponentInChildren<Text>().text = "";
        }
    }
}
