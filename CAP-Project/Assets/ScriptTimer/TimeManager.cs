using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManagement : MonoBehaviour
{
    public float countdown;
    public bool TimerOn = false;

    public Text TimerText;
    public WebsocketGame websocket;

    public void ClockOnOff()
    {
        if (TimerOn) 
        {
            resetTimer();
            //TimerText.text = "10";
        }
        else
        {
            TimerOn = true;
        }
    }

    private void Update()
    {
        if (TimerOn)
        {
            if (countdown > 0 )
            {
                countdown -= Time.deltaTime;
                updateTimer(countdown);
            }
            else
            {
                TimerOn = false;
                if (UnitManager.Instance.role.isHost && UnitManager.Instance.role.playerTurn == 0)
                {
                    if (UnitManager.Instance.role._game1)
                    {
                        Debug.Log(Dice.Instance.value);
                        UnitManager.Instance.SelectedPlayer.RandomCell(Dice.Instance.value);
                    }
                    else if (UnitManager.Instance.role._game2)
                    {
                        UnitManager.Instance.SelectedPlayer.disableTileFromPlayer(UnitManager.Instance.SelectedPlayer.OccupiedTile);
                        websocket.reqNextPlayer("70");
                    }
                }
                else if (UnitManager.Instance.role.isJoin && UnitManager.Instance.role.playerTurn == 1)
                {
                    if (UnitManager.Instance.role._game1)
                    {
                        Debug.Log(Dice.Instance.value);
                        UnitManager.Instance.SelectedPlayer.RandomCell(Dice.Instance.value);
                    }
                    else if (UnitManager.Instance.role._game2)
                    {
                        UnitManager.Instance.SelectedPlayer.disableTileFromPlayer(UnitManager.Instance.SelectedPlayer.OccupiedTile);
                        websocket.reqNextPlayer("70");
                    }
                }
                Dice.Instance.SetText(0);
                UnitManager.Instance.SelectedPlayer.dice = 0;
                Dice.Instance.SelectedPlayer.dice = 0;
                Dice.Instance.value = -1;

            }
        }
    }

    void updateTimer(float currentTime)
    {
        float seconds = Mathf.Floor(currentTime % 60);

        TimerText.text = string.Format("{00}",seconds);
    }

    public void resetTimer()
    {
        TimerOn = false;
        countdown = 11;
        UnitManager.Instance.SelectedPlayer.dice = 0;
        Dice.Instance.value = -1;
        Dice.Instance.SetText(0);
        TimerText.text = "10";
    }
}
