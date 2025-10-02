using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    [Header("Player Spawning")]
    [SerializeField] private List<PlayerSpawnSettings> playerSpawnSettings = new();
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform ballSpawnPoint;

    [Header("UI & Gameplay")]
    [SerializeField] private ScoreBoard scoreBoard;
    [SerializeField] private Timer timer;
    [SerializeField, Min(0f)] private float respawnDelay = 1.25f;
    [SerializeField, Min(0f)] private float ballLaunchForce = 12f;

    [Header("Goal Colliders")]
    [SerializeField] private BoxCollider2D gatesColliderLeft;
    [SerializeField] private BoxCollider2D gatesColliderRight;

    private readonly Dictionary<PlayerInput.ControlScheme, GameObject> _playerInstances = new();
    private IEntitySpawner _entitySpawner;
    private PlayerSpawner _playerSpawner;
    private GameObject _ballInstance;
    private Coroutine _goalRoutine;
    private bool _isGameActive;
    private bool _isRespawning;
    private int _playerOneScore;
    private int _playerTwoScore;

    public static GameManager Instance => _instance;
    public IReadOnlyDictionary<PlayerInput.ControlScheme, GameObject> PlayerInstances => _playerInstances;
    public GameObject BallInstance => _ballInstance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        _entitySpawner = new EntitySpawner();
        _playerSpawner = new PlayerSpawner(_entitySpawner, new PlayerConfigurator());

        EnsurePlayerSettings();
    }

    private void Start()
    {
        EnsureUiReferences();
        InitializeGame();
    }

    private void Update()
    {
        if (!_isGameActive || timer == null)
        {
            return;
        }

        if (timer.RemainingTime <= 0f)
        {
            EndGame();
        }
    }

    public void InitializeGame()
    {
        EnsureUiReferences();

        SpawnPlayers();
        ResetScores();

        _isRespawning = false;
        _isGameActive = true;

        SetGatesEnabled(true);
        ResetPlayersToSpawn();
        ResetBall(null);

        if (timer != null)
        {
            timer.enabled = true;
        }
    }

    public GameObject GetPlayer(PlayerInput.ControlScheme controlScheme)
    {
        return _playerInstances.TryGetValue(controlScheme, out GameObject instance) ? instance : null;
    }

    internal void GoalScored(int gateId)
    {
        if (!_isGameActive || _isRespawning)
        {
            return;
        }

        PlayerInput.ControlScheme losingPlayer;

        switch (gateId)
        {
            case 0:
                _playerTwoScore++;
                losingPlayer = PlayerInput.ControlScheme.PlayerOne;
                break;
            case 1:
                _playerOneScore++;
                losingPlayer = PlayerInput.ControlScheme.PlayerTwo;
                break;
            default:
                Debug.LogWarning($"GameManager: Unsupported gate id {gateId}.", this);
                return;
        }

        UpdateScoreBoard();

        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        if (_goalRoutine != null)
        {
            StopCoroutine(_goalRoutine);
        }

        _goalRoutine = StartCoroutine(HandleGoalRoutine(losingPlayer));
    }

    private IEnumerator HandleGoalRoutine(PlayerInput.ControlScheme losingPlayer)
    {
        _isRespawning = true;
        SetGatesEnabled(false);

        if (_ballInstance != null && _ballInstance.TryGetComponent(out Rigidbody2D currentBallBody))
        {
            currentBallBody.linearVelocity = Vector2.zero;
            currentBallBody.angularVelocity = 0f;
        }

        if (_ballInstance != null)
        {
            _ballInstance.SetActive(false);
        }

        if (respawnDelay > 0f)
        {
            yield return new WaitForSeconds(respawnDelay);
        }

        ResetPlayersToSpawn();
        ResetBall(losingPlayer);

        SetGatesEnabled(true);
        _isRespawning = false;
        _goalRoutine = null;
    }

    private void ResetScores()
    {
        _playerOneScore = 0;
        _playerTwoScore = 0;
        UpdateScoreBoard();
    }

    private void UpdateScoreBoard()
    {
        ScoreBoard board = scoreBoard ?? FindObjectOfType<ScoreBoard>();
        if (board == null)
        {
            return;
        }

        scoreBoard = board;
        board.SetScore(_playerOneScore, _playerTwoScore);
    }

    private void ResetPlayersToSpawn()
    {
        foreach (PlayerSpawnSettings settings in playerSpawnSettings)
        {
            if (settings == null)
            {
                continue;
            }

            GameObject playerInstance = GetPlayer(settings.ControlScheme);
            if (playerInstance == null)
            {
                continue;
            }

            Transform spawnPoint = settings.SpawnPoint;
            if (spawnPoint != null)
            {
                playerInstance.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            }

            if (playerInstance.TryGetComponent(out Rigidbody2D body))
            {
                body.linearVelocity = Vector2.zero;
                body.angularVelocity = 0f;
            }

            PlayerInput input = playerInstance.GetComponent<PlayerInput>();
            if (input != null && !input.enabled)
            {
                input.enabled = true;
            }
        }
    }

    private void ResetBall(PlayerInput.ControlScheme? losingPlayer)
    {
        _ballInstance = _entitySpawner.Replace(ballPrefab, _ballInstance, ballSpawnPoint);

        if (_ballInstance == null)
        {
            return;
        }

        Rigidbody2D body = _ballInstance.GetComponent<Rigidbody2D>();
        if (body == null)
        {
            return;
        }

        body.linearVelocity = Vector2.zero;
        body.angularVelocity = 0f;

        if (!losingPlayer.HasValue)
        {
            return;
        }

        Vector2 launchDirection = ResolveLaunchDirection(losingPlayer.Value);
        if (launchDirection.sqrMagnitude <= 0f)
        {
            return;
        }

        body.AddForce(launchDirection.normalized * ballLaunchForce, ForceMode2D.Impulse);
    }

    private Vector2 ResolveLaunchDirection(PlayerInput.ControlScheme losingPlayer)
    {
        PlayerSpawnSettings settings = GetSpawnSettings(losingPlayer);
        if (settings == null || settings.SpawnPoint == null || ballSpawnPoint == null)
        {
            return losingPlayer == PlayerInput.ControlScheme.PlayerOne ? Vector2.left : Vector2.right;
        }

        Vector2 direction = settings.SpawnPoint.position - ballSpawnPoint.position;
        if (direction.sqrMagnitude < 0.0001f)
        {
            direction = losingPlayer == PlayerInput.ControlScheme.PlayerOne ? Vector2.left : Vector2.right;
        }

        return direction;
    }

    private void SetGatesEnabled(bool enabled)
    {
        if (gatesColliderLeft != null)
        {
            gatesColliderLeft.enabled = enabled;
        }

        if (gatesColliderRight != null)
        {
            gatesColliderRight.enabled = enabled;
        }
    }

    private void EndGame()
    {
        if (!_isGameActive)
        {
            return;
        }

        _isGameActive = false;

        if (_goalRoutine != null)
        {
            StopCoroutine(_goalRoutine);
            _goalRoutine = null;
        }

        SetGatesEnabled(false);

        foreach (GameObject instance in _playerInstances.Values)
        {
            if (instance == null)
            {
                continue;
            }

            if (instance.TryGetComponent(out PlayerInput input))
            {
                input.enabled = false;
            }

            if (instance.TryGetComponent(out Rigidbody2D body))
            {
                body.linearVelocity = Vector2.zero;
                body.angularVelocity = 0f;
            }
        }

        if (_ballInstance != null && _ballInstance.TryGetComponent(out Rigidbody2D ballBody))
        {
            ballBody.linearVelocity = Vector2.zero;
            ballBody.angularVelocity = 0f;
        }

        if (timer != null)
        {
            timer.enabled = false;
        }
    }

    private void SpawnPlayers()
    {
        HashSet<PlayerInput.ControlScheme> processedSchemes = new HashSet<PlayerInput.ControlScheme>();

        foreach (PlayerSpawnSettings settings in playerSpawnSettings)
        {
            if (settings == null)
            {
                continue;
            }

            GameObject existingInstance = GetPlayer(settings.ControlScheme);
            GameObject playerInstance = _playerSpawner.SpawnPlayer(settings, existingInstance);

            if (playerInstance == null)
            {
                continue;
            }

            _playerInstances[settings.ControlScheme] = playerInstance;
            processedSchemes.Add(settings.ControlScheme);
        }

        if (processedSchemes.Count == 0)
        {
            return;
        }

        foreach (PlayerInput.ControlScheme scheme in new List<PlayerInput.ControlScheme>(_playerInstances.Keys))
        {
            if (processedSchemes.Contains(scheme))
            {
                continue;
            }

            GameObject orphanedInstance = _playerInstances[scheme];
            if (orphanedInstance != null)
            {
                Destroy(orphanedInstance);
            }

            _playerInstances.Remove(scheme);
        }
    }

    private void EnsurePlayerSettings()
    {
        if (playerSpawnSettings != null && playerSpawnSettings.Count > 0)
        {
            return;
        }
    }

    private void EnsureUiReferences()
    {
        if (scoreBoard == null)
        {
            scoreBoard = FindObjectOfType<ScoreBoard>();
        }

        if (timer == null)
        {
            timer = FindObjectOfType<Timer>();
        }
    }

    private PlayerSpawnSettings GetSpawnSettings(PlayerInput.ControlScheme scheme)
    {
        foreach (PlayerSpawnSettings settings in playerSpawnSettings)
        {
            if (settings != null && settings.ControlScheme == scheme)
            {
                return settings;
            }
        }

        return null;
    }
}
