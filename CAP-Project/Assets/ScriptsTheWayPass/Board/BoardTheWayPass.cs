using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardTheWayPass : MonoBehaviour
{
    public static BoardTheWayPass Instance;
    public Dictionary<string, Tile> _tiles;
    [SerializeField] private Tile hexPrefab;
    [SerializeField] private Transform _cam;
    [SerializeField] private int numLadders;
    [SerializeField] private int percentageCellDestroys;

    public int width;
    public int height;
    public Tile[,] board;
    public List<Item> bombs;

    float xOffset = 1f;
    float yOffset = 0.8f;

    void Awake()
    {
        Instance = this;
    }

    public void generateBoard()
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
        stateDefind();
        //GameMangerTheWayPass.Instance.ChangeState(GameStateTheWayPass.SpawnPlayer);
        //GameMangerTheWayPass.Instance.ChangeState(GameStateTheWayPass.PlayerTurn);
        GameMangerTheWayPass.Instance.ChangeState(GameStateTheWayPass.ReqToServer);
    }
    public void stateDefind()
    {
        //defindeStartCell();
        //destroyCell(percentageCellDestroys);
        //defindeCell(numLadders, 3);
    }

    public void defindeStartCell()
    {
        Tile startCell = board[0, 0];
        Tile finalCell = board[this.width - 1, this.height - 1];
        startCell.setPrefab(2);
        finalCell.setPrefab(2);
    }

    public void destroyCell(int percentage)
    {
        int full = this.width * this.height;
        int num_tileToDestroy = (full / 100) * percentage;
        List<Tile> tilesToDestroy = new List<Tile>();
        List<Tile> listTile = new List<Tile>();
        List<Tile> neighbors = new List<Tile>();
        foreach (Tile hex in board)
        {
            if (hex.TileType == 0)
            {
                listTile.Add(hex);
            }
        }
        for (int i = 0; i < num_tileToDestroy; i++)
        {
            //Get from Backend later
            Tile hex = listTile[Random.Range(0, listTile.Count)];
            listTile.Remove(hex);
            if (!tilesToDestroy.Contains(hex))
            {
                tilesToDestroy.Add(hex);
                /*
                neighbors = hex.neighbors;//00
                for (int j = tilesToDestroy.Count - 1; j >= 0; j--)
                {
                    //00 11 
                    Tile cell = tilesToDestroy[j];
                    if (neighbors.Contains(cell))
                    {
                        tilesToDestroy.RemoveAt(j);
                    }
                }
                */
            }
        }
        foreach (Tile tile in tilesToDestroy)
        {
            Destroy(tile.gameObject);
            board[tile.x, tile.y] = null;
        };
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

    private void defindeCell(int count, int type)
    {
        List<Tile> tilesToItem = new List<Tile>();
        List<Tile> listTile = new List<Tile>();
        foreach (Tile hex in board)
        {
            if (hex != null && hex.TileType == 0)
            {
                listTile.Add(hex);
            }
        }
        for (int i = 0; i < count; i++)
        {
            //Get from Backend later
            Tile hex = listTile[Random.Range(0, listTile.Count)];
            listTile.Remove(hex);
            if (!tilesToItem.Contains(hex))
            {
                tilesToItem.Add(hex);
            }
        }
        foreach (Tile tile in tilesToItem)
        {
            Destroy(tile.gameObject);
            board[tile.x, tile.y] = CreateTile(tile.x, tile.y, tile.xPos, tile.yOffset, board[tile.x, tile.y].setName(type), type);
            updateNeighbor(board[tile.x, tile.y], board);
        };
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

    private void updateNeighbor(Tile cell, Tile[,] board)
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

    private Tile CreateTile(int x, int y, float xPos, float yOffset, string namePrefix, int type)
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
        return hex_go;
    }

    public Tile GetTile()
    {
        return board[0, 0];
    }

    public Tile GetTileJoin()
    {
        return board[0, 1];
    }
}


