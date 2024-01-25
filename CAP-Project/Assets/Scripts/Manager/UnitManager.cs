using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    public BasePlayer SelectedPlayer;

    [SerializeField] public GameObject _playerList;
    private HashSet<BasePlayer> players = new HashSet<BasePlayer>();

    void Awake()
    {
        Instance = this;
    }

    public void SpawnPlayer()
    {
        int playerCount = 4;

        for (int i = 0; i < playerCount; i++)
        {
            Transform child = _playerList.transform.GetChild(i);
            if (child != null)
            {
                child.gameObject.SetActive(true);
                players.Add(child.GetComponent<BasePlayer>());
            }
        }

        SetSelectedPlayer(players.First<BasePlayer>());
    }

    public void SetSelectedPlayer(BasePlayer player)
    {
        SelectedPlayer = player;
        MouseManager.Instance.PLAYER = player;
        Dice.Instance.SelectedPlayer = player;
        GameManager.Instance.SelectedPlayer = player;
        UIManager.Instance.SelectedPlayer = player;
    }

    public void IncreaseHeart()
    {
        SelectedPlayer.increaseHearts();
    }

    public void DncreaseHeart()
    {
        SelectedPlayer.decreaseHearts();
    }

    public void resetAllPlayer()
    {
        foreach (var item in players)
        {
            item.resetPlayer();
        };
    }
}