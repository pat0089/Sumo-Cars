using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    private static GameManager _gameManager;

    public GameObject PowerupPrefab { get; private set; }

    private GameState _currentGameState;
    private double _gameClock = 0;
    private string _gameOverStatus = "";

    private int _playersRemaining = 0;
    private int _player2Remaining = 0;
    private int _enemiesRemaining = 0;

    private double _pregameTimer = 5;
    private ArenaBehaviour _currentArena;
    private PowerupController _powerups;
    private MusicControls _musicControls;
    private CarBehaviour _player;
    private CarBehaviour _player2;
    private List<CarBehaviour> _cars = new List<CarBehaviour>();
    private bool twoPlayers = false;

    private GameObject _canvas = null;
    private float _canvasHeight = 0;
    private float _canvasWidth = 0;

    private void Awake() {
        if (_gameManager == null) { // Sets up the global GameManager
            _gameManager = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // Add scene loading listener
        } else {
            Destroy(gameObject); // Destroys current GameManger object if one already exists
            return;
        }

        PowerupPrefab = Resources.Load<GameObject>("Prefabs/Powerup");

        _musicControls = GetComponent<MusicControls>();
        Debug.Assert(_musicControls != null, "No Music Controls!");

        _canvas = GameObject.Find("Canvas");
        Debug.Assert(_canvas != null, "No Canvas!");

        _canvasHeight = _canvas.GetComponent<RectTransform>().rect.height;
        _canvasWidth = _canvas.GetComponent<RectTransform>().rect.width;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
        Debug.Log("Scene loaded (" + loadSceneMode + "): " + scene.name + ".");


        foreach (GameObject go in scene.GetRootGameObjects()) { // Detect type of scene
            if (go.GetComponent<ArenaBehaviour>() != null) {
                SetState(GameState.Pregame, false);

                _currentArena = go.GetComponent<ArenaBehaviour>();
                _currentArena._gameManager = this;
                _powerups = go.GetComponent<PowerupController>();
                _cars.Clear();
                foreach (GameObject car in GameObject.FindGameObjectsWithTag("Car")) {
                    CarBehaviour carBehaviour = car.GetComponent<CarBehaviour>();
                    if (carBehaviour == null) {
                        Debug.LogError("GameObject tagged with `car` does not have CarBehaviour!");
                        continue;
                    }

                    _cars.Add(carBehaviour);
                }
            }
        }
    }

    private void Start() {
    }

    private void Update() {
        //Debug.Log("Gamestate: " + GetState());
        if (_currentGameState == GameState.Pregame) {
            _gameOverStatus = "";
            _gameClock = 0;
            _player2Remaining = FindObjectsOfType<TwoPlayerRemover>().Length;
            _playersRemaining = FindObjectsOfType<PlayerController>().Length;
            _enemiesRemaining = FindObjectsOfType<EnemyController>().Length;

            if (_pregameTimer > 0) {
                Time.timeScale = 0;
                _pregameTimer -= Time.unscaledDeltaTime;
            } else {
                //Start the clock
                Time.timeScale = 1;
                SetState(GameState.Ingame, false);
                //Start Powerup Spawning
                _currentArena.StartPowerupTimer();
            }
        }

        if (GetState() == GameState.Ingame) {
            _gameClock += Time.deltaTime;
            CheckElimination();
            CheckGameOver();

            CheckTargets();
        }

        _canvas = GameObject.Find("Canvas");
        _canvasHeight = _canvas.GetComponent<RectTransform>().rect.height;
        _canvasWidth = _canvas.GetComponent<RectTransform>().rect.width;
    }

    private void CheckTargets() {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        if (players.Length == 0) {
            return;
        }

        bool isPlayerTargeted = false;
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        foreach (EnemyController e in enemies) {
            foreach (PlayerController p in players) {
                if (e.curTarget == p.gameObject) {
                    isPlayerTargeted = true;
                }
            }
        }

        if (!isPlayerTargeted && enemies.Length > 0) {
            int index = Random.Range(0, enemies.Length);
            Debug.Log(enemies[index].name + ": CHANGE TO PLAYER TARGET FROM \"" + enemies[index].GetComponent<EnemyController>().curTarget.name + "\"");
            enemies[index].GetNextTarget();
        }
    }

    private void FixedUpdate() {
    }

    public float GetCanvasHeight() {
        return _canvasHeight;
    }

    public float GetCanvasWidth() {
        return _canvasWidth;
    }

    private void CheckElimination() {
        if (FindObjectsOfType<PlayerController>().Length < _playersRemaining) {
            _playersRemaining--;
            _musicControls.PlayElimination();
        }

        if (FindObjectsOfType<TwoPlayerRemover>().Length < _player2Remaining) {
            _player2Remaining--;
            _musicControls.PlayElimination();
        }

        if (FindObjectsOfType<EnemyController>().Length < _enemiesRemaining) {
            _enemiesRemaining--;
            _musicControls.PlayElimination();
        }
    }

    private void CheckGameOver() {
        var players = FindObjectsOfType<PlayerController>();
        var numPlayers = FindObjectsOfType<PlayerController>().Length;

        int numEnemies = FindObjectsOfType<EnemyController>().Length;
        if (numPlayers == 0 || (numEnemies == 0 && numPlayers == 1)) {
            if (numPlayers == 0) {
                _musicControls.PlayGameLost();
                _gameOverStatus = "You lose!";
            } else {
                _musicControls.PlayGameWon();
                if (twoPlayers) {
                    _gameOverStatus = players[0].name + " wins!";
                } else {
                    _gameOverStatus = "You win!";
                }
            }

            SetState(GameState.Postgame, false);
            Time.timeScale = 0;
        }
    }

    public double GetPregameTimer() {
        return _pregameTimer;
    }

    public double GetGameClock() {
        return _gameClock;
    }

    public string GetGameOver() {
        return _gameOverStatus;
    }

    public void setTwoPlayer(bool isTwoPlayer) {
        twoPlayers = isTwoPlayer;
    }

    public bool IsTwoPlayer() {
        return twoPlayers;
    }

    //Public functions (shared across scenes)
    public void SetState(GameState state, bool isReplay) {
        _currentGameState = state;
        if (isReplay || (state != GameState.Ingame && state != GameState.Pregame && state != GameState.Postgame)) {
            _pregameTimer = 5;
        }

        gameObject.GetComponent<MusicControls>().CheckBGMState();
    }

    public GameState GetState() {
        return _currentGameState;
    }

    public void SetVolume(float v) {
        _musicControls.SetVolume(v);
    }

    public float GetVolume() {
        return _musicControls.GetVolume();
    }

    public void DestroyCar(CarBehaviour car) {
        _cars.Remove(car);
        Destroy(car.gameObject);
    }

    public void Powerup(GameObject car) {
        //Debug.Log("in powerup");
        CarBehaviour thisCar = car.GetComponent<CarBehaviour>();
        if (thisCar.powerUpType == 3 || thisCar.powerUpType == 5) {
            _powerups.chooseTrigger(car, car, car.GetComponent<CarBehaviour>().powerUpType);
        } else {
            foreach (CarBehaviour opponent in _cars) {
                //Debug.Log("in loop");
                if (opponent != thisCar) {
                    //Debug.Log("before gamemanager function");
                    _powerups.chooseTrigger(opponent.gameObject, car, thisCar.powerUpType);
                }
            }
        }
    }

    public ArenaBehaviour Arena => _currentArena;
    public List<CarBehaviour> CarsList => _cars;

    public static GameManager Instance { // The GameManager instance
        get {
            if (_gameManager == null) {
                Debug.LogError("No GameManager instance exists! Has any scene loaded a GameManager yet?");
            }

            return _gameManager;
        }
    }

}