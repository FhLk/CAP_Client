using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance;
    [SerializeField] private GameObject BOARD;
    [SerializeField] public BaseUnit PLAYER;
    private int genBomb = 0;
    void Awake()
    {
        Instance = this;
    }
    public void onResetBoard()
    {
        foreach (Transform cell in BOARD.transform)
        {
            GameObject.Destroy(cell.gameObject);
        }
        GameManager.Instance.ChangeState(GameState.GenerateBoard);
        Dice.Instance.value = -1;
        UnitManager.Instance.SelectedPlayer.dice = 0;
        UIManager.Instance.showTurnOfWho(1);
        this.genBomb = 0;
    }

    public void onGenerateBomb()
    {
        this.genBomb++;
        Board_Cell.Instance.defindeBoomCell(1);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Target: " + hit.collider.gameObject.name);
                //Board_Cell.Instance.removeCell(hit.collider.gameObject.GetComponent<HexagonTile>());
            }

        }
    }

}
