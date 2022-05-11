using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // for Loading Scene!!

public class DemoControls : MonoBehaviour {
    public GameObject back;

    void Awake() {
        //Buttons
        Debug.Assert(back != null);
    }

    // Start is called before the first frame update
    void Start() {
        back.GetComponent<Button>().onClick.AddListener(() => SwitchScene("MainMenu"));
    }

    // Update is called once per frame
    void Update() {
    }

    private void SwitchScene(string scene) {
        GameManager.Instance.SetState(GameState.MainMenu, false);
        SceneManager.LoadScene(scene);
    }
}