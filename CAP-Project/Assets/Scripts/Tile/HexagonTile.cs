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
    public bool isDisplay;

    void Awake()
    {
        isActive = true;
        isDisplay = false;
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
        if (this.TileType == 1 && !isActive && isDisplay)
        {
            this.transform.GetComponent<SpriteRenderer>().sprite = _cellDisable[this.TileType];
        }
        else if (this.TileType != 1 && isActive && isDisplay)
        {
            this.transform.GetComponent<SpriteRenderer>().sprite = _cellHover[this.TileType];
        }
        else
        {
            this.transform.GetComponent<SpriteRenderer>().sprite = _cellHover[0];
        }
    }

    void OnMouseExit()
    {
        if (this.TileType == 1 && !isActive && isDisplay)
        {
            this.transform.GetComponent<SpriteRenderer>().sprite = _cellDisable[this.TileType];
        }
        else if (this.TileType != 1 && isActive && isDisplay)
        {
            this.transform.GetComponent<SpriteRenderer>().sprite = _cellDefault[this.TileType];
        }
        else
        {
            this.transform.GetComponent<SpriteRenderer>().sprite = _cellDefault[0];
        }
    }

    void OnMouseDown()
    {
        if (GameManager.Instance.GameState != GameState.PlayerTurn) return;
        if (Dice.Instance.value != -1 && this.isActive)
        {
            if (this.TileType == 1)
            {
                isActive = false;
                isDisplay = true;
                UnitManager.Instance.DecreaseHeart();
                this.transform.GetComponent<SpriteRenderer>().sprite = _cellDisable[this.TileType];
            }
            else if(this.TileType == 2)
            {
                //content of event
            }
            else if (this.TileType == 3)
            {
                UnitManager.Instance.IncreaseHeart();
            }
            else if (this.TileType == 4)
            {
                MouseManager.Instance.onResetBoard();
            }
            else if (this.TileType == 5)
            {
                UnitManager.Instance.ActiveSheild();
            }
            if (this.TileType != 1)
            {
                WebsocketCLI.Instance.reqDataInGame("30",this.x,this.y);
                GameObject.Destroy(gameObject);
            }
            UnitManager.Instance.SelectedPlayer.playerClick();
        }
        //Destroy(this);
        /*if (OccupiedUnit == null)
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
        }*/
    }

    public void SetUnit(BaseUnit unit)
    {
        /*if (unit.OccupiedTile != null) unit.OccupiedTile.OccupiedUnit = null;
        //unit.transform.position = transform.position;
        unit.transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
        //unit.transform.position.Set(transform.position.x,transform.position.y + 0.05f,transform.position.z);
        OccupiedUnit = unit;
        unit.OccupiedTile = this;*/
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
            return "Heart_";
        }
        else if (t == 4)
        {
            return "Reset_";
        }
        return "Hex_";
    }

    public void setPrefab(int t)
    {
        this.transform.GetComponent<SpriteRenderer>().sprite = _cellDefault[t];
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
