using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BasePlayerTheWayPass : BaseUnit
{
    public string id;
    public string playerName;
    public int indexPlayer;
    public int dice = 0;
    public bool playerState;
    public int isWinner = -1; //win = 0, lose = 1
    public HashSet<Tile> set = new HashSet<Tile>();

    private void Awake()
    {
        this.dice = 0;
    }

    public void playerMove()
    {
        this.dice -= 1;
        WebSocketGameTheWayPass.Instance.reqCell("30", OccupiedTile.x, OccupiedTile.y, OccupiedTile.TileType);
        if (this.dice == 0)
        {
            DiceTheWayPass.Instance.value = -1;
            WebSocketGameTheWayPass.Instance.reqNextPlayer("70");
        }
    }

    public void resetTile(HashSet<Tile> set)
    {
        foreach (Tile n in set)
        {
            n._isWalkable = false;
        }
    }

    public void shadeTileFromPlayer(Tile tile)
    {
        if (tile != null)
        {
            set.Clear();
            set.Add(tile);
            foreach (Tile n in tile.neighbors)
            {
                n._isWalkable = true;
                set.Add(n);
            }
        }
    }
}
