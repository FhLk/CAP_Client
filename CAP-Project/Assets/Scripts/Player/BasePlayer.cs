using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BasePlayer : BaseUnit
{
    public string id;
    public string playerName;
    public int dice = 0;
    private int countWalk = 1;
    public HashSet<HexagonTile> set = new HashSet<HexagonTile>();

    public void shadeTileFromPlayer(HexagonTile tile)
    {
        if (tile != null)
        {
            set.Clear();
            set.Add(tile);
            foreach (HexagonTile n in tile.neighbors)
            {
                //n.change(Color.yellow);
                n.change(countWalk);
                n._isWalkable = true;
                set.Add(n);
            }
        }
    }

    public void resetTile(HashSet<HexagonTile> set)
    {
        foreach (HexagonTile n in set)
        {
            if (n.TileType == 1 || n.TileType == 2) { n.change(Color.red); }
            else if (n.TileType == 3) { n.change(new Color(0f, 0.3690929f, 1f)); }
            else if (n.TileType == 4) { n.change(new Color(0.8345884f, 0f, 0.9150943f)); }
            else { n.change(Color.white); }
            n._isWalkable = false;
            n._textDice.text = "";
        }
    }

    public void playerMove()
    {
        countWalk++;
        this.dice -= 1;
        if(this.dice == 0)
        {
            countWalk = 1;
            Dice.Instance.dice.text = $"Dice ({this.dice.ToString()})";
        }
    }
}
