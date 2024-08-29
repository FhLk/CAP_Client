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
    [SerializeField] public Text moveDisplay;
    public WebsocketGame Websocket;

    public BasePlayer SelectedPlayer;
    public Image faceDice;
    private Dictionary<int, Sprite> _dic = new Dictionary<int, Sprite>();

    [SerializeField] public Text walkDisplay;
    private bool isRandom = false;
    public TimeManagement timeManagement;

    void Awake()
    {
        Instance = this;
        if (Websocket.role.isJoin)
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
        faceDice.sprite = _diced[0];
    }

    void Update()
    {
        if (Websocket.role.isHost && Websocket.role.playerTurn == 0)
        {
            faceDice.enabled = true;
        }
        else if(Websocket.role.isHost && Websocket.role.playerTurn != 0)
        {
            faceDice.enabled = false;
            faceDice.sprite = _diced[0];
        }
        if(Websocket.role.isJoin && Websocket.role.playerTurn == 1)
        {
            faceDice.enabled = true;
        }
        else if(Websocket.role.isJoin && Websocket.role.playerTurn != 1)
        {
            faceDice.enabled = false;
            faceDice.sprite = _diced[0];
        }
    }

    public void OnButtonPress()
    {
        SelectedPlayer = UnitManager.Instance.SelectedPlayer;
        if (SelectedPlayer.dice == 0 && !isRandom)
        {
            isRandom = true;
            if (Websocket.role._game1)
            {
                StartCoroutine(DisplayNumberOnGameMinesweeper());
            }
            else
            {
                StartCoroutine(DisplayNumberOnGameTheWayPass());
            }
        }
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
    IEnumerator DisplayNumberOnGameMinesweeper()
    {
        for (int i = 0; i < 30; i++)
        {
            int randomNumber = Random.Range(1, 7);
            faceDice.GetComponent<Image>().sprite = _dic[randomNumber];
            yield return new WaitForSeconds(0.05f);
        }
        isRandom = false;
        int n = randomDice();
        faceDice.GetComponent<Image>().sprite = _dic[n];
        sendDice(n, SelectedPlayer);
        this.value = n;
        SetText(n);
        //Websocket.reqRollDice("-40",this.value);
    }

    IEnumerator DisplayNumberOnGameTheWayPass()
    {
        for (int i = 0; i < 30; i++)
        {
            int randomNumber = Random.Range(1, 7);
            faceDice.GetComponent<Image>().sprite = _dic[randomNumber];
            yield return new WaitForSeconds(0.05f);
        }
        isRandom = false;
        int n = randomDice();
        faceDice.GetComponent<Image>().sprite = _dic[n];
        sendDice(n, SelectedPlayer);
        this.value = n;
        SetText(n);
        SelectedPlayer.shadeTileFromPlayer(SelectedPlayer.OccupiedTile);
        //Websocket.reqRollDice("-40", this.value);
    }

    public void SetText(int value)
    {
        if (UnitManager.Instance.role._game1)
        {
            moveDisplay.text = $"Press   {value}   more times.";
        }
        else if (UnitManager.Instance.role._game2)
        {
            walkDisplay.text = $"Move    {value}    times.";
        }
    }
}

