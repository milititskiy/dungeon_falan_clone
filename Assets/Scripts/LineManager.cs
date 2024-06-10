using UnityEngine;
using System.Collections.Generic;

public class LineManager : MonoBehaviour
{
    private LineRenderer lineRenderer;

    private void Start()
    {
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

    public void UpdateLineRenderer(List<Tile> selectedTiles)
    {
        lineRenderer.positionCount = selectedTiles.Count + 1; // +1 to include the player's position
        lineRenderer.SetPosition(0, GameManager.Instance.player.transform.position); // Start from the player's position

        for (int i = 0; i < selectedTiles.Count; i++)
        {
            lineRenderer.SetPosition(i + 1, selectedTiles[i].transform.position);
        }
    }

    public void ClearLine()
    {
        lineRenderer.positionCount = 0;  // Clear the line
    }
}
