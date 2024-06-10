using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum TileType { Empty, Treasure, Weapon, Potion, Shield, Enemy }
    public TileType type;
    public Vector2Int gridPosition;

    private bool isSelected = false;
    private Material originalMaterial;
    public Material glowMaterial; // Assign this in the Inspector

    private void Awake()
    {
        // Adjust the BoxCollider2D size
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.size = new Vector2(0.8f, 0.8f); // Adjust the size as needed
        }

        // Store the original material
        originalMaterial = GetComponent<SpriteRenderer>().material;
    }

    private void OnMouseDown()
    {
        GameManager.Instance.SelectTile(this);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButton(0))
        {
            GameManager.Instance.SelectTile(this);
        }

        // Apply glow material
        GetComponent<SpriteRenderer>().material = glowMaterial;
    }

    private void OnMouseExit()
    {
        // Revert to the original material
        GetComponent<SpriteRenderer>().material = originalMaterial;
    }

    public void ToggleSelection(bool select)
    {
        isSelected = select;
        // Change color based on selection if desired
    }

    public void Darken(bool darken)
    {
        // Darken or reset color if desired
    }

    public bool IsSelected()
    {
        return isSelected;
    }
}
