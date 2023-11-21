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
                _round.text = $"Round {_r = 1}";
                UnitManager.Instance.SpawnPlayer();
                break;
            case GameState.PlayerTurn:
                break;
            case GameState.NextPlayerTurn:
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