using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Minesweeper : Board
{
    public static Minesweeper Instance;
    [SerializeField] public BasePlayer[] players;
    [SerializeField] public int numBombs;
    public List<Item> bombs;

    void Awake()
    {
        Instance = this;
    }

    public void stateDefind()
    {
        this.defindeCell(numBombs, 1);
    }

    public void defindeCell(int count, int type)
    {
        List<Tile> tilesToItem = new List<Tile>();
        List<Tile> neighbors = new List<Tile>();
        for (int i = 0; i < count; i++)
        {
            Debug.Log("wow");
            Tile hex = board[this.bombs[i].x, this.bombs[i].y];
            Debug.Log(hex.x + " " + hex.y);
            if (!tilesToItem.Contains(hex))
            {
                tilesToItem.Add(hex);
            }
        }
        foreach (Tile tile in tilesToItem)
        {
            Destroy(tile.gameObject);
            board[tile.x, tile.y] = this.CreateTile(tile.x, tile.y, tile.xPos, tile.yOffset, board[tile.x, tile.y].setName(type), type);
            updateNeighbor(board[tile.x, tile.y], board);
        };
    }
}
