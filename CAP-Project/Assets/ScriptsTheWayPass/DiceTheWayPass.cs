using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DiceTheWayPass : MonoBehaviour
{
    public static DiceTheWayPass Instance;
    private List<Sprite> _diced;
    [SerializeField] public int value = -1;

    public BasePlayerTheWayPass SelectedPlayer;
    public Image faceDice;
    private Dictionary<int, Sprite> _dic = new Dictionary<int, Sprite>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _diced = Resources.LoadAll<Sprite>("Dice").ToList();
        for (int i = 0; i < _diced.Count; i++)
        {
            _dic.Add(i + 1, _diced[i]);
        }
    }

    void Update()
    {
        if (WebSocketGameTheWayPass.Instance.role.isHost && WebSocketGameTheWayPass.Instance.role.playerTurn == 0)
        {
            faceDice.enabled = true;
        }
        else if (WebSocketGameTheWayPass.Instance.role.isHost && WebSocketGameTheWayPass.Instance.role.playerTurn != 0)
        {
            faceDice.enabled = false;
        }
        if (WebSocketGameTheWayPass.Instance.role.isJoin && WebSocketGameTheWayPass.Instance.role.playerTurn == 1)
        {
            faceDice.enabled = true;
        }
        else if (WebSocketGameTheWayPass.Instance.role.isJoin && WebSocketGameTheWayPass.Instance.role.playerTurn != 1)
        {
            faceDice.enabled = false;
        }
    }

    public void OnButtonPress()
    {
        SelectedPlayer = UnitManagerTheWayPass.Instance.SelectedPlayer;
        StartCoroutine(ShowRandomNumber());
    }

    private int randomDice()
    {
        int dice = Random.Range(1, 7);
        return dice;
    }

    private void sendDice(int dice, BasePlayerTheWayPass player)
    {
        player.dice = dice;
    }
    IEnumerator ShowRandomNumber()
    {
        for (int i = 0; i < 30; i++)
        {
            int randomNumber = Random.Range(1, 7);
            faceDice.GetComponent<Image>().sprite = _dic[randomNumber];
            yield return new WaitForSeconds(0.05f);
        }
        int n = randomDice();
        faceDice.GetComponent<Image>().sprite = _dic[n];
        sendDice(n, SelectedPlayer);
        this.value = n;
        SelectedPlayer.shadeTileFromPlayer(SelectedPlayer.OccupiedTile);
    }
}

