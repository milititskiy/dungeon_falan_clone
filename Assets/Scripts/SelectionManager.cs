using UnityEngine;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour
{
    private List<Tile> selectedTiles = new List<Tile>();
    private Tile.TileType selectedTileType;
    private Vector2Int lastDirection;
    private bool isInitialDirectionSet = false;

    private Dictionary<Tile.TileType, List<Tile.TileType>> validMatchCombinations;

    private void Awake()
    {
        InitializeValidMatchCombinations();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            DeselectTile();
        }
    }

    private void InitializeValidMatchCombinations()
    {
        validMatchCombinations = new Dictionary<Tile.TileType, List<Tile.TileType>>();

        // Define valid match combinations
        validMatchCombinations.Add(Tile.TileType.Weapon, new List<Tile.TileType> { Tile.TileType.Enemy, Tile.TileType.Weapon });
        validMatchCombinations.Add(Tile.TileType.Enemy, new List<Tile.TileType> { Tile.TileType.Weapon, Tile.TileType.Enemy });

        // Add other combinations here as needed
        validMatchCombinations.Add(Tile.TileType.Treasure, new List<Tile.TileType> { Tile.TileType.Treasure });
        validMatchCombinations.Add(Tile.TileType.Potion, new List<Tile.TileType> { Tile.TileType.Potion });
        validMatchCombinations.Add(Tile.TileType.Shield, new List<Tile.TileType> { Tile.TileType.Shield });
    }

    public void ProcessSelection()
    {
        if (selectedTiles.Count >= 3)
        {
            foreach (Tile tile in selectedTiles)
            {
                tile.ToggleSelection(false);
                GameManager.Instance.gridManager.DestroyTile(tile);
            }
            GameManager.Instance.player.MoveAlongPath(selectedTiles);
            GameManager.Instance.scoreManager.UpdateScore(selectedTiles.Count); // Update score based on the number of selected tiles
        }
        else
        {
            foreach (Tile tile in selectedTiles)
            {
                tile.ToggleSelection(false);
            }
        }
        selectedTiles.Clear();
        isInitialDirectionSet = false;
        GameManager.Instance.lineManager.ClearLine();
        GameManager.Instance.gridManager.ResetTileColors();
    }

    public void SelectTile(Tile tile)
    {
        Debug.Log("SelectTile called");
        if (selectedTiles.Count == 0)
        {
            Debug.Log("First tile selected");
            if (GameManager.Instance.gridManager.AreTilesAdjacent(GameManager.Instance.player.gridPosition, tile.gridPosition))
            {
                Debug.Log("Tile is adjacent to player");
                tile.ToggleSelection(true);
                selectedTiles.Add(tile);
                selectedTileType = tile.type;
                lastDirection = tile.gridPosition - GameManager.Instance.player.gridPosition;
                isInitialDirectionSet = true;
                GameManager.Instance.lineManager.UpdateLineRenderer(selectedTiles);
                GameManager.Instance.gridManager.UpdateTileColors(selectedTileType, selectedTiles);
            }
        }
        else
        {
            Tile lastSelectedTile = selectedTiles[selectedTiles.Count - 1];
            Vector2Int currentDirection = tile.gridPosition - lastSelectedTile.gridPosition;
            Debug.Log($"Current direction: {currentDirection}, Last direction: {lastDirection}");
            if (GameManager.Instance.gridManager.AreTilesAdjacent(lastSelectedTile.gridPosition, tile.gridPosition) && !tile.IsSelected() && IsValidTileType(tile.type))
            {
                Debug.Log("Tile is adjacent and matches type");
                if (isInitialDirectionSet)
                {
                    if (GameManager.Instance.gridManager.IsValidMove(lastDirection, currentDirection))
                    {
                        Debug.Log("Move is valid");
                        tile.ToggleSelection(true);
                        selectedTiles.Add(tile);
                        lastDirection = currentDirection;
                        GameManager.Instance.lineManager.UpdateLineRenderer(selectedTiles);
                        GameManager.Instance.gridManager.UpdateTileColors(selectedTileType, selectedTiles);
                    }
                }
                else
                {
                    tile.ToggleSelection(true);
                    selectedTiles.Add(tile);
                    lastDirection = currentDirection;
                    isInitialDirectionSet = true;
                    GameManager.Instance.lineManager.UpdateLineRenderer(selectedTiles);
                    GameManager.Instance.gridManager.UpdateTileColors(selectedTileType, selectedTiles);
                }
            }
        }
    }

    private bool IsValidTileType(Tile.TileType tileType)
    {
        if (validMatchCombinations.ContainsKey(selectedTileType))
        {
            return validMatchCombinations[selectedTileType].Contains(tileType);
        }

        // Allow tiles of the same type to be selected together if not defined in the dictionary
        return selectedTileType == tileType;
    }

    public void DeselectTile()
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
                GameManager.Instance.gridManager.UpdateTileColors(selectedTileType, selectedTiles);
                GameManager.Instance.lineManager.UpdateLineRenderer(selectedTiles);
            }
        }
    }
}
