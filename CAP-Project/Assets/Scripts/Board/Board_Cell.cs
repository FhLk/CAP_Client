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
    [SerializeField] private HexagonTile bombPrefab;
    [SerializeField] private Transform _cam;
    [SerializeField] private float percentage;

    public int width;
    public int height;
    private HexagonTile[,] board;

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
                initBoard[x, y] = CreateTile(x, y, xPos, yOffset, "Hex_", hexPrefab);
            }
        }
        setNeighbors(initBoard);
        defindeBombCell(initBoard);

        _cam.transform.position = new Vector3((float)width / 1.958f - 0.5f, (float)height / 2.5f - 1.0f, -10);

        GameManager.Instance.ChangeState(GameState.SpawnPlayer);
        GameManager.Instance.ChangeState(GameState.PlayerTurn);
        board = initBoard;
    }

    public void removeCell(HexagonTile cell)
    {
        int x = cell.x;
        int y = cell.y;
        if (board[x, y] != null)
        {
            Destroy(board[x, y].gameObject);
            updateNeighbor(board[x, y], board);
        }
    }

    private HexagonTile[,] defindeBombCell(HexagonTile[,] board)
    {
        int totalEvent = 0;
        List<HexagonTile> tilesToEvent = new List<HexagonTile>();
        List<HexagonTile> neighbors = new List<HexagonTile>();
        while (tilesToEvent.Count < totalEvent)
        {
            int x = Random.Range(0, this.width);
            int y = Random.Range(0, this.height);
            if (!tilesToEvent.Contains(board[x, y]) && board[x, y] != null)
            {
                if (board[x, y].TileType != 1 || board[x, y].TileType != 2)
                {
                    tilesToEvent.Add(board[x, y]);
                    neighbors = board[x, y].neighbors;
                    for (int i = tilesToEvent.Count - 1; i >= 0; i--)
                    {
                        HexagonTile cell = tilesToEvent[i];
                        if (neighbors.Contains(cell))
                        {
                            tilesToEvent.RemoveAt(i);
                        }
                    }
                }
            }
        }
        foreach (HexagonTile tile in tilesToEvent)
        {
            Destroy(tile.gameObject);
            board[tile.x, tile.y] = CreateTile(tile.x, tile.y, tile.xPos, tile.yOffset, "Bomb_", bombPrefab);
            updateNeighbor(board[tile.x, tile.y], board);
            //tile.shadeTileFromTile(tile, 2);
        }
        return board;
    }

    public HexagonTile[,] defindeBoomCell(int bomb)
    {
        int totalEvent = bomb;
        List<HexagonTile> tilesToEvent = new List<HexagonTile>();
        List<HexagonTile> neighbors = new List<HexagonTile>();
        while (tilesToEvent.Count < totalEvent)
        {
            int x = 0;
            int y = 0;
            if (!tilesToEvent.Contains(board[x, y]) && board[x, y] != null)
            {
                if (board[x, y].TileType != 1 || board[x, y].TileType != 2)
                {
                    tilesToEvent.Add(board[x, y]);
                    neighbors = board[x, y].neighbors;
                    for (int i = tilesToEvent.Count - 1; i >= 0; i--)
                    {
                        HexagonTile cell = tilesToEvent[i];
                        if (neighbors.Contains(cell))
                        {
                            tilesToEvent.RemoveAt(i);
                        }
                    }
                }
            }
        }
        foreach (HexagonTile tile in tilesToEvent)
        {
            Destroy(tile.gameObject);
            board[tile.x, tile.y] = CreateTile(tile.x, tile.y, tile.xPos, tile.yOffset, "Bomb_", bombPrefab);
            updateNeighbor(board[tile.x, tile.y], board);
            tile.shadeTileFromTile(tile, 2);
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

    private HexagonTile CreateTile(int x, int y, float xPos, float yOffset, string namePrefix, HexagonTile prefab)
    {
        HexagonTile hex_go = Instantiate(prefab, new Vector3(xPos, y * yOffset, (float)y / 10), Quaternion.identity);
        hex_go.x = x;
        hex_go.y = y;
        hex_go.xPos = xPos;
        hex_go.yOffset = yOffset;
        hex_go.name = namePrefix + x + "_" + y;
        this._tiles[hex_go.name] = hex_go;
        hex_go.transform.SetParent(this.transform);
        return hex_go;
    }
}
