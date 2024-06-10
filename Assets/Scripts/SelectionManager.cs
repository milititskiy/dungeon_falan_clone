using UnityEngine;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour
{
    private List<Tile> selectedTiles = new List<Tile>();
    private Tile.TileType selectedTileType;
    private Vector2Int lastDirection;
    private bool isInitialDirectionSet = false;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            DeselectTile();
        }
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
            if (GameManager.Instance.gridManager.AreTilesAdjacent(lastSelectedTile.gridPosition, tile.gridPosition) && !tile.IsSelected() && tile.type == selectedTileType)
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
