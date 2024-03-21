using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    public static Dice Instance;
    private List<Sprite> _diced;
    [SerializeField] public int value = -1;

    public BasePlayer SelectedPlayer;
    public Image faceDice;
    private Dictionary<int, Sprite> _dic = new Dictionary<int, Sprite>();

    void Awake()
    {
        Instance = this;
        if (WebSocketGame.Instance.role.isJoin)
        {
            faceDice.enabled = false;
        }
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
        if (WebSocketGame.Instance.role.isHost && WebSocketGame.Instance.role.playerTurn == 0)
        {
            faceDice.enabled = true;
        }
        else if(WebSocketGame.Instance.role.isHost && WebSocketGame.Instance.role.playerTurn != 0)
        {
            faceDice.enabled = false;
        }
        if(WebSocketGame.Instance.role.isJoin && WebSocketGame.Instance.role.playerTurn == 1)
        {
            faceDice.enabled = true;
        }
        else if(WebSocketGame.Instance.role.isJoin && WebSocketGame.Instance.role.playerTurn != 1)
        {
            faceDice.enabled = false;
        }
    }

    public void OnButtonPress()
    {
        SelectedPlayer = UnitManager.Instance.SelectedPlayer;
        StartCoroutine(ShowRandomNumber());
    }

    private int randomDice()
    {
        int dice = Random.Range(1,7);
        return dice;
    }

    private void sendDice(int dice,BasePlayer player)
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
    }
}

