using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HexagonWalk : Tile
{
    [SerializeField] public bool _isWalkable = false;
    public BaseUnit OccupiedUnit;
    public Tile[,] laddresList;

    void Awake()
    {
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
    }

    void OnMouseOver()
    {
        if (this.OccupiedUnit == null && _isWalkable)
        {
            this.transform.GetComponent<SpriteRenderer>().sprite = _cellDefault[this.TileType];
        }

        
    }

    void OnMouseExit()
    {
        if (this.OccupiedUnit == null && _isWalkable)
        {
            this.transform.GetComponent<SpriteRenderer>().sprite = _cellHover[this.TileType];
        }
        
    }

    void OnMouseDown()
    {
        if (GameManager.Instance.GameState != GameState.PlayerTurn) return;
        if (OccupiedUnit == null && this._isWalkable)
        {
            if (UnitManager.Instance.SelectedPlayer != null && Dice.Instance.value > 0)
            {
                SetUnit(UnitManager.Instance.SelectedPlayer);
                if (this.TileType == 3)
                {
                    this.OccupiedUnit = null;
                    UnitManager.Instance.SelectedPlayer.OccupiedTile = TheWayPass.Instance.GetLadder().GetComponent<HexagonWalk>();
                    UnitManager.Instance.SelectedPlayer.OccupiedTile.gameObject.GetComponent<HexagonWalk>().SetUnit(UnitManager.Instance.SelectedPlayer);
                }
                else if (this.TileType == 2)
                {
                    WebSocketTheWayPass.Instance.reqEndGame("80");
                }
                UnitManager.Instance.SelectedPlayer.playerMove();
                UnitManager.Instance.SelectedPlayer.resetTile(UnitManager.Instance.SelectedPlayer.set);
            }
            if (UnitManager.Instance.SelectedPlayer.dice != 0)
            {
                UnitManager.Instance.SelectedPlayer.shadeTileFromPlayer(UnitManager.Instance.SelectedPlayer.OccupiedTile.gameObject.GetComponent<HexagonWalk>());
            }
        }
    }

    public void resetTile()
    {
        this.OccupiedUnit = null;
    }

    public void SetUnit(BaseUnit unit)
    {
        if (unit.OccupiedTile != null) unit.OccupiedTile.gameObject.GetComponent<HexagonWalk>().OccupiedUnit = null;
        unit.transform.position = transform.position;
        if (unit.UnitId == "00")
        {
            unit.transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, -2f);
        }
        else if (unit.UnitId == "01")
        {
            unit.transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, -1f);
        }
        unit.transform.position.Set(transform.position.x, transform.position.y + 0.05f, transform.position.z);
        OccupiedUnit = unit;
        unit.OccupiedTile = this;
    }

}
