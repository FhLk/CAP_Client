using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField] public bool _isWalkable = false;

    public string TileName;
    public int TileType;
    public BaseUnit OccupiedUnit;
    public List<Tile> laddresList;

    public List<Tile> neighbors = new List<Tile>();
    public int x;
    public int y;
    public float xPos;
    public float yOffset;
    public HashSet<Tile> setStart = new HashSet<Tile>();

    private Dictionary<int, Sprite> _cellDefault = new Dictionary<int, Sprite>();
    private List<Sprite> _recoures;

    void Awake()
    {
        _recoures = Resources.LoadAll<Sprite>("Cell-Type-Default").ToList();
        for (int i = 0; i < _recoures.Count; i++)
        {
            _cellDefault.Add(i, _recoures[i]);
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
        if (GameMangerTheWayPass.Instance.GameState != GameStateTheWayPass.PlayerTurn) return;
        if (OccupiedUnit == null && this._isWalkable)
        {
            if (UnitManagerTheWayPass.Instance.SelectedPlayer != null && DiceTheWayPass.Instance.value > 0)
            {
                SetUnit(UnitManagerTheWayPass.Instance.SelectedPlayer);
                UnitManagerTheWayPass.Instance.SelectedPlayer.playerMove();
                UnitManagerTheWayPass.Instance.SelectedPlayer.resetTile(UnitManagerTheWayPass.Instance.SelectedPlayer.set);

            }
            if (UnitManagerTheWayPass.Instance.SelectedPlayer.dice != 0)
            {
                UnitManagerTheWayPass.Instance.SelectedPlayer.shadeTileFromPlayer(UnitManagerTheWayPass.Instance.SelectedPlayer.OccupiedTile);
            }
        }
    }

    public void disableTile()
    {
    }

    public void SetUnit(BaseUnit unit)
    {
        if (unit.OccupiedTile != null) unit.OccupiedTile.OccupiedUnit = null;
        unit.transform.position = transform.position;
        unit.transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, -1f);
        unit.transform.position.Set(transform.position.x,transform.position.y + 0.05f,transform.position.z);
        OccupiedUnit = unit;
        unit.OccupiedTile = this;
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

    public void addNeighbors(int x, int y, Tile[,] board)
    {
        if (x < BoardTheWayPass.Instance.width && x >= 0 && y < BoardTheWayPass.Instance.height && y >= 0)
        {
            var cell = board[x, y];
            if (cell != null && !neighbors.Contains(cell))
            {
                neighbors.Add(cell);
            }
        }
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
