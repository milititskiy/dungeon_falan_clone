using UnityEngine;

public class Background : MonoBehaviour
{
    public SpriteRenderer backgroundSprite;

    private void Start()
    {
        backgroundSprite.sortingLayerName = "Background";
    }
}
