using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class HexagonClick : Tile
{
    public bool isDisable;
    public bool hidden;


    void Awake()
    {
        isDisable = false;
        hidden = true;
        this._recoures = Resources.LoadAll<Sprite>("Cell-Type-Default").ToList();
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
        if (!this.isDisable && this.TileType != 1)
        {
            this.transform.GetComponent<SpriteRenderer>().sprite = _cellHover[this.TileType];
        }
        else if (this.TileType == 1 && this.hidden)
        {
            this.transform.GetComponent<SpriteRenderer>().sprite = _cellHover[0];
        }
    }

    void OnMouseExit()
    {
        if (!this.isDisable && this.TileType != 1)
        {
            this.transform.GetComponent<SpriteRenderer>().sprite = _cellDefault[this.TileType];
        }
        else if (this.TileType == 1 && this.hidden)
        {
            this.transform.GetComponent<SpriteRenderer>().sprite = _cellDefault[0];
        }
    }

    void OnMouseDown()
    {
        CellClick(this);
    }
}
