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
    public int indexPlayer;
    public int dice;
    public bool playerState;
    public int isWinner = -1; //win = 0, lose = 1
    public HashSet<Tile> set = new HashSet<Tile>();

    private void Awake()
    {
        this.dice = 0;
    }

    public void playerClick()
    {
        this.dice -= 1;
        Dice.Instance.moveDisplay.text = $"Press   {this.dice}   more times.";
        if (this.dice == 0)
        {
            Dice.Instance.value = -1;
            WebSocketMinsweeper.Instance.reqNextPlayer("70");
        }
    }

    public void playerMove()
    {
        this.dice -= 1;
        Dice.Instance.walkDisplay.text = $"Move    {this.dice}    times.";
        WebSocketTheWayPass.Instance.reqCell("30", OccupiedTile.x, OccupiedTile.y, OccupiedTile.TileType);
        if (this.dice == 0)
        {
            Dice.Instance.value = -1;
            WebSocketTheWayPass.Instance.reqNextPlayer("70");
        }
    }

    public void resetTile(HashSet<Tile> set)
    {
        foreach (HexagonWalk n in set)
        {
            n._isWalkable = false;
            n.setPrefab(n.TileType);
        }
    }

    public void shadeTileFromPlayer(Tile tile)
    {
        if (tile != null)
        {
            set.Clear();
            set.Add(tile);
            foreach (HexagonWalk n in tile.neighbors)
            {
                n._isWalkable = true;
                set.Add(n);
                n.setPrefabHover(n.TileType);
                if (n.OccupiedUnit != null || (n.x == 0 && n.y == 0))
                {
                    n.setPrefab(n.TileType);
                }
            }
        }
    }
}
