using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // for Loading Scene!!

public class MenuControls : MonoBehaviour {

    #region Screen Variables

    private string[] screen_names = {
        "scrn_Start", "scrn_MapSelect", "scrn_HowToPlay", "scrn_Credits", "scrn_Options"
    };

    private GameObject[] screens;

    private enum Screens {
        scrn_Start,
        scrn_MapSelect,
        scrn_HowToPlay,
        scrn_Credits,
        scrn_Options,

        //Screen Helpers
        scrn_MapSelect2
    };

    #endregion

    #region Button Variables

    static private string[] button_names = {
        "btn0_1Player", "btn0_2Players", "btn0_HowToPlay", "btn0_Credits", "btn0_Options",
        "btn1_Play", "btn1_Back", "btn1_Previous", "btn1_Next",
        "btn2_Back",
        "btn3_Back",
        "btn4_Back"
    };

    private GameObject[] buttons;

    private enum Buttons {
        btn0_1Player,
        btn0_2Players,
        btn0_HowToPlay,
        btn0_Credits,
        btn0_Options,
        btn1_Play,
        btn1_Back,
        btn1_Previous,
        btn1_Next,
        btn2_Back,
        btn3_Back,
        btn4_Back
    };

    #endregion

    #region Map Variables

    static private string[] map_img_names1 = {
        "img_map1-1", "img_map2-1", "img_map3-1", "img_map4-1", "img_map5-1", "img_map6-1"
    };

    static private string[] map_img_names2 = {
        "img_map1-2", "img_map2-2", "img_map3-2", "img_map4-2", "img_map5-2", "img_map6-2"
    };

    static private string[] map_names = {
        "Map1_Circle", "Map2_Borders", "Map3_Xfactor", "Map4_Circle+", "Map5_Borders+", "Map6_Xfactor+"
    };

    private GameObject[] map_images1;
    private GameObject[] map_images2;

    private List<string[]> map_details_list = new List<string[]> {
        new string[] {"Circle", "Now in 3D!", "2/5", "3"},
        new string[] {"Borders", "Walls? Walls!", "3/5", "3"},
        new string[] {"The \"X\" Factor", "Moving Walls? Yes.", "4/5", "3"},
        new string[] { "Circle+", "More enemies!", "4/5", "7"},
        new string[] {"Borders+", "More wal- I mean enemies!", "5/5", "7"},
        new string[] {"The \"X\" Factor+", "Moving Walls? Yes. Also more enemies.", "4/5", "7"},
    };

    private enum MapDetails {
        Name,
        Description,
        Difficulty,
        EnemyCount
    }

    //UI Text
    private string[] map_select_text_names = {
        "txt_MapName", "txt_Description", "val_Difficulty", "val_Enemies"
    };

    public Text txt_Difficulty = null;
    public Text txt_Enemeies = null;

    private GameObject[] map_select_text;

    #endregion

    public GameObject sldVolume = null;

    //General
    private bool check_update = false;
    private int current_map = 0;

    void Awake() {
        #region Find Elements

        //Buttons
        buttons = new GameObject[button_names.Length];
        for (int i = 0; i < button_names.Length; i++) {
            buttons[i] = GameObject.Find(button_names[i]);
            Debug.Assert(buttons[i] != null);
        }

        //Map Images
        map_images1 = new GameObject[map_img_names1.Length];
        for (int i = 0; i < map_img_names1.Length; i++) {
            map_images1[i] = GameObject.Find(map_img_names1[i]);
            Debug.Assert(map_images1[i] != null);
            map_images1[i].SetActive(false);
        }

        map_images2 = new GameObject[map_img_names2.Length];
        for (int i = 0; i < map_img_names2.Length; i++) {
            map_images2[i] = GameObject.Find(map_img_names2[i]);
            Debug.Assert(map_images2[i] != null);
            map_images2[i].SetActive(false);
        }

        //MapSelection Text
        map_select_text = new GameObject[map_select_text_names.Length];
        for (int i = 0; i < map_select_text_names.Length; i++) {
            map_select_text[i] = GameObject.Find(map_select_text_names[i]);
            Debug.Assert(map_select_text[i] != null);
            map_select_text[i].GetComponent<Text>().text =
                map_details_list[current_map][i];
        }

        //Screens - KEEP THIS LAST TO DEACTIVATE
        screens = new GameObject[screen_names.Length];
        for (int i = 0; i < screen_names.Length; i++) {
            screens[i] = GameObject.Find(screen_names[i]);
            Debug.Assert(screens[i] != null);
            if (i != 0) {
                screens[i].SetActive(false);
            }
        }

        #endregion

        Debug.Assert(txt_Difficulty != null);
        Debug.Assert(txt_Enemeies != null);
    }

