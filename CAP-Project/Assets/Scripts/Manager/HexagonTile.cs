using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class HexagonTile : MonoBehaviour
{
    public string TileName;
    public int TileType;
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] public Text _textDice;
    [SerializeField] private GameObject _highlight;
    public bool _isWalkable;
    public List<HexagonTile> neighbors = new List<HexagonTile>();
    public int x;
    public int y;

    public BaseUnit OccupiedUnit;
    public bool Walkable => _isWalkable && OccupiedUnit == null;
    private int countWalk = 1;


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
                Debug.Log(countWalk++);
                UnitManager.Instance.SelectedPlayer.playerMove();
                UnitManager.Instance.SelectedPlayer.resetTile(UnitManager.Instance.SelectedPlayer.set);
                SetUnit(UnitManager.Instance.SelectedPlayer);
                UnitManager.Instance.SetSelectedPlayer(null);

            }
            UnitManager.Instance.SetSelectedPlayer((BasePlayer)OccupiedUnit);
            if (UnitManager.Instance.SelectedPlayer.dice != 0)
            {
                UnitManager.Instance.SelectedPlayer.shadeTileFromPlayer(UnitManager.Instance.SelectedPlayer.OccupiedTile);
            }

        }
    }

    public void SetUnit(BaseUnit unit)
    {
        if (unit.OccupiedTile != null) unit.OccupiedTile.OccupiedUnit = null;
        unit.transform.position = transform.position;
        OccupiedUnit = unit;
        unit.OccupiedTile = this;
    }

    public void addNeighbors(int x, int y, HexagonTile[,] board)
    {
        if (x < Board_Cell.Instance.width && x >= 0 && y < Board_Cell.Instance.height && y >= 0)
        {
            var cell = board[x, y];
            if (cell != null)
            {
                neighbors.Add(cell);
            }
        }
    }

    public void change(Color color)
    {
        _renderer.color = color;
    }    
    public void change(int walk)
    {
        _textDice.text = walk.ToString();
    }
}
