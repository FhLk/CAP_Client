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
    private int count = 1;
    public GameObject[] LIST;
    public Text countList;
    public GameObject prefabToJoin;
    public GameObject prefabToLeave;
    public GameObject LISTDisplay;
    public Text LobbyID;

    private void Awake()
    {
        Instance = this;
    }
    public void Start()
    {
        id = WebsocketCLI.Instance.lobbyId;
        LobbyID.text = id;
        countList.text = count + "/4";
    }
    public void updateLobby(int index)
    {
        countList.text = (index) +"/4";
        Transform childToDestroy = LISTDisplay.transform.GetChild(index-1);
        if (childToDestroy != null)
        {
            Vector3 positionToDestroy = childToDestroy.transform.position;

            Destroy(childToDestroy.gameObject);

            GameObject newPlayer = Instantiate(prefabToJoin, LISTDisplay.transform);
            newPlayer.transform.position = positionToDestroy;
            newPlayer.transform.SetSiblingIndex(index-1);
            LIST[index-1] = newPlayer;
        }
    }
}