    // Start is called before the first frame update
    void Start() {
        #region Button Mapping

        //Start Screen

        #region Start Screen

        //Start Screen
        buttons[(int) Buttons.btn0_1Player].GetComponent<Button>().onClick.AddListener(
            () => SwitchScreen(Screens.scrn_MapSelect)
        );
        buttons[(int) Buttons.btn0_2Players].GetComponent<Button>().onClick.AddListener(
            () => SwitchScreen(Screens.scrn_MapSelect2)
        );
        buttons[(int) Buttons.btn0_HowToPlay].GetComponent<Button>().onClick.AddListener(
            () => SwitchScreen(Screens.scrn_HowToPlay)
        );
        buttons[(int)Buttons.btn0_Options].GetComponent<Button>().onClick.AddListener(
            () => SwitchScreen(Screens.scrn_Options)
        );
        buttons[(int) Buttons.btn0_Credits].GetComponent<Button>().onClick.AddListener(
            () => SwitchScreen(Screens.scrn_Credits)
        );

        #endregion

        //Map Selection Screen

        #region Map Selection Screen

        buttons[(int) Buttons.btn1_Play].GetComponent<Button>().onClick.AddListener(
            () => PlayMap()
        );
        buttons[(int) Buttons.btn1_Back].GetComponent<Button>().onClick.AddListener(
            () => SwitchScreen(Screens.scrn_Start)
        );
        buttons[(int) Buttons.btn1_Previous].GetComponent<Button>().onClick.AddListener(
            () => SwitchMap(false)
        );
        buttons[(int) Buttons.btn1_Next].GetComponent<Button>().onClick.AddListener(
            () => SwitchMap(true)
        );

        #endregion

        //How to Play Screen

        #region How to Play Screen

        buttons[(int) Buttons.btn2_Back].GetComponent<Button>().onClick.AddListener(
            () => SwitchScreen(Screens.scrn_Start)
        );

        #endregion

        //Options Screen
        sldVolume.GetComponent<Slider>().value = GameManager.Instance.GetVolume();
        buttons[(int)Buttons.btn4_Back].GetComponent<Button>().onClick.AddListener(
            () => SwitchScreen(Screens.scrn_Start)
        );

        //Credits Screen

        #region Credits Screen

        buttons[(int) Buttons.btn3_Back].GetComponent<Button>().onClick.AddListener(
            () => SwitchScreen(Screens.scrn_Start)
        );

        #endregion

        #endregion
    }

    // Update is called once per frame
    void Update() {
        if (check_update) {
            //if (screens[(int)Screens.scrn_MapSelect].activeSelf) {
            check_update = false;
            for (int i = 0; i < map_images1.Length; i++) {
                map_images1[i].SetActive(false);
            }

            for (int i = 0; i < map_images2.Length; i++) {
                map_images2[i].SetActive(false);
            }
            //}

            #region Updates

            if (GameManager.Instance.IsTwoPlayer() == false) {
                //Change Map Image for 1 Player
                map_images1[current_map].SetActive(true);
            } else {
                map_images2[current_map].SetActive(true);
            }

            //Change Map Name
            map_select_text[(int) MapDetails.Name].GetComponent<Text>().text
                = map_details_list[current_map][(int) MapDetails.Name];
            //Change Map Description
            map_select_text[(int) MapDetails.Description].GetComponent<Text>().text
                = map_details_list[current_map][(int) MapDetails.Description];
            //Change Difficulty Text
            map_select_text[(int) MapDetails.Difficulty].GetComponent<Text>().text
                = map_details_list[current_map][(int) MapDetails.Difficulty];
            //Change Enemies Text
            int enemies = int.Parse(map_details_list[current_map][(int) MapDetails.EnemyCount]);
            if (GameManager.Instance.IsTwoPlayer() == true) {
                enemies -= 1;
            }

            map_select_text[(int) MapDetails.EnemyCount].GetComponent<Text>().text
                = enemies.ToString();

            #endregion
        }

        FormatTextSize();
        AdjustVolume();
    }

