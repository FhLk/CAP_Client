using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    public static Dice Instance;
    [SerializeField] public Text dice;
    public BasePlayer SelectedPlayer;

    void Awake()
    {
        Instance = this;
    }

    public void OnButtonPress()
    {
        SelectedPlayer = UnitManager.Instance.SelectedPlayer;
        SelectedPlayer.resetTile(SelectedPlayer.set);
        int n = randomDice();
        dice.text = $"Dice ({n.ToString()})";
        sendDice(n, SelectedPlayer);
        SelectedPlayer.shadeTileFromPlayer(SelectedPlayer.OccupiedTile);
    }

    private int randomDice()
    {
        int dice = Random.Range(1,7);
        return dice;
    }

    private void sendDice(int dice,BasePlayer player)
    {
        player.dice = dice;
    }
}
