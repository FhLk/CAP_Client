using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class BoardMinesweeper : MonoBehaviour
{
    public static BoardMinesweeper Instance;
    public Dictionary<string, HexagonTile> _tiles;
    [SerializeField] private HexagonTile hexPrefab;
    [SerializeField] private Transform _cam;
    [SerializeField] private int numBombs;
    [SerializeField] private int numEvents;
    [SerializeField] private int numResets;
    [SerializeField] private int numHearts;
    [SerializeField] private int numSheilds;
    [SerializeField] public BasePlayer[] players;

    public int width;
    public int height;
    public HexagonTile[,] board;
    public List<Item> bombs;

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
                initBoard[x, y] = CreateTile(x, y, xPos, yOffset, "Hex_", 0);
            }
        }
        setNeighbors(initBoard);
        _cam.transform.position = new Vector3((float)width / 1.958f - 0.5f, (float)height / 2.5f - 1.0f, -10);
        board = initBoard;
        stateRequest();
    }

    private void stateRequest()
    {
        if (WebSocketGame.Instance.role.isJoin)
        {
            WebSocketGame.Instance.reqBomb("90", this.height, this.width, this.numBombs);
        }
        else
        {
            WebSocketGame.Instance.reqBomb("90", this.height, this.width, this.numBombs);
        }


    }
    public void stateDefind()
    {
        defindeCell(numBombs, 1);
        /*
        board = defindeCell(board, numEvents, 2);
        board = defindeCell(board, numHearts, 3);
        board = defindeCell(board, numResets, 4);
        board = defindeCell(board, numSheilds, 5);
        */
    }

    public void removeCell(HexagonTile tile)
    {
        if (tile != null)
        {
            Destroy(tile.gameObject);
            updateNeighbor(tile, board);
        }
    }

    public void disableCell(HexagonTile tile)
    {
        tile.disableTile();
        updateNeighbor(tile, board);
        WebSocketGame.Instance.reqEndGame("80");
    }

    private void defindeCell(int count, int type)
    {
        List<HexagonTile> tilesToItem = new List<HexagonTile>();
        List<HexagonTile> neighbors = new List<HexagonTile>();
        for (int i = 0; i < count; i++)
        {
            HexagonTile hex = board[this.bombs[i].x, this.bombs[i].y];
            Debug.Log(hex.x + " " + hex.y);
            if (!tilesToItem.Contains(hex))
            {
                tilesToItem.Add(hex);
            }
        }
        foreach (HexagonTile tile in tilesToItem)
        {
            Destroy(tile.gameObject);
            CreateTileBomb(tile.x, tile.y, tile.xPos, tile.yOffset, board[tile.x, tile.y].setName(type), type);
            updateNeighbor(board[tile.x, tile.y], board);
        }
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

    private void CreateTileBomb(int x, int y, float xPos, float yOffset, string namePrefix, int type)
    {
        Debug.Log(board[x, y]);
        board[x, y] = Instantiate(hexPrefab, new Vector3(xPos, y * yOffset, (float)y / 10), Quaternion.identity);
        board[x, y].x = x;
        board[x, y].y = y;
        board[x, y].xPos = xPos;
        board[x, y].yOffset = yOffset;
        board[x, y].name = namePrefix + x + "_" + y;
        board[x, y].TileType = type;
        board[x, y].TileName = namePrefix;
        board[x, y].setPrefab(type);
        this._tiles[board[x, y].name] = board[x, y];
        board[x, y].transform.SetParent(this.transform);

        //return hex_go;
    }
}
