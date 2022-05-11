using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TwoPlayerRemover : MonoBehaviour
{
    // Start is called before the first frame update
    //public bool twoPlayers = false;
    public GameObject TwoPlayerModel = null;
    public GameObject PlayerTwoTrail = null;

    void Start()
    {
        if(GameManager.Instance.IsTwoPlayer() == true)
        {
            var player1 = FindObjectOfType<PlayerController>().gameObject;
            player1.name = "Player 1";
            Destroy(GetComponent<EnemyController>());
            var player2 = gameObject.AddComponent<PlayerController>();
            player2.leftKey = Key.LeftArrow;
            player2.rightKey = Key.RightArrow;
            player2.powerUp = Key.RightShift;
            player2.accelerateKey = Key.UpArrow;
            player2.reverseKey = Key.DownArrow;
            name = "Player 2";

            //grab and delete the current model
            var enemyModelXform = transform.GetChild(1);
            var oldPos = enemyModelXform.localPosition;
            oldPos.z = 0f;
            var oldRotation = enemyModelXform.localRotation;
            var oldScale = enemyModelXform.localScale;
            Destroy(enemyModelXform.gameObject);
            var newModel = Instantiate(TwoPlayerModel, transform);
            newModel.transform.localScale = oldScale;
            newModel.transform.localRotation = oldRotation;
            newModel.transform.localPosition = oldPos;

            //add trail
            var trail = Instantiate(PlayerTwoTrail, transform);
            trail.transform.localPosition = new Vector3(0f, -0.3f, 0f);

            //add wheel turning 
            newModel.transform.GetChild(2).gameObject.AddComponent<WheelTurn>();

        }
        Destroy(this);
    }
}
