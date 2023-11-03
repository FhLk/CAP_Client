using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexagonTile : MonoBehaviour
{
    public string TileName;
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private bool _isWalkable;
    public List<HexagonTile> neighbors = new List<HexagonTile>();
    public int x;
    public int y;

    public BaseUnit OccupiedUnit;
    public bool Walkable => _isWalkable && OccupiedUnit == null;


    public void Init(bool isOffset)
    {
        _renderer.color = isOffset ? _offsetColor : _baseColor;
    }

    void OnMouseDown()
    {
        if (GameManager.Instance.GameState != GameState.PlayerTurn) return;
        if (OccupiedUnit == null)
        {
            if (UnitManager.Instance.SelectedPlayer != null)
            {
                SetUnit(UnitManager.Instance.SelectedPlayer);
                UnitManager.Instance.SetSelectedHero(null);

            }
            UnitManager.Instance.SetSelectedHero((BasePlayer)OccupiedUnit);
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
}
