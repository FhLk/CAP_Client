using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
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
    [SerializeField] private Transform _cam;
    public int width;
    public int height;

    float xOffset = 0.95f;
    float yOffset = 0.8f;
    private HexagonTile hex_go;

    // Start is called before the first frame update

    void Awake()
    {
        Instance = this;
    }

    public void generateBoard()
    {
        _tiles = new Dictionary<Vector2, HexagonTile>();
        int countEvent = 0;
        bool haveStore = false;
        bool haveStart = false;
        bool haveFinal = false;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int isDestroy = Random.Range(0, 5);
                int isEvent = Random.Range(0, 10);
                int isStore = Random.Range(0, 100);
                int isStart = Random.Range(0, 60);
                int isFinal = Random.Range(0, 60);

                if (isDestroy != 0)
                {
                    float xPos = x * xOffset;
                    if (y % 2 == 0)
                    {
                        xPos += xOffset / 2f;
                    }
                    if (isEvent != 0)
                    {
                        if(isStore == 50 && !haveStore)
                        {
                            hex_go = Instantiate(storePrefab, new Vector3(xPos, y * yOffset, (float)y / 10), Quaternion.identity);
                            hex_go.name = "Store_" + x + "_" + y;
                            haveStore = true;
                        }
                        else if(isStart == 50 && !haveStart)
                        {
                            hex_go = Instantiate(startPrefab, new Vector3(xPos, y * yOffset, (float)y / 10), Quaternion.identity);
                            hex_go.name = "Start_" + x + "_" + y;
                            haveStart = true;
                        }
                        else if (isFinal == 50 && !haveFinal)
                        {
                            hex_go = Instantiate(startPrefab, new Vector3(xPos, y * yOffset, (float)y / 10), Quaternion.identity);
                            hex_go.name = "Final_" + x + "_" + y;
                            haveFinal = true;
                        }
                        else
                        {
                            hex_go = Instantiate(hexPrefab, new Vector3(xPos, y * yOffset, (float)y / 10), Quaternion.identity);
                            hex_go.name = "Hex_" + x + "_" + y;
                        }
                    }
                    else
                    {
                        if ((x > 1 && x < width - 1) && (y > 1 && y < height - 1))
                        {
                            countEvent++;
                            if (countEvent <= 6)
                            {
                                hex_go = Instantiate(eventPrefab, new Vector3(xPos, y * yOffset, (float)y / 10), Quaternion.identity);
                                hex_go.name = "Event_" + x + "_" + y;
                            }
                        }
                        else
                        {
                            if (isStore == 50 && !haveStore)
                            {
                                hex_go = Instantiate(storePrefab, new Vector3(xPos, y * yOffset, (float)y / 10), Quaternion.identity);
                                hex_go.name = "Store_" + x + "_" + y;
                                haveStore = true;
                            }
                            else if (isStart == 50 && !haveStart)
                            {
                                hex_go = Instantiate(startPrefab, new Vector3(xPos, y * yOffset, (float)y / 10), Quaternion.identity);
                                hex_go.name = "Start_" + x + "_" + y;
                                haveStart = true;
                            }
                            else if (isFinal == 50 && !haveFinal)
                            {
                                hex_go = Instantiate(startPrefab, new Vector3(xPos, y * yOffset, (float)y / 10), Quaternion.identity);
                                hex_go.name = "Final_" + x + "_" + y;
                                haveFinal = true;
                            }
                            else
                            {
                                hex_go = Instantiate(hexPrefab, new Vector3(xPos, y * yOffset, (float)y / 10), Quaternion.identity);
                                hex_go.name = "Hex_" + x + "_" + y;
                            }
                        }

                    }

                    //var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                    //hex_go.Init(isOffset);
                    _tiles[new Vector2(xPos, y * yOffset)] = hex_go;
                    hex_go.transform.SetParent(this.transform);
                }
            }
        }
        _cam.transform.position = new Vector3((float)width / 2.05f - 0.5f, (float)height / 2.5f - 0.5f, -10);
        //GameManager.Instance.ChangeState(GameState.SpawnPlayer);
        //GameManager.Instance.ChangeState(GameState.PlayerTurn);
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
