using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("HUD")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Game Over")]
    [SerializeField] private TMP_Text finalScoreText;

    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        startButton.onClick.AddListener(() => { AudioManager.Instance.PlayButtonClick(); GameManager.Instance.StartGame(); });
        restartButton.onClick.AddListener(() => { AudioManager.Instance.PlayButtonClick(); GameManager.Instance.RestartGame(); });
    }

    private void OnEnable()
    {
        
    }

    private void Start()
    {
        GameManager.Instance.OnStateChanged += HandleStateChanged;
        GameManager.Instance.OnScoreChanged += HandleScoreChanged;
        HandleStateChanged(GameManager.Instance.CurrentState);
        HandleScoreChanged(GameManager.Instance.Score);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnStateChanged -= HandleStateChanged;
        GameManager.Instance.OnScoreChanged -= HandleScoreChanged;
    }

    private void HandleStateChanged(GameState state)
    {
        loadingPanel.SetActive(state == GameState.Loading);
        startPanel.SetActive(state == GameState.MainMenu);
        hudPanel.SetActive(state == GameState.Playing);
        gameOverPanel.SetActive(state == GameState.GameOver);

        if (state == GameState.GameOver)
            finalScoreText.text = $"Pulpits walked: {GameManager.Instance.Score}";
    }

    private void HandleScoreChanged(int score)
    {
        scoreText.text = $"Score: {score}";
    }
}
