using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance;
    private Dictionary<Vector2, HexagonTile> _tiles;
    [SerializeField] private HexagonTile hexPrefab;
    [SerializeField] private HexagonTile TestPrefab;
    [SerializeField] private Transform _cam;
    public int width;
    public int height;

    float xOffset = 0.95f;
    float yOffset = 0.8f;

    void Awake()
    {
        Instance = this;
    }

    private System.Random random = new System.Random();

    // Custom random function that returns true with a given probability
    private bool CustomRandom(float probability)
    {
        return random.NextDouble() < probability;
    }

    public void generateBoard()
    {
        _tiles = new Dictionary<Vector2, HexagonTile>();
        int totalTiles = width * height;
        int tilesToDestroy = (int)(totalTiles * 0.2); // 20% ของ board ที่จะถูกทำลาย
        int countCell = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //bool isCreate = CustomRandom(0.8f) ;
                int isDestroy;
                if(countCell < 40)
                {
                    isDestroy = Random.Range(0, 5);
                }
                else
                {
                    isDestroy = 1;
                }

                if (isDestroy != 0)
                {
                    float xPos = x * xOffset;
                    if (y % 2 == 0)
                    {
                        xPos += xOffset / 2f;
                    }

                    var hex_go = Instantiate(hexPrefab, new Vector3(xPos, y * yOffset, (float)y / 10), Quaternion.identity);

                    hex_go.name = "Hex_" + x + "_" + y;
                    var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                    hex_go.Init(isOffset);

                    _tiles[new Vector2(xPos, y * yOffset)] = hex_go;

                    hex_go.transform.SetParent(this.transform);
                }
                else
                {
                    countCell++;
                    float xPos = x * xOffset;
                    if (y % 2 == 0)
                    {
                        xPos += xOffset / 2f;
                    }

                    var hex_go = Instantiate(TestPrefab, new Vector3(xPos, y * yOffset, (float)y / 10), Quaternion.identity);

                    hex_go.name = "Hex_" + x + "_" + y;
                    var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                    hex_go.Init(isOffset);

                    _tiles[new Vector2(xPos, y * yOffset)] = hex_go;

                    hex_go.transform.SetParent(this.transform);
                }

            }
        }
        Debug.Log(countCell);
        if(countCell < 40)
        {
            Debug.Log("wow");
            foreach (Transform cell in this.transform)
            {
                GameObject.Destroy(cell.gameObject);
            }
            generateBoard();
        }
        else
        {
            _cam.transform.position = new Vector3((float)width / 2.05f - 0.5f, (float)height / 2.5f - 0.5f, -10);
            GameManager.Instance.ChangeState(GameState.SpawnPlayer);
            GameManager.Instance.ChangeState(GameState.PlayerTurn);
        }
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


