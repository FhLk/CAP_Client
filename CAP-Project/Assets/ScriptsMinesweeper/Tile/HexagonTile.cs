using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class HexagonTile : MonoBehaviour
{
    public string TileName;
    public int TileType;
    private List<Sprite> _recoures;
    private Dictionary<int, Sprite> _cellDefault = new Dictionary<int, Sprite>();
    private Dictionary<int, Sprite> _cellHover = new Dictionary<int, Sprite>();
    private Dictionary<int, Sprite> _cellDisable = new Dictionary<int, Sprite>();
    public List<HexagonTile> neighbors = new List<HexagonTile>();
    public int x;
    public int y;
    public float xPos;
    public float yOffset;
    public HashSet<HexagonTile> setStart = new HashSet<HexagonTile>();
    public bool isActive;
    public bool hidden;

    void Awake()
    {
        isActive = true;
        hidden = false;
        _recoures = Resources.LoadAll<Sprite>("Cell-Type-Default").ToList();
        for (int i = 0; i < _recoures.Count; i++)
        {
            _cellDefault.Add(i, _recoures[i]);
        }
        _recoures = Resources.LoadAll<Sprite>("Cell-Type-Hover").ToList();
        for (int i = 0; i < _recoures.Count; i++)
        {
            _cellHover.Add(i, _recoures[i]);
        }
        _recoures = Resources.LoadAll<Sprite>("Cell-Type-Disable").ToList();
        for (int i = 0; i < _recoures.Count; i++)
        {
            _cellDisable.Add(i, _recoures[i]);
        }
    }

    void OnMouseOver()
    {
    }

    void OnMouseExit()
    {
    }

    void OnMouseDown()
    {
        if (GameManager.Instance.GameState != GameState.PlayerTurn) return;
        if (Dice.Instance.value != -1 && this.isActive)
        {
            if (this.TileType == 1)
            {
                isActive = false;
                hidden = false;
                disableTile();
                WebSocketGame.Instance.reqCell("30", x, y,TileType);
            }

            if (this.TileType != 1)
            {
                WebSocketGame.Instance.reqCell("30", x, y, TileType);
                Destroy(gameObject);
            }
            UnitManager.Instance.SelectedPlayer.playerClick();
        }
    }

    public void disableTile()
    {
        this.transform.GetComponent<SpriteRenderer>().sprite = _cellDisable[this.TileType];
    }

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
        return "Hex_";
    }

    public void setPrefab(int t)
    {
        this.transform.GetComponent<SpriteRenderer>().sprite = _cellDefault[t];
    }

    public void addNeighbors(int x, int y, HexagonTile[,] board)
    {
        if (x < BoardMinesweeper.Instance.width && x >= 0 && y < BoardMinesweeper.Instance.height && y >= 0)
        {
            var cell = board[x, y];
            if (cell != null && !neighbors.Contains(cell))
            {
                neighbors.Add(cell);
            }
        }
    }

    public void shadeTileFromTile(HexagonTile tile, int r)
    {
        if (tile != null)
        {
            int countCircle = 0;
            setStart.Clear();
            setStart.Add(tile);
            foreach (HexagonTile n in tile.neighbors)
            {
                setStart.Add(n);
            }
            countCircle++;
            while (countCircle < r)
            {
                List<HexagonTile> nextCircleTiles = new List<HexagonTile>();
                foreach (HexagonTile n in setStart)
                {
                    foreach (HexagonTile neighbor in n.neighbors)
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
