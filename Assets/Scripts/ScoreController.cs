using UnityEngine;
using UnityEngine.UIElements;

public class ScoreController : MonoBehaviour
{
    public int score = 0;
    private Label scoreLabel;
    private UIDocument uiDocument;

    private void Awake()
    {
        // Check for one existing ScoreController and don't create a new one if it exists
        ScoreController[] existingControllers = FindObjectsOfType<ScoreController>();

        if (existingControllers.Length > 1)
        {
            // If an existing controller is found, destroy this one
            Destroy(this.gameObject);
            return;
        }

        uiDocument = FindFirstObjectByType<UIDocument>();

        scoreLabel = uiDocument.rootVisualElement.Q<Label>("Score");

        // Persist this from game scene to end game scene
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        UpdateScoreLabel();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreLabel();
    }

    private void UpdateScoreLabel()
    {
        if (scoreLabel != null)
        {
            scoreLabel.text = "Score: " + score.ToString();
        }
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreLabel();
    }
}
