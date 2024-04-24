using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public string TileName;
    public int TileType;
    public List<Sprite> _recoures;
    public int x;
    public int y;
    public Dictionary<int, Sprite> _cellHover = new Dictionary<int, Sprite>();
    public Dictionary<int, Sprite> _cellDisable = new Dictionary<int, Sprite>();
    public Dictionary<int, Sprite> _cellDefault = new Dictionary<int, Sprite>();
    public float xPos;
    public float yOffset;
    public HashSet<Tile> setStart = new HashSet<Tile>();
    public List<Tile> neighbors = new List<Tile>();

    public string setName(int t)
    {
        if (t == 1)
        {
            return "Bomb_";
        }
        else if (t == 2)
        {
            return "Event_";
        }
        else if (t == 3)
        {
            return "Ladder_";
        }
        return "Hex_";
    }

    public void setPrefab(int t)
    {
        this.transform.GetComponent<SpriteRenderer>().sprite = _cellDefault[t];
    }

    public void disableTile()
    {
        this.transform.GetComponent<SpriteRenderer>().sprite = _cellDisable[this.TileType];
    }

    public void addNeighbors(int x, int y, Tile[,] board)
    {
        if (x < board.GetLength(0) && x >= 0 && y < board.GetLength(1) && y >= 0)
        {
            var cell = board[x, y];
            if (cell != null && !neighbors.Contains(cell))
            {
                neighbors.Add(cell);
            }
        }
    }

    public void setPrefabHover(int t)
    {
        this.transform.GetComponent<SpriteRenderer>().sprite = _cellHover[t];
    }

    public void shadeTileFromTile(Tile tile, int r)
    {
        if (tile != null)
        {
            int countCircle = 0;
            setStart.Clear();
            setStart.Add(tile);
            foreach (Tile n in tile.neighbors)
            {
                setStart.Add(n);
            }
            countCircle++;
            while (countCircle < r)
            {
                List<Tile> nextCircleTiles = new List<Tile>();
                foreach (Tile n in setStart)
                {
                    foreach (Tile neighbor in n.neighbors)
                    {
                        //neighbor.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.black;
                        nextCircleTiles.Add(neighbor);
                        if (!setStart.Contains(neighbor) && !nextCircleTiles.Contains(neighbor))
                        {
                            //neighbor.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.black;
                            nextCircleTiles.Add(neighbor);
                        }
                    }
                }
                setStart.AddRange(nextCircleTiles);
                countCircle++;
            }
        }
    }
}
