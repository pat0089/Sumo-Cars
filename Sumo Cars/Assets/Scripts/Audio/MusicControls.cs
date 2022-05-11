using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicControls : MonoBehaviour {

    #region

    private static MusicControls _musicControls;
    private static GameManager _gameManager;

    private AudioSource _bgm;

    //Background Music
    private AudioClip _bgmMenu;
    private AudioClip _bgmPregame;
    private AudioClip _bgmIngame;
    private AudioClip _bgmPostgame;

    //Sound Effects
    private AudioClip _sfxCollision;
    private AudioClip _sfxCountdown;
    private AudioClip _sfxElimination;
    private AudioClip _sfxGameWon;
    private AudioClip _sfxGameLost;
    private AudioClip _sfxMenuHover;
    private AudioClip _sfxMenuClick;
    private AudioClip _sfxBoostPickup;
    private AudioClip _sfxBoostUsed;

    private float _volume = 0.3f;

    private void Awake() {
        _gameManager = FindObjectOfType<GameManager>();
        Debug.Assert(_gameManager != null);

        _bgmMenu = Resources.Load<AudioClip>("Audio/Music/Menu/bgm_Menu");
        _bgmPregame = Resources.Load<AudioClip>("Audio/Music/InGame/bgm_PreGame");
        _bgmIngame = Resources.Load<AudioClip>("Audio/Music/InGame/bgm_InGame");
        _bgmPostgame = Resources.Load<AudioClip>("Audio/Music/InGame/bgm_PostGame");

        //sfx_collision = Resources.Load<AudioClip>("Audio/SFX/");
        _sfxCountdown = Resources.Load<AudioClip>("Audio/SFX/sfx_countdown");
        _sfxElimination = Resources.Load<AudioClip>("Audio/SFX/sfx_elimination");
        _sfxGameWon = Resources.Load<AudioClip>("Audio/SFX/sfx_gameWon");
        _sfxGameLost = Resources.Load<AudioClip>("Audio/SFX/sfx_gameLost");
        //sfx_menuHover = Resources.Load<AudioClip>("Audio/SFX/");
        _sfxMenuClick = Resources.Load<AudioClip>("Audio/SFX/sfx_menuClick");
        _sfxBoostPickup = Resources.Load<AudioClip>("Audio/SFX/sfx_boostPickup");
        _sfxBoostUsed = Resources.Load<AudioClip>("Audio/SFX/sfx_boostUsed");

        if (_musicControls == null) // Sets up the global Music Controller
        {
            _musicControls = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject); // Destroys current GameManger object if one already exists
        }

        _bgm = GetComponent<AudioSource>();
    }

    public void SetVolume(float v) {
        _volume = v;
        AudioSource[] clips = FindObjectsOfType<AudioSource>();
        foreach (AudioSource a in clips) {
            a.volume = v;
        }
    }

    public float GetVolume() {
        return _volume;
    }

    public void PlayMusic(AudioSource src, bool loop) {
        src.loop = loop;
        src.volume = _volume;
        if (src.isPlaying) return;
        src.Play();
    }

    public void StopMusic(AudioSource src) {
        src.Stop();
    }

    #endregion

    // Start is called before the first frame update
    void Start() {
        CheckBGMState();
    }

    // Update is called once per frame
    void Update() {
        DestroySFX();
    }

    public void CheckBGMState() {
        //Debug.Log("GS: " + _gameManager.GetState());
        //Debug.Log("IGS: " + GameManager.Instance.GetState());
        switch (GameManager.Instance.GetState()) {
            case GameState.MainMenu:
                foreach (AudioSource track in GetComponents<AudioSource>()) {
                    if (!track.Equals(_bgm)) {
                        Destroy(track);
                    }
                }

                _bgm.clip = _bgmMenu;
                break;
            case GameState.Pregame:
                _bgm.clip = _bgmPregame;
                break;
            case GameState.Ingame:
                _bgm.clip = _bgmIngame;
                break;
            case GameState.Postgame:
                _bgm.clip = _bgmPostgame;
                break;
        }

        PlayMusic(_bgm, true);
    }

    private void DestroySFX() {
        foreach (AudioSource track in GetComponents<AudioSource>()) {
            if (!track.isPlaying && !track.Equals(_bgm)) {
                Destroy(track);
            }
        }
    }

    public void PlayCountdown() {
    }

    public void PlayElimination() {
        AudioSource asrc = gameObject.AddComponent<AudioSource>();
        asrc.clip = _sfxElimination;
        PlayMusic(asrc, false);
    }

    public void PlayGameWon() {
        AudioSource asrc = gameObject.AddComponent<AudioSource>();
        asrc.clip = _sfxGameWon;
        PlayMusic(asrc, false);
    }

    public void PlayGameLost() {
        AudioSource asrc = gameObject.AddComponent<AudioSource>();
        asrc.clip = _sfxGameLost;
        PlayMusic(asrc, false);
    }

    public void PlayMenuClick() {
    }

    public void PlayBoostPickup() {
    }

    public void PlayBoostUsed() {
    }
}