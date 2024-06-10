using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;  // UI Text to display the score
    private int score;  // Score variable

    private void Start()
    {
        score = 0;  // Initialize score
        UpdateScoreText();  // Update the score display
    }

    public void UpdateScore(int tilesCount)
    {
        // Update the score based on the number of tiles selected
        score += tilesCount * 10; // Example scoring logic: 10 points per tile
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }
}