    private void AdjustVolume() {
        GameManager.Instance.SetVolume(sldVolume.GetComponent<Slider>().value);
    }

    private void SwitchMap(bool isNext) {
        //Switch to previous or next map
        if (isNext) {
            current_map++;
        } else {
            current_map--;
        }

        if (GameManager.Instance.IsTwoPlayer() == false) {
            if (current_map == -1) {
                current_map = map_images1.Length - 1;
            } else if (current_map == map_images1.Length) {
                current_map = 0;
            }
        } else {
            if (current_map == -1) {
                current_map = map_images2.Length - 1;
            } else if (current_map == map_images2.Length) {
                current_map = 0;
            }
        }

        check_update = true;
    }

    private void PlayMap() {
        SceneManager.LoadScene(map_names[current_map]);
    }

    private void SwitchScreen(Screens screen) {
        int switch_to = -1;
        //Find screen to switch to
        switch (screen) {
            case Screens.scrn_Start:
                switch_to = (int) Screens.scrn_Start;
                break;
            case Screens.scrn_MapSelect:
                GameManager.Instance.setTwoPlayer(false);
                switch_to = (int) Screens.scrn_MapSelect;
                check_update = true;
                break;
            case Screens.scrn_MapSelect2:
                GameManager.Instance.setTwoPlayer(true);
                switch_to = (int) Screens.scrn_MapSelect;
                check_update = true;
                break;
            case Screens.scrn_HowToPlay:
                switch_to = (int) Screens.scrn_HowToPlay;
                break;
            case Screens.scrn_Options:
                switch_to = (int) Screens.scrn_Options;
                break;
            case Screens.scrn_Credits:
                switch_to = (int) Screens.scrn_Credits;
                break;
        }

        //Debugging
        if (switch_to == -1) {
            Debug.Log("Invalid Switch Screen");
            return;
        }

        //Switch Screen
        for (int i = 0; i < screens.Length; i++) {
            if (i == switch_to) {
                screens[i].SetActive(true);
                continue;
            }

            screens[i].SetActive(false);
        }
    }

    private void FormatTextSize() {
        //Variable Text
        map_select_text[(int) MapDetails.Name].GetComponent<Text>().fontSize = (int) (GameManager.Instance.GetCanvasHeight() * 0.078);
        map_select_text[(int) MapDetails.Description].GetComponent<Text>().fontSize = (int) (GameManager.Instance.GetCanvasHeight() * 0.026);
        map_select_text[(int) MapDetails.Difficulty].GetComponent<Text>().fontSize = (int) (GameManager.Instance.GetCanvasHeight() * 0.065);
        map_select_text[(int) MapDetails.EnemyCount].GetComponent<Text>().fontSize = (int) (GameManager.Instance.GetCanvasHeight() * 0.065);
        txt_Difficulty.fontSize = (int) (GameManager.Instance.GetCanvasHeight() * 0.065);
        txt_Enemeies.fontSize = (int) (GameManager.Instance.GetCanvasHeight() * 0.065);

        //Main Menu Buttons
        for (int i = 0; i < 5; i++) {
            buttons[i].GetComponentInChildren<Text>().fontSize = (int) (GameManager.Instance.GetCanvasHeight() * 0.052);
        }

        //Other Buttons
        for (int i = 5; i < buttons.Length; i++) {
            buttons[i].GetComponentInChildren<Text>().fontSize = (int) (GameManager.Instance.GetCanvasHeight() * 0.0417);
        }
    }
}