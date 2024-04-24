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
    public bool _game1;
    public bool _game2;

    void Awake()
    {
        isHost = false;
        isJoin = false;
        playerTurn = -1;
        lobbyId = string.Empty;
        isWin = -1;
    }

    public void ChooseGame1()
    {
        _game1 = true;
        _game2 = false;
    }

    public void ChooseGame2()
    {
        _game1 = false;
        _game2 = true;
    }
}
