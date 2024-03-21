using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Action",menuName ="Action")]
public class PlayerRole : ScriptableObject
{
    public bool isHost = false;
    public bool isJoin = false;
    public int playerTurn = -1;
    public int round = 0;
    public string lobbyId;
    public string playerName;
    public string id;
    public int isWin = -1; //win = 0, lose = 1

    void Awake()
    {
        isHost = false;
        isJoin = false;
        playerTurn = -1;
        lobbyId = string.Empty;
        isWin = -1;
    }
}
