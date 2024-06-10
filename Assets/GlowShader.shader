using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum TileType { Empty, Treasure, Weapon, Potion, Shield }
    public TileType type;
    public Vector2Int gridPosition;

    private bool isSelected = false;
    private Color originalColor; // Store the original color of the tile
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

    private void Start()
    {
        // Set the original color based on the tile type
        switch (type)
        {
            case TileType.Treasure:
                originalColor = Color.yellow;
                break;
            case TileType.Weapon:
                originalColor = Color.red;
                break;
            case TileType.Potion:
                originalColor = Color.blue;
                break;
            case TileType.Shield:
                originalColor = Color.green;
                break;
            default:
                originalColor = Color.white;
                break;
        }
        GetComponent<SpriteRenderer>().color = originalColor;
    }

    private void OnMouseDown()
    {
        GameManager.Instance.selectionManager.SelectTile(this);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButton(0))
        {
            GameManager.Instance.selectionManager.SelectTile(this);
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
        GetComponent<SpriteRenderer>().color = select ? new Color(0, 1, 0, 0.6f) : originalColor; // Change color based on selection
    }

    public void Darken(bool darken)
    {
        if (!isSelected)
        {
            GetComponent<SpriteRenderer>().color = darken ? new Color(0.5f, 0.5f, 0.5f, 1f) : originalColor; // Darken or reset color
        }
    }

    public bool IsSelected()
    {
        return isSelected;
    }
}
