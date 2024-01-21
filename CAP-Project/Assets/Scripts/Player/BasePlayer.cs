using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BasePlayer : BaseUnit
{
    public string id;
    public string playerName;
    public int dice = 0;
    public HashSet<HexagonTile> set = new HashSet<HexagonTile>();

    public void playerClick()
    {
        this.dice -= 1;
        if(this.dice == 0)
        {
            Dice.Instance.value = -1;
            GameManager.Instance.ChangeState(GameState.NextPlayerTurn);
        }
    }
}
