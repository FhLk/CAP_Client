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
            case GameState.SpawnPlayer:
                //_round.text = $"Round {WebsocketCLI.Instance._action.round = _r}";
                UnitManager.Instance.SpawnPlayer();
                break;
            case GameState.PlayerTurn:
                //UIManager.Instance.showTurnOfWho(WebsocketCLI.Instance._action.playerTurn);
                break;
            case GameState.NextPlayerTurn:
                Transform nextPlayer = UnitManager.Instance._playerList.transform.GetChild(WebSocketGame.Instance.GetInstanceID());
                UnitManager.Instance.SetSelectedPlayer(nextPlayer.GetComponent<BasePlayer>());
                //_round.text = $"Round {WebsocketCLI.Instance._action.round}";
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
    SpawnPlayer = 1,
    PlayerTurn = 2,
    NextPlayerTurn = 3,
}