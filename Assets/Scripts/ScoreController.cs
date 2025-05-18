using UnityEngine;
using UnityEngine.UIElements;

public class ScoreController : MonoBehaviour
{
    public int score = 0;
    private Label scoreLabel;
    private UIDocument uiDocument;

    private void Awake()
    {
        uiDocument = FindFirstObjectByType<UIDocument>();

        scoreLabel = uiDocument.rootVisualElement.Q<Label>("Score");
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
            scoreLabel.text = score.ToString();
        }
    }
}
