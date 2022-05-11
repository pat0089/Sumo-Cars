using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpDataHolder : MonoBehaviour {
    // Start is called before the first frame update
    public int typeOfPower = 0;
    private bool used = false;

    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if (used == true) {
            Destroy(transform.gameObject);
        }
    }

    public int getPower() {
        used = true;
        return typeOfPower;
    }
}