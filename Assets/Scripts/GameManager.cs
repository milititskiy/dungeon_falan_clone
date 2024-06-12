using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Player player;
    public GridManager gridManager;
    public SelectionManager selectionManager;
    public ScoreManager scoreManager;
    public LineManager lineManager;

    public bool inBattle; // Track the current state

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

            // Initialize the inBattle state
            inBattle = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize the game in the appropriate state
        Debug.Log("Game started. Initial inBattle state: " + inBattle);
        if (inBattle)
        {
            InitializeBattle();
        }
        else
        {
            InitializeNotBattle();
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

    public void ToggleBattleState()
    {
        Debug.Log("Before toggle, inBattle state: " + inBattle);
        inBattle = !inBattle; // Toggle the inBattle state
        Debug.Log("After toggle, inBattle state: " + inBattle);
        if (inBattle)
        {
            InitializeBattle();
        }
        else
        {
            InitializeNotBattle();
        }
    }

    public void InitializeBattle()
    {
        Debug.Log("Initializing Battle State");
        // Clear the grid before initializing
        gridManager.ClearGrid();

        // Populate grid with tiles and start the battle
        gridManager.InitializeGrid();
    }

    public void InitializeNotBattle()
    {
        Debug.Log("Initializing Not Battle State");
        // Clear the grid and place the player
        gridManager.ClearGrid();
        gridManager.PlacePlayer(player);
    }
}
