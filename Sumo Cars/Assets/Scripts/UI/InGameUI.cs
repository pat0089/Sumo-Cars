using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; // for Loading Scene!!

public class InGameUI : MonoBehaviour {
    public Text txtGameClock = null;
    public Text txtPlayersLeft = null;
    public Text txtTip = null;
    public Text txtGameOver = null;
    public Text txtPregameTimer = null;

    private bool _showOptions = false;
    public GameObject pnlOptions = null;
    public GameObject pnlEndGame = null;
    public GameObject sldVolume = null;
    public GameObject btnResume = null;
    public GameObject btnHowToPlay = null;
    public GameObject btnMainMenu = null;
    public GameObject btnGameOver = null;
    public GameObject btnReplay = null;

    public GameObject scrnHowToPlay = null;
    public GameObject btnBackToGame = null;

    void Awake() {
        Debug.Assert(pnlOptions != null);
        Debug.Assert(pnlEndGame != null);
        Debug.Assert(btnResume != null);
        Debug.Assert(btnReplay != null);
        Debug.Assert(btnMainMenu != null);
        Debug.Assert(txtGameClock != null);
        Debug.Assert(txtPlayersLeft != null);
        Debug.Assert(txtTip != null);
        Debug.Assert(txtGameOver != null);
        Debug.Assert(txtPregameTimer != null);
        Debug.Assert(sldVolume != null);
        Debug.Assert(scrnHowToPlay != null);
        Debug.Assert(btnBackToGame != null);
        Debug.Assert(btnGameOver != null);
    }

    // Start is called before the first frame update
    void Start() {
        btnReplay.GetComponent<Button>().onClick.AddListener(() => SwitchScene(SceneManager.GetActiveScene().name));
        btnResume.GetComponent<Button>().onClick.AddListener(() => ResumeGame());
        btnMainMenu.GetComponent<Button>().onClick.AddListener(() => SwitchScene("MainMenu"));
        btnGameOver.GetComponent<Button>().onClick.AddListener(() => SwitchScene("MainMenu"));
        btnHowToPlay.GetComponent<Button>().onClick.AddListener(() => SwitchScreen("HowToPlay"));
        btnBackToGame.GetComponent<Button>().onClick.AddListener(() => SwitchScreen("Game"));
        sldVolume.GetComponent<Slider>().value = GameManager.Instance.GetVolume();
        pnlOptions.SetActive(false);
        scrnHowToPlay.SetActive(false);
        pnlEndGame.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        txtGameClock.text = FormatTime(GameManager.Instance.GetGameClock());
        txtPlayersLeft.text = FormatRemainingPlayers(FindObjectsOfType<CarBehaviour>().Length);
        txtGameOver.text = FormatGameOver();
        txtPregameTimer.text = FormatPregameTimer();
        if (Keyboard.current[Key.Escape].wasPressedThisFrame && GameManager.Instance.GetState() == GameState.Ingame) {
            if (scrnHowToPlay.activeSelf) {
                SwitchScreen("Game");
            } else {
                pnlOptions.SetActive(!pnlOptions.activeSelf);
                if (pnlOptions.activeSelf == true) {
                    Time.timeScale = 0;
                } else {
                    Time.timeScale = 1;
                }
            }
        }

        if (GameManager.Instance.GetState() == GameState.Postgame) {
            pnlEndGame.SetActive(true);
        }

        AdjustVolume();
        FormatTextSize();
    }

    private void ResumeGame() {
        pnlOptions.SetActive(false);
        Time.timeScale = 1;
    }

    private string FormatTime(double time) {
        int minutes = (int) (time / 60);
        int seconds = (int) (time % 60);
        return $"{minutes}:{seconds,2:00}";
    }

    //WIP
    private string FormatRemainingPlayers(int count) {
        return "Remaining: " + count;
    }

    private string FormatGameOver() {
        return GameManager.Instance.GetGameOver();
    }

    private string FormatPregameTimer() {
        double timer = GameManager.Instance.GetPregameTimer();
        if (GameManager.Instance.GetState() == GameState.Pregame) {
            if (timer > 3) {
                txtPregameTimer.fontSize = (int) (GameManager.Instance.GetCanvasHeight() * 0.05);
                return "The game will\nbegin in...";
            }

            txtPregameTimer.fontSize = (int) (GameManager.Instance.GetCanvasHeight() * 0.1);
            return Mathf.Ceil((float) timer).ToString();
        } else {
            return "";
        }
    }

    private void SwitchScreen(string s) {
        if (s.Equals("HowToPlay")) {
            scrnHowToPlay.SetActive(true);
        } else if (s.Equals("Game")) {
            scrnHowToPlay.SetActive(false);
        }
    }

    private void SwitchScene(string scene) {
        if (scene.Equals("MainMenu")) {
            Time.timeScale = 1;
            GameManager.Instance.SetState(GameState.MainMenu, false);
        } else {
            
            GameManager.Instance.SetState(GameState.Pregame, true);
        }
        pnlEndGame.SetActive(false);
        SceneManager.LoadScene(scene);
    }

    private void AdjustVolume() {
        GameManager.Instance.SetVolume(sldVolume.GetComponent<Slider>().value);
    }

    private void FormatTextSize() {
        //Variable Text
        txtGameClock.fontSize = (int) (GameManager.Instance.GetCanvasHeight() * 0.0417);
        txtPlayersLeft.fontSize = (int) (GameManager.Instance.GetCanvasHeight() * 0.0417);
        txtGameOver.fontSize = (int) (GameManager.Instance.GetCanvasHeight() * 0.0417);
        txtPregameTimer.fontSize = (int) (GameManager.Instance.GetCanvasHeight() * 0.0417);

        //Other Text
        txtTip.fontSize = (int) (GameManager.Instance.GetCanvasHeight() * 0.0417);
        Text[] text = pnlOptions.GetComponentsInChildren<Text>();
        for (int i = 0; i < text.Length; i++) {
            if (text[i].text == "Volume") {
                text[i].fontSize = (int) (GameManager.Instance.GetCanvasHeight() * 0.03125);
                break;
            }

            text[i].fontSize = (int) (GameManager.Instance.GetCanvasHeight() * 0.0417);
        }

        Text txtBackToGame = scrnHowToPlay.GetComponentInChildren<Text>();
        txtBackToGame.fontSize = (int) (GameManager.Instance.GetCanvasHeight() * 0.0417);
        text = pnlEndGame.GetComponentsInChildren<Text>();
        for (int i = 0; i < text.Length; i++) {
            text[i].fontSize = (int) (GameManager.Instance.GetCanvasHeight() * 0.0417);
        }
    }
}