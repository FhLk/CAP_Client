using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance;
    [SerializeField] private GameObject BOARD;
    [SerializeField] public BaseUnit PLAYER;

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
        GameObject.Destroy(PLAYER.gameObject);
        Board_Cell.Instance.generateBoard();
        Dice.Instance.dice.text = $"Dice ({0.ToString()})";
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
            }

        }
    }


}
