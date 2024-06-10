using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Player player;
    public Tile[,] grid;
    public int gridWidth;
    public int gridHeight;
    public GameObject tilePrefab;
    public float tileSpacing = 1.1f;  // Add a spacing factor for tiles

    private List<Tile> selectedTiles = new List<Tile>();
    private HashSet<Tile.TileType> selectedTileTypes = new HashSet<Tile.TileType>();
    private Vector2Int lastDirection;
    private bool isInitialDirectionSet = false;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeGrid();
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Configure the LineRenderer
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;

        // Set the sorting layer and order
        lineRenderer.sortingLayerName = "Foreground"; // Ensure this layer is above the tiles
        lineRenderer.sortingOrder = 1; // Ensure the order is higher than the tiles

        // Adjust the Z position of the LineRenderer
        lineRenderer.transform.position = new Vector3(lineRenderer.transform.position.x, lineRenderer.transform.position.y, -1);
    }

    private void InitializeGrid()
    {
        grid = new Tile[gridWidth, gridHeight];
        Vector2Int playerStartPos = player.gridPosition;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Skip the player's initial position
                if (x == playerStartPos.x && y == playerStartPos.y)
                {
                    continue;
                }

                Vector3 position = new Vector3(x * tileSpacing, y * tileSpacing, 0);
                GameObject tileObject = Instantiate(tilePrefab, position, Quaternion.identity);
                Tile tile = tileObject.GetComponent<Tile>();
                tile.gridPosition = new Vector2Int(x, y);
                tile.type = GetRandomTileType(); // Assign a random tile type
                tileObject.GetComponent<SpriteRenderer>().sortingLayerName = "Grid";  // Set sorting layer
                grid[x, y] = tile;

                // Set the sprite color based on the tile type (this is just for demonstration)
                tileObject.GetComponent<SpriteRenderer>().color = tile.originalColor;
            }
        }
    }

    private Tile.TileType GetRandomTileType()
    {
        // Randomly select a tile type (excluding Empty)
        Tile.TileType[] types = { Tile.TileType.Treasure, Tile.TileType.Weapon, Tile.TileType.Potion, Tile.TileType.Shield };
        return types[Random.Range(0, types.Length)];
    }

    public void SelectTile(Tile tile)
    {
        if (selectedTiles.Count == 0)
        {
            if (AreTilesAdjacent(player.gridPosition, tile.gridPosition))
            {
                tile.ToggleSelection(true);
                selectedTiles.Add(tile);
                selectedTileTypes.Add(tile.type);
                lastDirection = tile.gridPosition - player.gridPosition;
                isInitialDirectionSet = true;
                UpdateLineRenderer();
                UpdateTileColors();
            }
        }
        else
        {
            Tile lastSelectedTile = selectedTiles[selectedTiles.Count - 1];
            Vector2Int currentDirection = tile.gridPosition - lastSelectedTile.gridPosition;
            if (AreTilesAdjacent(lastSelectedTile.gridPosition, tile.gridPosition) && !tile.IsSelected())
            {
                if (isInitialDirectionSet)
                {
                    if (IsValidMove(lastDirection, currentDirection))
                    {
                        tile.ToggleSelection(true);
                        selectedTiles.Add(tile);
                        selectedTileTypes.Add(tile.type);
                        lastDirection = currentDirection;
                        UpdateLineRenderer();
                        UpdateTileColors();
                    }
                }
                else
                {
                    tile.ToggleSelection(true);
                    selectedTiles.Add(tile);
                    selectedTileTypes.Add(tile.type);
                    lastDirection = currentDirection;
                    isInitialDirectionSet = true;
                    UpdateLineRenderer();
                    UpdateTileColors();
                }
            }
        }
    }

    private void DeselectTile()
    {
        if (selectedTiles.Count > 1)
        {
            Tile lastSelectedTile = selectedTiles[selectedTiles.Count - 1];
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Tile previousTile = selectedTiles[selectedTiles.Count - 2];

            if (Vector2.Distance(mousePosition, previousTile.transform.position) < Vector2.Distance(mousePosition, lastSelectedTile.transform.position))
            {
                lastSelectedTile.ToggleSelection(false);
                selectedTiles.RemoveAt(selectedTiles.Count - 1);
                UpdateTileColors();
                UpdateLineRenderer();
            }
        }
    }

    private bool AreTilesAdjacent(Vector2Int pos1, Vector2Int pos2)
    {
        int dx = Mathf.Abs(pos1.x - pos2.x);
        int dy = Mathf.Abs(pos1.y - pos2.y);
        return (dx <= 1 && dy <= 1) && !(dx == 0 && dy == 0);
    }

    private bool IsValidMove(Vector2Int lastDir, Vector2Int currentDir)
    {
        // Normalize the direction vectors
        Vector2Int normalizedLastDir = new Vector2Int(lastDir.x != 0 ? lastDir.x / Mathf.Abs(lastDir.x) : 0, lastDir.y != 0 ? lastDir.y / Mathf.Abs(lastDir.y) : 0);
        Vector2Int normalizedCurrentDir = new Vector2Int(currentDir.x != 0 ? currentDir.x / Mathf.Abs(currentDir.x) : 0, currentDir.y != 0 ? currentDir.y / Mathf.Abs(currentDir.y) : 0);

        // Allow move if it is continuing in the same direction or transitioning from straight to diagonal or vice versa
        return (normalizedLastDir == normalizedCurrentDir) ||
               (Mathf.Abs(normalizedCurrentDir.x) == 1 && Mathf.Abs(normalizedCurrentDir.y) == 1) ||
               (normalizedCurrentDir.x == 0 || normalizedCurrentDir.y == 0);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            DeselectTile();
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (selectedTiles.Count >= 3)
            {
                foreach (Tile tile in selectedTiles)
                {
                    tile.ToggleSelection(false);
                    DestroyTile(tile);
                }
                player.MoveAlongPath(selectedTiles);
            }
            else
            {
                foreach (Tile tile in selectedTiles)
                {
                    tile.ToggleSelection(false);
                }
            }
            selectedTiles.Clear();
            selectedTileTypes.Clear();
            isInitialDirectionSet = false;
            lineRenderer.positionCount = 0;  // Clear the line
            ResetTileColors();
        }
    }

    private void DestroyTile(Tile tile)
    {
        Vector2Int position = tile.gridPosition;
        Destroy(tile.gameObject);
        grid[position.x, position.y] = null;
    }

    public Vector3 GetTileWorldPosition(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x * tileSpacing, gridPosition.y * tileSpacing, 0);
    }

    private void UpdateLineRenderer()
    {
        lineRenderer.positionCount = selectedTiles.Count;
        for (int i = 0; i < selectedTiles.Count; i++)
        {
            lineRenderer.SetPosition(i, selectedTiles[i].transform.position);
        }
    }

    private void UpdateTileColors()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Tile tile = grid[x, y];
                if (tile != null && !tile.IsSelected())
                {
                    tile.Darken(!selectedTileTypes.Contains(tile.type));
                }
            }
        }
    }

    private void ResetTileColors()
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
}
