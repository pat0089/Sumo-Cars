using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PowerupBehaviour : MonoBehaviour {

    public PowerupType type;

    private void Awake() {
    }

    void Start() {
    }

    void Update() {
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Car")) {
            ApplyPowerup(other.gameObject, type);
        }
    }

    public static void ApplyPowerup(GameObject gameObject, PowerupType powerupType) {
        switch (powerupType) {
            case PowerupType.Push:

                break;
            case PowerupType.Pull:
                break;
            case PowerupType.Stop:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(powerupType), powerupType, null);
        }
    }

    public static GameObject CreatePowerup(PowerupType type) {
        GameObject powerupObject = Instantiate(GameManager.Instance.PowerupPrefab);
        PowerupBehaviour pub = powerupObject.GetComponent<PowerupBehaviour>();
        pub.type = type;
        return powerupObject;
    }

    public enum PowerupType {
        Push,
        Pull,
        Stop,
    }
}