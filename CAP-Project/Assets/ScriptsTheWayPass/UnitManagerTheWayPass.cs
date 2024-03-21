using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManagerTheWayPass : MonoBehaviour
{
    public static UnitManagerTheWayPass Instance;
    private List<ScriptableUnit> _units;
    public BasePlayerTheWayPass SelectedPlayer;
    [SerializeField] public List<BasePlayerTheWayPass> playerList;


    private void Awake()
    {
        Instance = this;

        _units = Resources.LoadAll<ScriptableUnit>("UnitsTheWayPass").ToList();
    }

    public void SpawnPlayer()
    {
        int playerCount = 2;

        for (int i = 0; i < playerCount; i++)
        {
            var prefab = i == 0 ? GetPrefabUnit<BasePlayerTheWayPass>(Faction.Player1) : GetPrefabUnit<BasePlayerTheWayPass>(Faction.Player2);
            var spawnedPlayer = Instantiate(prefab);
            playerList.Add(spawnedPlayer);
            Tile startTile = BoardTheWayPass.Instance.GetTile();
            startTile.SetUnit(spawnedPlayer);
        }
        SetSelectedPlayer(playerList.First<BasePlayerTheWayPass>());
    }

    public void SpawnPlayerJoin()
    {
        int playerCount = 1;

        for (int i = 0; i < playerCount; i++)
        {
            var prefab = GetPrefabUnit<BasePlayerTheWayPass>(Faction.Player2);
            var spawnedPlayer = Instantiate(prefab);
            playerList.Add(spawnedPlayer);
            Tile startTile = BoardTheWayPass.Instance.GetTile();
            startTile.SetUnit(spawnedPlayer);
        }
        SetSelectedPlayer(playerList.First<BasePlayerTheWayPass>());
    }

    private T GetPrefabUnit<T>(Faction faction) where T : BaseUnit
    {
        return (T)_units.Where(u => u.Faction == faction).First().UnitPrefab;
    }

    public void SetSelectedPlayer(BasePlayerTheWayPass player)
    {
        SelectedPlayer = player;
        GameMangerTheWayPass.Instance.SelectedPlayer = player;
        DiceTheWayPass.Instance.SelectedPlayer = player;
    }
}
