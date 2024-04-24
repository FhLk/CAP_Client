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
    public Tile[,] _board;
    public Board initBoard;
    [SerializeField] public List<BasePlayer> listPlayer;
    public WebsocketGame websocket;

    void Awake()
    {
        Instance = this;
        if (websocket.role._game1)
        {
            _round = this.GetComponentInChildren<Canvas>().GetComponentInChildren<Text>();
        }
        _r = 1;
    }

    void Start()
    {
        if (websocket.role._game1)
        {
            ChangeStateOnMinesweeper(GameState.GenerateBoard);
        }
        else
        {
            ChangeStateOnTheWayPass(GameState.GenerateBoard);
        }
    }

    public void ChangeStateOnMinesweeper(GameState newState)
    {
        GameState = newState;
        switch (newState)
        {
            case GameState.GenerateBoard:
                _board = initBoard.generateBoard();
                break;
            case GameState.ReqToServer:
                WebSocketMinsweeper.Instance.reqBoard("50", _board);
                break;
            case GameState.SpawnPlayer:
                _round.text = $"Round {WebSocketMinsweeper.Instance.role.round = _r}";
                UnitManager.Instance.SpawnPlayerOnMinesweeper(); 
                break;
            case GameState.PlayerTurn:
                UIManager.Instance.showTurnOfWho(WebSocketMinsweeper.Instance.role.playerTurn);
                break;
            case GameState.NextPlayerTurn:
                Transform nextPlayer = UnitManager.Instance._playerList.transform.GetChild(WebSocketMinsweeper.Instance.role.playerTurn);
                UnitManager.Instance.SetSelectedPlayer(nextPlayer.GetComponent<BasePlayer>());
                _round.text = $"Round {WebSocketMinsweeper.Instance.role.round}";
                ChangeStateOnMinesweeper(GameState.PlayerTurn);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    public void ChangeStateOnTheWayPass(GameState newState)
    {
        GameState = newState;
        switch (newState)
        {
            case GameState.GenerateBoard:
                _board = initBoard.generateBoard();
                GameManager.Instance.ChangeStateOnTheWayPass(GameState.SpawnPlayer);
                GameManager.Instance.ChangeStateOnTheWayPass(GameState.PlayerTurn);
                break;
            case GameState.ReqToServer:
                Debug.Log("wow");
                WebSocketTheWayPass.Instance.reqBoard("50", _board);
                break;
            case GameState.SpawnPlayer:
                UnitManager.Instance.SpawnPlayerOnTheWayPass();
                break;
            case GameState.PlayerTurn:
                break;
            case GameState.NextPlayerTurn:
                BasePlayer nextPlayer = UnitManager.Instance.playerList[SelectedPlayer.indexPlayer == 1 ? 0 : 1];
                UnitManager.Instance.SetSelectedPlayer(nextPlayer);
                ChangeStateOnTheWayPass(GameState.PlayerTurn);
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