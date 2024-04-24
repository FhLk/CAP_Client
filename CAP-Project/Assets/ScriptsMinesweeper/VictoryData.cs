using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryData : MonoBehaviour
{
    public WinnerData winner;
    [SerializeField] private Text playerName;
    [SerializeField] private GameObject player;
    [SerializeField] private Sprite player_1;
    [SerializeField] private Sprite player_2;

    private void Start()
    {
        playerName.text = winner.playerName;
        if (winner.id == "host")
        {
            player.gameObject.GetComponent<SpriteRenderer>().sprite = player_1;
        }
        else if (winner.id == "join")
        {
            player.gameObject.GetComponent<SpriteRenderer>().sprite = player_2;
        }
    }
}
