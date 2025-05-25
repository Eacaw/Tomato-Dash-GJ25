using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class EndgameButtonController : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Button tryAgainButton;

    [SerializeField]
    private UnityEngine.UI.Button mainMenuButton;
    public UIDocument uiDocument;
    private Label gameOverScoreLabel;

    private void Start()
    {
        // find the scoreController and display the score in the UI
        ScoreController scoreController = FindObjectOfType<ScoreController>();
        if (scoreController != null)
        {
            // Assuming you have a method to display the score in the UI
            gameOverScoreLabel = uiDocument.rootVisualElement.Q<Label>("GameOverScore");
            if (gameOverScoreLabel != null)
            {
                gameOverScoreLabel.text = scoreController.score.ToString() + " points";
            }
        }

        // Add listeners to the buttons
        tryAgainButton.onClick.AddListener(() => LoadScene("GameWorld"));
        mainMenuButton.onClick.AddListener(() => LoadScene("MainMenu"));
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        Debug.Log("Clicked button");
    }
}
