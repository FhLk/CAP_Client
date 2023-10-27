using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    private List<ScriptableUnit> _units;
    public BasePlayer SelectedPlayer;

    void Awake()
    {
        Instance = this;

        _units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
    }

    public void SpawnPlayer()
    {
        var heroCount = 1;

        for (int i = 0; i < heroCount; i++)
        {
            var randomPrefab = GetRandomUnit<BasePlayer>(Faction.Player);
            var spawnedHero = Instantiate(randomPrefab);
            var randomSpawnTile = Board.Instance.GetPlayerSpawnTile();

            SetSelectedHero(spawnedHero);
            randomSpawnTile.SetUnit(spawnedHero);
        }

    }

    private T GetRandomUnit<T>(Faction faction) where T : BaseUnit
    {
        return (T)_units.Where(u => u.Faction == faction).OrderBy(o => Random.value).First().UnitPrefab;
    }

    public void SetSelectedHero(BasePlayer player)
    {
        SelectedPlayer = player;
        //MenuManager.Instance.ShowSelectedHero(hero);
    }
}