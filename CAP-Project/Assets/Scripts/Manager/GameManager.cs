using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState GameState;
    public BasePlayer SelectedPlayer;
    private Text _round;
    public int _r;
    private HexagonTile[,] _board;

    void Awake()
    {
        Instance = this;
        _round = this.GetComponentInChildren<Canvas>().GetComponentInChildren<Text>();
        _r = 1;
    }

    void Start()
    {
        ChangeState(GameState.GenerateBoard);
    }

    public void ChangeState(GameState newState)
    {
        GameState = newState;
        switch (newState)
        {
            case GameState.GenerateBoard:
                Board_Cell.Instance.generateBoard();
                break;
            case GameState.ReqToServer:
                WebSocketGame.Instance.reqBoard("50", Board_Cell.Instance.board);
                break;
            case GameState.SpawnPlayer:
                _round.text = $"Round {WebSocketGame.Instance.role.round = _r}";
                UnitManager.Instance.SpawnPlayer(); 
                break;
            case GameState.PlayerTurn:
                UIManager.Instance.showTurnOfWho(WebSocketGame.Instance.role.playerTurn);
                break;
            case GameState.NextPlayerTurn:
                Transform nextPlayer = UnitManager.Instance._playerList.transform.GetChild(WebSocketGame.Instance.role.playerTurn);
                UnitManager.Instance.SetSelectedPlayer(nextPlayer.GetComponent<BasePlayer>());
                _round.text = $"Round {WebSocketGame.Instance.role.round}";
                ChangeState(GameState.PlayerTurn);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }
}
public enum GameState
{
    GenerateBoard = 0,
    ReqToServer = 1,
    SpawnPlayer = 2,
    PlayerTurn = 3,
    NextPlayerTurn = 4,
}