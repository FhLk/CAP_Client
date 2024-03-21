using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameMangerTheWayPass : MonoBehaviour
{
    public static GameMangerTheWayPass Instance;
    public GameStateTheWayPass GameState;
    public BasePlayerTheWayPass SelectedPlayer;
    //private Text _round;
    public int _r;
    private HexagonTile[,] _board;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        ChangeState(GameStateTheWayPass.GenerateBoard);
    }

    public void ChangeState(GameStateTheWayPass newState)
    {
        GameState = newState;
        switch (newState)
        {
            case GameStateTheWayPass.GenerateBoard:
                BoardTheWayPass.Instance.generateBoard();
                break;
            case GameStateTheWayPass.ReqToServer:
                WebSocketGameTheWayPass.Instance.reqBoard("50", BoardTheWayPass.Instance.board);
                break;
            case GameStateTheWayPass.SpawnPlayer:
                UnitManagerTheWayPass.Instance.SpawnPlayer();
                break;
            case GameStateTheWayPass.PlayerTurn:
                break;
            case GameStateTheWayPass.NextPlayerTurn:
                BasePlayerTheWayPass nextPlayer = UnitManagerTheWayPass.Instance.playerList[SelectedPlayer.indexPlayer == 1 ? 0:1];
                UnitManagerTheWayPass.Instance.SetSelectedPlayer(nextPlayer);
                ChangeState(GameStateTheWayPass.PlayerTurn);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }
}

public enum GameStateTheWayPass
{
    GenerateBoard = 0,
    ReqToServer = 1,
    SpawnPlayer = 2,
    PlayerTurn = 3,
    NextPlayerTurn = 4,
}
