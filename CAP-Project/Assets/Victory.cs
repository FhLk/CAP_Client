using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Victory : MonoBehaviour
{
    public WinnerData winner;
    [SerializeField] private Text playerName;

    private void Start()
    {
        playerName.text = winner.playerName;
    }
}
