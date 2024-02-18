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
    public int indexPlayer;
    public int dice;
    private int maxHealth = 4;
    public int maxShield = 1;
    public int hearts;
    [SerializeField] private GameObject _base;
    public bool playerState;
    public bool shield;

    private void Awake()
    {
        this.hearts = 3;
        this.shield = false;
        this.dice = 0;
        if (!this.shield)
        {
            this._base.transform.GetChild(3).gameObject.SetActive(false);
        }
        for (int i = 0; i < this.hearts; i++)
        {
            //this._base.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void playerClick()
    {
        this.dice -= 1;
        if (this.dice == 0)
        {
            Dice.Instance.value = -1;
            WebsocketCLI.Instance.reqDataInGame("70", "123");
            //GameManager.Instance.ChangeState(GameState.NextPlayerTurn);
        }
    }

    public void increaseHearts()
    {
        if (this.hearts < maxHealth)
        {
            this.hearts++;
            this._base.transform.GetChild(  this.hearts - 1).gameObject.SetActive(true);
            if (this.hearts == maxHealth && !this.shield)
            {
                this._base.transform.GetChild(3).gameObject.SetActive(false);
                this.hearts--;
            }
        }

    }

    public void decreaseHearts()
    {
        if (this.shield)
        {
            this.shield = false;
            this._base.transform.GetChild(3).gameObject.SetActive(false);
            this.hearts--;
        }
        else
        {
            if(this.hearts > 0)
            {
                this.hearts--;
                this._base.transform.GetChild(this.hearts).gameObject.SetActive(false);
            }
        }
    }

    public void activeSheild()
    {
        this.shield = true;
        this.hearts++;
        this._base.transform.GetChild(3).gameObject.SetActive(true);
    }

    public void resetPlayer()
    {
        Awake();
    }
}
