using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Player player;
    public GridManager gridManager;
    public SelectionManager selectionManager;
    public ScoreManager scoreManager;
    public LineManager lineManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Assign the manager components
            gridManager = GetComponent<GridManager>();
            selectionManager = GetComponent<SelectionManager>();
            scoreManager = GetComponent<ScoreManager>();
            lineManager = GetComponent<LineManager>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            selectionManager.DeselectTile();
        }

        if (Input.GetMouseButtonUp(0))
        {
            selectionManager.ProcessSelection();
        }
    }
}
