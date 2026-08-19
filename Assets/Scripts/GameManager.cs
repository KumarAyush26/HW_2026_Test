using System;
using UnityEngine;

public enum GameState
{
    Loading,
    MainMenu,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene References")]
    [SerializeField] private PulpitSpawner pulpitSpawner;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private UIManager uiManager;

    public GameState CurrentState { get; private set; } = GameState.Loading;
    public int Score { get; private set; }

    public event Action<int> OnScoreChanged;
    public event Action<GameState> OnStateChanged;

    private GameObject activePlayer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        SetState(GameState.Loading);
        ConfigLoader.Instance.Load(OnConfigLoaded);
    }

    private void OnEnable()
    {
        OnScoreChanged += LogScore; 
    }

    private void LogScore(int newScore)
    {
        Debug.Log($"[Score] {newScore}");
    }

    private void OnConfigLoaded()
    {
        SetState(GameState.MainMenu);
        StartGame(); //change it later
    }

    public void StartGame()
    {
        Score = 0;
        OnScoreChanged?.Invoke(Score);

        if (activePlayer != null)
            Destroy(activePlayer);

        activePlayer = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);

        pulpitSpawner.BeginSpawning();

        SetState(GameState.Playing);
    }

    public void RegisterSuccessfulMove()
    {
        if (CurrentState != GameState.Playing) return;

        Score++;
        OnScoreChanged?.Invoke(Score);
    }

    public void ReportPlayerFell()
    {
        if (CurrentState != GameState.Playing) return;

        pulpitSpawner.StopSpawning();
        SetState(GameState.GameOver);
    }

    public void RestartGame()
    {
        pulpitSpawner.ResetSpawner();
        StartGame();
    }

    private void SetState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }
}
