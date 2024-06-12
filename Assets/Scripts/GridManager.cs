using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Tile[,] grid;
    public int gridWidth;
    public int gridHeight;
    public GameObject treasurePrefab;
    public GameObject weaponPrefab;
    public GameObject potionPrefab;
    public GameObject shieldPrefab;
    public GameObject enemyPrefab;
    public float tileSpacing = 1.1f; // Spacing factor for tiles

    private void Start()
    {
        // Initialize the grid based on the current state
        if (GameManager.Instance.inBattle)
        {
            InitializeGrid();
        }
        else
        {
            ClearGrid();
            PlacePlayer(GameManager.Instance.player);
        }
    }

    public void InitializeGrid()
    {
        grid = new Tile[gridWidth, gridHeight];
        Vector2Int playerStartPos = GameManager.Instance.player.gridPosition;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (x == playerStartPos.x && y == playerStartPos.y)
                {
                    continue;
                }

                Vector3 position = new Vector3(x * tileSpacing, y * tileSpacing, 0);
                GameObject tileObject = InstantiateTilePrefab(position);
                Tile tile = tileObject.GetComponent<Tile>();
                tile.gridPosition = new Vector2Int(x, y);
                tileObject.GetComponent<SpriteRenderer>().sortingLayerName = "Grid";  // Set sorting layer
                grid[x, y] = tile;
            }
        }
    }

    private GameObject InstantiateTilePrefab(Vector3 position)
    {
        Tile.TileType type = GetRandomTileType();
        GameObject prefab;

        switch (type)
        {
            case Tile.TileType.Treasure:
                prefab = treasurePrefab;
                break;
            case Tile.TileType.Weapon:
                prefab = weaponPrefab;
                break;
            case Tile.TileType.Potion:
                prefab = potionPrefab;
                break;
            case Tile.TileType.Shield:
                prefab = shieldPrefab;
                break;
            case Tile.TileType.Enemy:
                prefab = enemyPrefab;
                break;
            default:
                prefab = treasurePrefab;
                break;
        }

        GameObject tileObject = Instantiate(prefab, position, Quaternion.identity);
        Tile tile = tileObject.GetComponent<Tile>();
        tile.type = type;
        return tileObject;
    }

    private Tile.TileType GetRandomTileType()
    {
        // Randomly select a tile type (excluding Empty)
        Tile.TileType[] types = { Tile.TileType.Treasure, Tile.TileType.Weapon, Tile.TileType.Potion, Tile.TileType.Shield, Tile.TileType.Enemy };
        return types[Random.Range(0, types.Length)];
    }

    public Vector3 GetTileWorldPosition(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x * tileSpacing, gridPosition.y * tileSpacing, 0);
    }

    public bool AreTilesAdjacent(Vector2Int pos1, Vector2Int pos2)
    {
        int dx = Mathf.Abs(pos1.x - pos2.x);
        int dy = Mathf.Abs(pos1.y - pos2.y);
        return (dx <= 1 && dy <= 1) && !(dx == 0 && dy == 0);
    }

    public bool IsValidMove(Vector2Int lastDir, Vector2Int currentDir)
    {
        // Normalize the direction vectors
        Vector2Int normalizedLastDir = new Vector2Int(lastDir.x != 0 ? lastDir.x / Mathf.Abs(lastDir.x) : 0, lastDir.y != 0 ? lastDir.y / Mathf.Abs(lastDir.y) : 0);
        Vector2Int normalizedCurrentDir = new Vector2Int(currentDir.x != 0 ? currentDir.x / Mathf.Abs(currentDir.x) : 0, currentDir.y != 0 ? currentDir.y / Mathf.Abs(currentDir.y) : 0);

        // Allow move if it is continuing in the same direction or transitioning from straight to diagonal or vice versa
        return (normalizedLastDir == normalizedCurrentDir) ||
               (Mathf.Abs(normalizedCurrentDir.x) == 1 && Mathf.Abs(normalizedCurrentDir.y) == 1) ||
               (normalizedCurrentDir.x == 0 || normalizedCurrentDir.y == 0);
    }

    public void DestroyTile(Tile tile)
    {
        Vector2Int position = tile.gridPosition;
        Destroy(tile.gameObject);
        grid[position.x, position.y] = null;
    }

    public void UpdateTileColors(Tile.TileType selectedTileType, List<Tile> selectedTiles)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Tile tile = grid[x, y];
                if (tile != null && !tile.IsSelected())
                {
                    tile.Darken(tile.type != selectedTileType);
                }
            }
        }
    }

    public void ResetTileColors()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Tile tile = grid[x, y];
                if (tile != null)
                {
                    tile.Darken(false);
                }
            }
        }
    }

    public void ClearGrid()
    {
        if (grid == null)
        {
            grid = new Tile[gridWidth, gridHeight]; // Ensure the grid is initialized
        }

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y] != null)
                {
                    Destroy(grid[x, y].gameObject);
                    grid[x, y] = null;
                }
            }
        }
    }

    public void PlacePlayer(Player player)
    {
        player.transform.position = new Vector3(0, 0, 0); // Adjust the starting position as needed
        player.gridPosition = new Vector2Int(0, 0); // Adjust the starting grid position as needed
    }
}
