using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Board_Cell : MonoBehaviour
{
    public static Board_Cell Instance;
    public Dictionary<string, HexagonTile> _tiles;
    [SerializeField] private HexagonTile hexPrefab;
    [SerializeField] private Transform _cam;
    [SerializeField] private int numBombs;
    [SerializeField] private int numEvents;
    [SerializeField] private int numResets;
    [SerializeField] private int numHearts;
    [SerializeField] private int numSheilds;
    [SerializeField] public BasePlayer[] players;
    [SerializeField] private WebsocketCLI _websocket;

    public int width;
    public int height;
    public HexagonTile[,] board;
    private int[] cellTypes = { 0,1,2,3,4,5 };

    float xOffset = 1f;
    float yOffset = 0.8f;

    void Awake()
    {
        Instance = this;
    }

    public void generateBoard()
    {
        this._tiles = new Dictionary<string, HexagonTile>();
        HexagonTile[,] initBoard = new HexagonTile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xPos = x * xOffset;
                if (y % 2 == 0)
                {
                    xPos += xOffset / 2f;
                }
                initBoard[x, y] = CreateTile(x, y, xPos, yOffset, "Hex_",0);
            }
        }
        setNeighbors(initBoard);
        initBoard = stateDefind(initBoard);

        _cam.transform.position = new Vector3((float)width / 1.958f - 0.5f, (float)height / 2.5f - 1.0f, -10);
        board = initBoard;
        if (_websocket._action.isHost)
        {
            _websocket.reqDataInGame("50", players, board);
        }
        GameManager.Instance.ChangeState(GameState.SpawnPlayer);
        GameManager.Instance.ChangeState(GameState.PlayerTurn);
    }

    private HexagonTile[,] stateDefind(HexagonTile[,] initBoard)
    {
        HexagonTile[,] board = initBoard;
        board = defindeCell(board, numBombs, 1);
        board = defindeCell(board, numEvents, 2);
        board = defindeCell(board, numHearts, 3);
        board = defindeCell(board, numResets, 4);
        board = defindeCell(board, numSheilds, 5);
        return board;
    }

    public void removeCell(int x,int y)
    {
        if (board[x, y] != null)
        {
            Destroy(board[x, y].gameObject);
            updateNeighbor(board[x, y], board);
        }
    }

    private HexagonTile[,] defindeCell(HexagonTile[,] board, int count,int type)
    {
        int total = count;
        List<HexagonTile> tilesToItem = new List<HexagonTile>();
        List<HexagonTile> neighbors = new List<HexagonTile>();
        List<HexagonTile> listTile = new List<HexagonTile>();

        foreach (HexagonTile hex in board)
        {
            if (hex.TileType == 0)
            {
                listTile.Add(hex);
            }
        } 
        while (tilesToItem.Count < total)
        {
            if (listTile.Count <= (this.width * this.height)){
                HexagonTile randomHex = listTile[Random.Range(0, listTile.Count)];
                listTile.Remove(randomHex);
                if (!tilesToItem.Contains(randomHex) && randomHex != null)
                {
                    if (randomHex.TileType == 0)
                    {
                        tilesToItem.Add(randomHex);
                        neighbors = randomHex.neighbors;
                        for (int i = tilesToItem.Count - 1; i >= 0; i--)
                        {
                            HexagonTile cell = tilesToItem[i];
                            if (neighbors.Contains(cell))
                            {
                                tilesToItem.RemoveAt(i);
                            }
                        }
                    }
                }
            }
        }
        foreach (HexagonTile tile in tilesToItem)
        {
            Destroy(tile.gameObject);
            board[tile.x, tile.y] = CreateTile(tile.x, tile.y, tile.xPos, tile.yOffset, board[tile.x, tile.y].setName(type), type);
            updateNeighbor(board[tile.x, tile.y], board);
        }
        return board;
    }

    private void setNeighbors(HexagonTile[,] board)
    {
        foreach (HexagonTile cell in board)
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

    private void updateNeighbor(HexagonTile cell, HexagonTile[,] board)
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
        foreach (HexagonTile n in cell.neighbors)
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

    private HexagonTile CreateTile(int x, int y, float xPos, float yOffset, string namePrefix, int type)
    {
        HexagonTile hex_go = Instantiate(hexPrefab, new Vector3(xPos, y * yOffset, (float)y / 10), Quaternion.identity);
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
        return hex_go;
    }
}
