using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] public BasePlayer SelectedPlayer;
    [SerializeField] public GameObject UI;
    [SerializeField] public Text player_text;
    void Awake()
    {
        Instance = this;
        player_text.text = WebsocketLobby.Instance.role.playerName;
    }

    public void showTurnOfWho(int _p)
    {
        for (int i = 0; i < UI.transform.childCount - 1; i++)
        {
            Transform player = UI.transform.GetChild(i);
            if (_p != i)
            {
                player.GetComponent<BasePlayer>().GetComponent<SpriteRenderer>().color = new Color(0.6037736f, 0.6037736f, 0.6037736f);
            }
            else
            {
                player.GetComponent<BasePlayer>().GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }
}
