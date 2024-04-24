using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TheWayPass : Board
{
    public static TheWayPass Instance;
    [SerializeField] public int numLadders;
    [SerializeField] private int percentageCellDestroys;
    public Item[,] ladder;

    void Awake()
    {
        Instance = this;
    }

    public void stateRequest()
    {
        if (WebSocketTheWayPass.Instance.role.isJoin)
        {
            websocket.reqLadder("90", this.height, this.width, this.numLadders);
        }
    }
    public void stateDefind()
    {
        defindeFinallCell();
        //destroyCell(percentageCellDestroys);
        defindeCell(numLadders, 3);
    }

    public void defindeFinallCell()
    {
        Tile finalCell = board[this.width - 1, this.height - 1];
        Destroy(finalCell.gameObject);
        board[finalCell.x, finalCell.y] = CreateTile(finalCell.x, finalCell.y, finalCell.xPos, finalCell.yOffset, board[finalCell.x, finalCell.y].setName(2), 2);
        updateNeighbor(board[finalCell.x, finalCell.y], board);
    }

    public void destroyCell(int percentage)
    {
        int full = this.width * this.height;
        int num_tileToDestroy = (full / 100) * percentage;
        List<Tile> tilesToDestroy = new List<Tile>();
        List<Tile> listTile = new List<Tile>();
        List<Tile> neighbors = new List<Tile>();
        foreach (Tile hex in board)
        {
            if (hex.TileType == 0)
            {
                listTile.Add(hex);
            }
        }
        for (int i = 0; i < num_tileToDestroy; i++)
        {
            //Get from Backend later
            Tile hex = listTile[Random.Range(0, listTile.Count)];
            listTile.Remove(hex);
            if (!tilesToDestroy.Contains(hex))
            {
                tilesToDestroy.Add(hex);
                /*
                neighbors = hex.neighbors;//00
                for (int j = tilesToDestroy.Count - 1; j >= 0; j--)
                {
                    //00 11 
                    Tile cell = tilesToDestroy[j];
                    if (neighbors.Contains(cell))
                    {
                        tilesToDestroy.RemoveAt(j);
                    }
                }
                */
            }
        }
        foreach (Tile tile in tilesToDestroy)
        {
            Destroy(tile.gameObject);
            board[tile.x, tile.y] = null;
        };
    }

    private void defindeCell(int count, int type)
    {
        List<Tile> tilesToItem = new List<Tile>();

        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < ladder.GetLength(0); j++)
            {
                Tile hex = board[this.ladder[i, j].x, this.ladder[i, j].y];
                if (!tilesToItem.Contains(hex))
                {
                    tilesToItem.Add(hex);
                }
            }
        }
        foreach (Tile tile in tilesToItem)
        {
            Destroy(tile.gameObject);
            board[tile.x, tile.y] = CreateTile(tile.x, tile.y, tile.xPos, tile.yOffset, board[tile.x, tile.y].setName(type), type);
            updateNeighbor(board[tile.x, tile.y], board);
        };
    }

    public HexagonWalk GetTile()
    {
        return board[0, 0].GetComponent<HexagonWalk>();
    }

    public Tile GetTileJoin()
    {
        return board[0, 1];
    }

    public Tile GetLadder()
    {
        List<Tile> listTile = new List<Tile>();
        foreach (HexagonWalk hex in board)
        {
            if (hex.TileType == 0  && hex != board[0,0] && hex != board[this.width - 1,this.height -1] && hex.OccupiedUnit == null)
            {
                listTile.Add(hex);
            }
        }
        Tile randomHex = listTile[Random.Range(0, listTile.Count)];
        return randomHex;
        /*
        for (int i = 0; i < numLadders;i++)
        {
            for (int j = 0; j < 2; j++)
            {
                if (ladderUp.x == board[ladder[i,j].x, ladder[i, j].y].x)
                {
                    if( j == 1)
                    {
                        return board[ladder[i, 0].x, ladder[i, 0].y];
                    }
                    else if ( j == 0)
                    {
                        return board[ladder[i, 1].x, ladder[i, 1].y];
                    }
                }
            }
        }
        return null;
        */
    }
}


