using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Rendering;

public class Board_Cell : MonoBehaviour
{
    public static Board_Cell Instance;
    private Dictionary<Vector2, HexagonTile> _tiles;
    [SerializeField] private GameObject BOARD;
    [SerializeField] private HexagonTile hexPrefab;
    [SerializeField] private HexagonTile eventPrefab;
    [SerializeField] private HexagonTile startPrefab;
    [SerializeField] private HexagonTile finalPrefab;
    [SerializeField] private HexagonTile storePrefab;
    [SerializeField] private HexagonTile ladderPrefab;
    [SerializeField] private Transform _cam;
    private List<HexagonTile> listEvent = new List<HexagonTile>();
    private List<HexagonTile> listHex = new List<HexagonTile>();
    private List<HexagonTile> listStart_Final = new List<HexagonTile>();

    public int width;
    public int height;

    float xOffset = 0.95f;
    float yOffset = 0.8f;

    // Start is called before the first frame update

    void Awake()
    {
        Instance = this;
    }

    public void generateBoard()
    {
        _tiles = new Dictionary<Vector2, HexagonTile>();
        int countEvent = 0;
        int countLadder = 0;
        bool haveStore = false;
        bool haveStart = false;
        bool haveFinal = false;
        bool isCondition = false;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (countLadder == 6  && countEvent == 6 && haveFinal && haveStart && haveStore)
                {
                    isCondition = true;
                }
                int isDestroy = Random.Range(0, 5);

                if (isDestroy != 0)
                {
                    int isEvent = Random.Range(0, 5);
                    int isStore = Random.Range(0, 10);
                    int isStart = Random.Range(0, 10);
                    int isFinal = Random.Range(0, 10);
                    int isLadder = Random.Range(0, 5);

                    float xPos = x * xOffset;
                    if (y % 2 == 0)
                    {
                        xPos += xOffset / 2f;
                    }
                    if (isEvent != 0)
                    {
                        CreateTile(x, y, xPos, yOffset, "Hex_", hexPrefab, ref _tiles);
                    }
                    else
                    {
                        if ((x > 1 && x < width - 1) && (y > 1 && y < height - 1))
                        {
                            if (countEvent < 6)
                            {
                                countEvent++;
                                CreateTile(x, y, xPos, yOffset, "Event_", eventPrefab, ref _tiles);
                            }
                            else if (countLadder < 6 && isLadder == 0)
                            {
                                countLadder++;
                                CreateTile(x, y, xPos, yOffset, "Ladder_", ladderPrefab, ref _tiles);
                            }
                        }
                        else
                        {
                            if (isStore == 5 && !haveStore)
                            {
                                CreateTile(x, y, xPos, yOffset, "Store_", storePrefab, ref _tiles);
                                haveStore = true;
                            }
                            else if (isStart == 5 && !haveStart)
                            {
                                CreateTile(x, y, xPos, yOffset, "Start_", startPrefab, ref _tiles);
                                haveStart = true;
                            }
                            else if (isFinal == 5 && !haveFinal)
                            {
                                CreateTile(x, y, xPos, yOffset, "Final_", startPrefab, ref _tiles);
                                haveFinal = true;
                            }
                            else
                            {
                                CreateTile(x, y, xPos, yOffset, "Hex_", hexPrefab, ref _tiles);
                            }
                        }

                    }
                }
            }
        }
        validateBoard(isCondition);
        _cam.transform.position = new Vector3((float)width / 2.05f - 0.5f, (float)height / 2.5f - 0.5f, -10);
    }

    private void validateBoard(bool isCondition)
    {
        if (!isCondition)
        {
            Debug.Log("wow");
            foreach (Transform cell in BOARD.transform)
            {
                GameObject.Destroy(cell.gameObject);
            }
            generateBoard();
        }
    }

    private void CreateTile(int x, int y, float xPos, float yOffset, string namePrefix, HexagonTile prefab, ref Dictionary<Vector2, HexagonTile> tiles)
    {
        HexagonTile hex_go = Instantiate(prefab, new Vector3(xPos, y * yOffset, (float)y / 10), Quaternion.identity);
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
    }

    public HexagonTile GetPlayerSpawnTile()
    {
        return _tiles.Where(t => t.Key.x < width && t.Value.Walkable).OrderBy(t => Random.value).First().Value;
    }

    public HexagonTile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }
}
