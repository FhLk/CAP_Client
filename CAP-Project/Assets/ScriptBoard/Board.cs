using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    static Board Instance;
    public Dictionary<string, Tile> _tiles;
    [SerializeField] public HexagonWalk hexPrefab;
    [SerializeField] public Transform _cam;
    public WebsocketGame websocket;


    public int width;
    public int height;
    public float xOffset = 1f;
    public float yOffset = 0.8f;

    public Tile[,] board;

    private void Awake()
    {
        Instance = this;
    }

    public Tile[,] generateBoard()
    {
        this._tiles = new Dictionary<string, Tile>();
        Tile[,] initBoard = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xPos = x * xOffset;
                if (y % 2 == 0)
                {
                    xPos += xOffset / 2f;
                }
                initBoard[x, y] = CreateTile(x, y, xPos, yOffset, "Hex_", 0);
            }
        }
        setNeighbors(initBoard);
        _cam.transform.position = new Vector3((float)width / 1.958f - 0.5f, (float)height / 2.5f - 1.0f, -10);
        board = initBoard;
        return board;
    }

    public void stateRequest(int num)
    {

        if (websocket.role.isHost)
        {
            websocket.reqBomb("90", this.height, this.width, num);
        }

    }

    public void disableCell(Tile tile)
    {
        tile.disableTile();
        updateNeighbor(tile, board);
        WebSocketMinsweeper.Instance.reqEndGame("80");
    }

    public void removeCell(Tile tile)
    {
        if (tile != null)
        {
            Destroy(tile.gameObject);
            updateNeighbor(tile, board);
        }
    }

    public void removeCell(int x, int y)
    {
        if (board[x, y] != null)
        {
            if (board[x, y].TileType != 1)
            {
                Destroy(board[x, y].gameObject);
                updateNeighbor(board[x, y], board);
            }
            else
            {
                board[x, y].disableTile();
                updateNeighbor(board[x, y], board);
            }
        }
    }

    private void setNeighbors(Tile[,] board)
    {
        foreach (Tile cell in board)
        {
            if (cell == null) continue;
            //down
            cell.addNeighbors(cell.x, cell.y - 1, board);
            //up
            cell.addNeighbors(cell.x, cell.y + 1, board);
            //left
            cell.addNeighbors(cell.x - 1, cell.y, board);
            //right
            cell.addNeighbors(cell.x + 1, cell.y, board);
            if (cell.y % 2 == 0)
            {
                cell.addNeighbors(cell.x + 1, cell.y - 1, board);
                cell.addNeighbors(cell.x + 1, cell.y + 1, board);

            }
            else
            {
                cell.addNeighbors(cell.x - 1, cell.y - 1, board);
                cell.addNeighbors(cell.x - 1, cell.y + 1, board);
            }
        }
    }

    public void updateNeighbor(Tile cell, Tile[,] board)
    {
        //down
        cell.addNeighbors(cell.x, cell.y - 1, board);
        //up
        cell.addNeighbors(cell.x, cell.y + 1, board);
        //left
        cell.addNeighbors(cell.x - 1, cell.y, board);
        //right
        cell.addNeighbors(cell.x + 1, cell.y, board);
        if (cell.y % 2 == 0)
        {
            cell.addNeighbors(cell.x + 1, cell.y - 1, board);
            cell.addNeighbors(cell.x + 1, cell.y + 1, board);

        }
        else
        {
            cell.addNeighbors(cell.x - 1, cell.y - 1, board);
            cell.addNeighbors(cell.x - 1, cell.y + 1, board);
        }
        foreach (Tile n in cell.neighbors)
        {
            n.neighbors.Clear();
            //down
            n.addNeighbors(n.x, n.y - 1, board);
            //up
            n.addNeighbors(n.x, n.y + 1, board);
            //left
            n.addNeighbors(n.x - 1, n.y, board);
            //right
            n.addNeighbors(n.x + 1, n.y, board);
            if (n.y % 2 == 0)
            {
                n.addNeighbors(n.x + 1, n.y - 1, board);
                n.addNeighbors(n.x + 1, n.y + 1, board);

            }
            else
            {
                n.addNeighbors(n.x - 1, n.y - 1, board);
                n.addNeighbors(n.x - 1, n.y + 1, board);
            }
        }
    }

    public Tile CreateTile(int x, int y, float xPos, float yOffset, string namePrefix, int type)
    {
        Tile hex_go = Instantiate(hexPrefab, new Vector3(xPos, y * yOffset, (float)y / 10), Quaternion.identity);
        hex_go.x = x;
        hex_go.y = y;
        hex_go.xPos = xPos;
        hex_go.yOffset = yOffset;
        hex_go.name = namePrefix + x + "_" + y;
        hex_go.TileType = type;
        hex_go.TileName = namePrefix;
        hex_go.setPrefab(type);
        this._tiles[hex_go.name] = hex_go;
        hex_go.transform.SetParent(this.transform);
        hex_go.transform.GetComponent<SpriteRenderer>().sortingOrder = -1;
        if (websocket.role._game1)
        {
            Destroy(hex_go.gameObject.GetComponent<HexagonWalk>());
        }
        else
        {
            Destroy(hex_go.gameObject.GetComponent<HexagonClick>());
        }
        return hex_go;
    }
}
