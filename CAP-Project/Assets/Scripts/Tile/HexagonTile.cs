using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class HexagonTile : MonoBehaviour
{
    public string TileName;
    public int TileType;
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    public bool _isWalkable;
    public List<HexagonTile> neighbors = new List<HexagonTile>();
    public int x;
    public int y;
    public float xPos;
    public float yOffset;
    public HashSet<HexagonTile> setStart = new HashSet<HexagonTile>();
    public BaseUnit OccupiedUnit;
    public bool Walkable => _isWalkable && OccupiedUnit == null;

    public void Init(bool isOffset)
    {
        _renderer.color = isOffset ? _offsetColor : _baseColor;
    }

    void OnMouseDown()
    {
        if (GameManager.Instance.GameState != GameState.PlayerTurn) return;
        if (OccupiedUnit == null && this._isWalkable)
        {
            if (UnitManager.Instance.SelectedPlayer != null)
            {
                UnitManager.Instance.SelectedPlayer.playerMove();
                UnitManager.Instance.SelectedPlayer.resetTile(UnitManager.Instance.SelectedPlayer.set);
                SetUnit(UnitManager.Instance.SelectedPlayer);
                UnitManager.Instance.SetSelectedPlayer(null);
            }
            UnitManager.Instance.SetSelectedPlayer((BasePlayer)OccupiedUnit);
            if(UnitManager.Instance.SelectedPlayer.OccupiedTile.TileType == 2)
            {
                SceneManager.LoadScene("End");
            }
            if (UnitManager.Instance.SelectedPlayer.dice != 0)
            {
                UnitManager.Instance.SelectedPlayer.shadeTileFromPlayer(UnitManager.Instance.SelectedPlayer.OccupiedTile);
            }
        }
    }

    public void SetUnit(BaseUnit unit)
    {
        if (unit.OccupiedTile != null) unit.OccupiedTile.OccupiedUnit = null;
        //unit.transform.position = transform.position;
        unit.transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
        //unit.transform.position.Set(transform.position.x,transform.position.y + 0.05f,transform.position.z);
        OccupiedUnit = unit;
        unit.OccupiedTile = this;
    }

    public void addNeighbors(int x, int y, HexagonTile[,] board)
    {
        if (x < Board_Cell.Instance.width && x >= 0 && y < Board_Cell.Instance.height && y >= 0)
        {
            var cell = board[x, y];
            if (cell != null && !neighbors.Contains(cell))
            {
                neighbors.Add(cell);
            }
        }
    }

    public void change(Color color)
    {
        _renderer.color = color;
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
                        if (!setStart.Contains(neighbor) && !nextCircleTiles.Contains(neighbor))
                        {
                            //neighbor.change(Color.gray);
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
