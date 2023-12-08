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
    [SerializeField] private HexagonTile eventPrefab;
    [SerializeField] private HexagonTile startPrefab;
    [SerializeField] private HexagonTile finalPrefab;
    [SerializeField] private HexagonTile storePrefab;
    [SerializeField] private HexagonTile ladderPrefab;
    [SerializeField] private HexagonTile TestPrefab;
    [SerializeField] private Transform _cam;
    [SerializeField] private float percentage;

    public int width;
    public int height;

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
        initBoard = removeCell(initBoard);
        setNeighbors(initBoard);
        initBoard = defindeStartCell(initBoard);
        defindeEventCell(initBoard);

        _cam.transform.position = new Vector3((float)width / 1.958f - 0.5f, (float)height / 2.5f - 1.0f, -10);
        GameManager.Instance.ChangeState(GameState.SpawnPlayer);
        GameManager.Instance.ChangeState(GameState.PlayerTurn);

    }

    private HexagonTile[,] removeCell(HexagonTile[,] board)
    {
        int totalTiles = width * height;
        int tileToDestroy = (int)(totalTiles * (this.percentage / 100f));

        // เก็บตำแหน่งของ tiles ที่จะถูกลบ
        List<(int, int)> tilesToRemove = new List<(int, int)>();

        // สุ่มและจัดเก็บตำแหน่งของ tiles ที่จะถูกลบ
        while (tilesToRemove.Count < tileToDestroy)
        {
            int x = Random.Range(0, this.width);
            int y = Random.Range(0, this.height);

            if (!tilesToRemove.Contains((x, y)))
            {
                tilesToRemove.Add((x, y));
            }
        }

        foreach (var (x, y) in tilesToRemove)
        {
            Destroy(board[x, y].gameObject);
            board[x, y] = null;
        }
        return board;
    }

    private HexagonTile[,] defindeStartCell(HexagonTile[,] board)
    {
        int x = Random.Range(0, this.width);
        int y = Random.Range(0, this.height);
        int countCell = 0;
        if (board[x, y] != null)
        {
            Destroy(board[x, y].gameObject);
            board[x, y] = CreateTile(x, y, board[x, y].xPos, board[x, y].yOffset, "Start_", startPrefab);
            updateNeighbor(board[x, y], board);
            board[x, y].shadeTileFromTile(board[x, y], 15);
            foreach (HexagonTile tile in board)
            {
                if (!board[x, y].setStart.Contains(tile) && tile != null)
                {
                    countCell++;
                }
            }
            HexagonTile[] spaeceBoard = new HexagonTile[countCell];
            int i = 0;
            foreach (HexagonTile tile in board)
            {
                if (!board[x, y].setStart.Contains(tile) && tile != null)
                {
                    spaeceBoard[i++] = tile;
                }
            }
            HexagonTile finalCell = defindeFinalCell(spaeceBoard);
            Destroy(board[finalCell.x, finalCell.y].gameObject);
            board[finalCell.x, finalCell.y] = CreateTile(finalCell.x, finalCell.y, finalCell.xPos, finalCell.yOffset, "Final_", finalPrefab);
            updateNeighbor(board[finalCell.x, finalCell.y], board);
        }
        else
        {
            defindeStartCell(board);
        }
        return board;
    }

    private HexagonTile[,] defindeEventCell(HexagonTile[,] board)
    {
        int totalEvent = 6;
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
            board[tile.x, tile.y] = CreateTile(tile.x, tile.y, tile.xPos, tile.yOffset, "Event_", eventPrefab);
            updateNeighbor(board[tile.x, tile.y], board);
            tile.shadeTileFromTile(tile, 2);
        }
        return board;
    }

    private HexagonTile defindeFinalCell(HexagonTile[] board)
    {
        int index = Random.Range(0, board.Length);
        return board[index];
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

    public HexagonTile GetPlayerSpawnTile()
    {
        var spawnTile = this._tiles.FirstOrDefault(t => t.Value.TileType == 1).Value;

        if (spawnTile == null)
        {
            MouseManager.Instance.onResetBoard();
            foreach (Transform tile in this.transform)
            {
               if(tile.gameObject.GetComponent<HexagonTile>().TileType == 1)
                {
                    return tile.gameObject.GetComponent<HexagonTile>();
                }
            }
        }
        return spawnTile;
        //return this._tiles.FirstOrDefault(t => t.Value.TileType == 1).Value;
    }
}
