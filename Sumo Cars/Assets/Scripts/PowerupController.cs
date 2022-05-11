using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupController : MonoBehaviour {
    //public int powerUpType = 0;
    //public bool powerUpGot = false;
    public GameManager _gameManager;

    private void Awake() {
    }

    private void Start() {
        _gameManager = GameManager.Instance;
    }

    void Update() {
    }

    public void chooseTrigger(GameObject car, GameObject start, int num) {
        //Debug.Log("within choose trigger");
        if (num == 0) {
            triggerPush(start, car);
        } else if (num == 1) {
            triggerPull(start, car);
        } else if (num == 2) {
            triggerSlow(car);
        } else if (num == 3) {
            triggerFast(start);
        } else if (num == 4) {
            triggerStop(car);
        } else if (num == 5) {
            triggerBoost(start);
        } else if (num == 6) {
            triggerDrag(car);
        }
    }

    public void triggerSlow(GameObject car) {
        if (car.GetComponent<CarBehaviour>().speedChanged == false) {
            triggerDrag(car);
            StartCoroutine(ChangeSpeed(car, car.GetComponent<CarBehaviour>().acceleration / 2, 5));
        }
    }

    public void triggerDrag(GameObject car) { //not used any more
        Vector2 currentSpeed = car.GetComponent<Rigidbody2D>().velocity;
        currentSpeed.x = currentSpeed.x / 2;
        currentSpeed.y = currentSpeed.y / 2;
        car.GetComponent<Rigidbody2D>().velocity = currentSpeed;
    }

    public void triggerFast(GameObject car) {
        if (car.GetComponent<CarBehaviour>().speedChanged == false) {
            StartCoroutine(ChangeSpeed(car, car.GetComponent<CarBehaviour>().acceleration * 2, 5));
        }
    }

    public void triggerBoost(GameObject car) {
        Vector2 currentSpeed = car.GetComponent<Rigidbody2D>().velocity;
        currentSpeed.x = currentSpeed.x * 2;
        currentSpeed.y = currentSpeed.y * 2;
        car.GetComponent<Rigidbody2D>().velocity = currentSpeed;
        ChangeTopSpeed(car, car.GetComponent<CarBehaviour>().MaxVelocity * 2, 1);
    }

    public void triggerStop(GameObject car) {
        car.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        car.GetComponent<CarBehaviour>()._engineForce = 0;
        car.GetComponent<CarBehaviour>().StartSkidding(2);
        ChangeSpeed(car, 0, 2);
    }

    public void triggerPush(GameObject originator, GameObject car) {
        Vector3 c = originator.transform.localPosition;
        Vector3 p = car.transform.localPosition;

        Vector3 direction = p - c;
        car.GetComponent<CarBehaviour>().StartSkidding(1);
        car.GetComponent<Rigidbody2D>().AddForce(direction.normalized * 30f, ForceMode2D.Impulse);
    }

    public void triggerPull(GameObject originator, GameObject car) {
        Vector3 c = originator.transform.localPosition;
        Vector3 p = car.transform.localPosition;

        Vector3 direction = p - c;
        car.GetComponent<CarBehaviour>().StartSkidding(1);
        car.GetComponent<Rigidbody2D>().AddForce(direction.normalized * -30f, ForceMode2D.Impulse);
    }

    IEnumerator ChangeSpeed(GameObject car, float speed, float timeInsert) {
        car.GetComponent<CarBehaviour>().speedChanged = true;
        var time = 0f;
        float carOrigAccel =  car.GetComponent<CarBehaviour>().acceleration;
        //Turn towards the side.
        while (time < timeInsert) {
            time += Time.deltaTime;
            car.GetComponent<CarBehaviour>().acceleration = speed;
            yield return null;
        }

        //Turn back to the starting position.
        while (time > 0) {
            time -= Time.deltaTime;
            car.GetComponent<CarBehaviour>().acceleration = carOrigAccel;
            yield return null;
        }

        car.GetComponent<CarBehaviour>().speedChanged = false;
    }

    IEnumerator ChangeTopSpeed(GameObject car, float speed, float timeInsert)
    {
        var time = 0f;
        float carOrigSpeed = car.GetComponent<CarBehaviour>().MaxVelocity;
        //Turn towards the side.
        while (time < timeInsert)
        {
            time += Time.deltaTime;
            car.GetComponent<CarBehaviour>().MaxVelocity = speed;
            yield return null;
        }

        //Turn back to the starting position.
        while (time > 0)
        {
            time -= Time.deltaTime;
            car.GetComponent<CarBehaviour>().MaxVelocity = carOrigSpeed;
            yield return null;
        }

    }
}