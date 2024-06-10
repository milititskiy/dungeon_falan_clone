using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public Vector2Int gridPosition;
    public float moveSpeed = 5f;
    public float driftAmount = 0.2f; // Adjust this value for the amount of drift

    private void Start()
    {
        // Set the initial position of the player on the grid
        transform.position = GameManager.Instance.gridManager.GetTileWorldPosition(gridPosition);
    }

    public void MoveAlongPath(List<Tile> path)
    {
        // Create a copy of the path list to avoid modification issues
        List<Tile> pathCopy = new List<Tile>(path);
        StartCoroutine(FollowPath(pathCopy));
    }

    private IEnumerator FollowPath(List<Tile> path)
    {
        Vector2Int? lastDirection = null;

        foreach (Tile tile in path)
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = GameManager.Instance.gridManager.GetTileWorldPosition(tile.gridPosition);
            Vector2Int currentDirection = tile.gridPosition - gridPosition;

            // Apply drift if changing direction
            if (lastDirection.HasValue && currentDirection != lastDirection.Value)
            {
                Vector3 drift = new Vector3(-currentDirection.y, currentDirection.x, 0) * driftAmount;
                startPosition += drift;
            }

            float journeyLength = Vector3.Distance(startPosition, targetPosition);
            float startTime = Time.time;

            while ((transform.position - targetPosition).sqrMagnitude > 0.01f)
            {
                float distCovered = (Time.time - startTime) * moveSpeed;
                float fracJourney = distCovered / journeyLength;
                float easedFracJourney = EaseInOutQuad(fracJourney);

                transform.position = Vector3.Lerp(startPosition, targetPosition, easedFracJourney);
                yield return null;
            }
            transform.position = targetPosition;
            gridPosition = tile.gridPosition;
            lastDirection = currentDirection;
        }
    }

    // Ease In and Ease Out function
    private float EaseInOutQuad(float t)
    {
        return t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
    }
}
