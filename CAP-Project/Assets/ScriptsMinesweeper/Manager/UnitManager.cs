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
    private List<ScriptableUnit> _units;

    public BasePlayer SelectedPlayer;

    [SerializeField] public GameObject _playerList;
    private HashSet<BasePlayer> players = new HashSet<BasePlayer>();
    [SerializeField] public List<BasePlayer> playerList;

    void Awake()
    {
        Instance = this;
        _units = Resources.LoadAll<ScriptableUnit>("UnitsTheWayPass").ToList();
    }

    public void SpawnPlayerOnMinesweeper()
    {
        int playerCount = 2;

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
        //MouseManager.Instance.PLAYER = player;
        Dice.Instance.SelectedPlayer = player;
        GameManager.Instance.SelectedPlayer = player;
        //UIManager.Instance.SelectedPlayer = player;
    }

    public void SpawnPlayerOnTheWayPass()
    {
        int playerCount = 2;

        for (int i = 0; i < playerCount; i++)
        {
            var prefab = i == 0 ? GetPrefabUnit<BasePlayer>(Faction.Player1) : GetPrefabUnit<BasePlayer>(Faction.Player2);
            var spawnedPlayer = Instantiate(prefab);
            playerList.Add(spawnedPlayer);
            HexagonWalk startTile = TheWayPass.Instance.GetTile();
            startTile.SetUnit(spawnedPlayer);
        }
        SetSelectedPlayer(playerList.First<BasePlayer>());
    }

    private T GetPrefabUnit<T>(Faction faction) where T : BaseUnit
    {
        return (T)_units.Where(u => u.Faction == faction).First().UnitPrefab;
    }

    //public void SetSelectedPlayerOnTheWayPass(BasePlayer player)
    //{
        //SelectedPlayer = player;
        //GameManager.Instance.SelectedPlayer = player;
        //Dice.Instance.SelectedPlayer = player;
    //}
}