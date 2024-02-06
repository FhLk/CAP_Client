using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class LobbyData : MonoBehaviour
{
    public static LobbyData Instance;
    public string id;
    int[] listPlayer = { 1, 2, 3 };
    private int count = 1;
    public GameObject[] LIST;
    public Text countList;
    public GameObject LISTDisplay;
    public Text LobbyID;
    public Sprite slotBG;

    private void Awake()
    {
        Instance = this;
    }
    public void Start()
    {
        id = WebsocketCLI.Instance.GenerateRandomID();
        LobbyID.text = id;
        countList.text = count + "/4";
    }

    public void updateLobby(int index)
    {
        countList.text = (index) +"/4";
        Transform childToChange = LISTDisplay.transform.GetChild(index - 1);
        childToChange.gameObject.GetComponent<SpriteRenderer>().sprite = slotBG;
        Transform firstChild = childToChange.transform.GetChild(0);
        firstChild.gameObject.SetActive(true);
        Transform secondChild = childToChange.transform.GetChild(1);
        secondChild.gameObject.SetActive(true);
        secondChild.GetComponent<Text>().text = childToChange.gameObject.GetComponent<PlayerAPI>().name;
    }
    public void updateLobby()
    {
    }
}
