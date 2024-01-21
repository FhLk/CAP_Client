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
    private int _r = 0;
    private int _player = 1;

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
                _player = 1;
                Board_Cell.Instance.generateBoard();
                break;
            case GameState.SpawnPlayer:
                _round.text = $"Round {_r = 1}";
                UnitManager.Instance.SpawnPlayer();
                break;
            case GameState.PlayerTurn:
                UIManager.Instance.showTurnOfWho(_player);
                break;
            case GameState.NextPlayerTurn:
                _player++;
                if(_player == 5)
                {
                    _player = 1;
                }
                Transform nextPlayer = UnitManager.Instance._playerList.transform.GetChild(_player-1);
                UnitManager.Instance.SetSelectedPlayer(nextPlayer.GetComponent<BasePlayer>());
                _r++;
                _round.text = $"Round {_r}";
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