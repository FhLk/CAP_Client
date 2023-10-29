using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    [SerializeField] Text dice;
    private List<ScriptableUnit> _units;
    public BasePlayer SelectedPlayer;

    void Awake()
    {
        _units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
    }
    public void OnButtonPress()
    {
        if (SelectedPlayer == null) 
        {
            SpawnPlayer();
        }
        int n = randomDice();
        dice.text = n.ToString();
        sendDice(n, SelectedPlayer);

    }

    private void SpawnPlayer()
    {
        var heroCount = 1;

        for (int i = 0; i < heroCount; i++)
        {
            var randomPrefab = GetRandomUnit<BasePlayer>(Faction.Player);
            var spawnedPlayer = Instantiate(randomPrefab);

            SetSelectedHero(spawnedPlayer);
        }

    }

    private T GetRandomUnit<T>(Faction faction) where T : BaseUnit
    {
        return (T)_units.Where(u => u.Faction == faction).OrderBy(o => Random.value).First().UnitPrefab;
    }

    public void SetSelectedHero(BasePlayer player)
    {
        SelectedPlayer = player;
    }

    private int randomDice()
    {
        int dice = Random.Range(1,7);
        return dice;
    }

    private void sendDice(int dice,BasePlayer player)
    {
        Debug.Log(player.dice);
        player.dice = dice;
        Debug.Log(player.dice);
    }
}
