using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board_Cell : MonoBehaviour
{
    public static Board_Cell Instance;
    private Dictionary<Vector2, HexagonTile> _tiles;
    [SerializeField] private HexagonTile hexPrefab;
    [SerializeField] private HexagonTile eventPrefab;
    [SerializeField] private HexagonTile startPrefab;
    [SerializeField] private HexagonTile finalPrefab;
    [SerializeField] private HexagonTile storePrefab;
    [SerializeField] private HexagonTile ladderPrefab;
    [SerializeField] private HexagonTile TestPrefab;
    [SerializeField] private Transform _cam;
    private List<HexagonTile> listEvent = new List<HexagonTile>();
    private List<HexagonTile> listHex = new List<HexagonTile>();
    private List<HexagonTile> listStart_Final = new List<HexagonTile>();

    public int width;
    public int height;

    float xOffset = 0.95f;
    float yOffset = 0.8f;

    void Awake()
    {
        Instance = this;
    }

    public void generateBoard()
    {
        _tiles = new Dictionary<Vector2, HexagonTile>();
        HexagonTile[,] initBoard = new HexagonTile[width, height];
        int totalTiles = width * height;
        int tileToDestroy = (int)(totalTiles * 0.25);
        bool isCondition = false;
        int countCell = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xPos = x * xOffset;
                if (y % 2 == 0)
                {
                    xPos += xOffset / 2f;
                }
                initBoard[x, y] = CreateTile(x, y, xPos, yOffset, "Hex_", hexPrefab, ref _tiles);
            }
        }
        //validateBoard(isCondition);
        setNeighbors(initBoard);
        _cam.transform.position = new Vector3((float)width / 2.05f - 0.5f, (float)height / 2.5f - 0.5f, -10);
        GameManager.Instance.ChangeState(GameState.SpawnPlayer);
        GameManager.Instance.ChangeState(GameState.PlayerTurn);
     
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

    private void validateBoard(bool isCondition)
    {
        if (!isCondition)
        {
            foreach (Transform cell in this.transform)
            {
                GameObject.Destroy(cell.gameObject);
            }
            generateBoard();
        }
    }

    private HexagonTile CreateTile(int x, int y, float xPos, float yOffset, string namePrefix, HexagonTile prefab, ref Dictionary<Vector2, HexagonTile> tiles)
    {
        HexagonTile hex_go = Instantiate(prefab, new Vector3(xPos, y * yOffset, (float)y / 10), Quaternion.identity);
        //hex_go.GetComponent;
        hex_go.x = x;
        hex_go.y = y;
        string typeTiled = hex_go.GetComponent<HexagonTile>().TileName;
        if (typeTiled == "Event")
        {
            listEvent.Add(hex_go);
        }
        else if (typeTiled == "Start" || typeTiled == "Final")
        {
            listStart_Final.Add(hex_go);
        }
        else
        {
            listHex.Add(hex_go);
        }
        hex_go.name = namePrefix + x + "_" + y;
        tiles[new Vector2(xPos, y * yOffset)] = hex_go.GetComponent<HexagonTile>();
        hex_go.transform.SetParent(this.transform);
        return hex_go;
    }

    public HexagonTile GetPlayerSpawnTile()
    {
        return _tiles.Where(t => t.Value.TileName == "Start").First().Value;
    }

    public HexagonTile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }
}
