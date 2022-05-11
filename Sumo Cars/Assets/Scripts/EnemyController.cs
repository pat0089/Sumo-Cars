using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    private CarBehaviour _carBehaviour;
    private ArenaBehaviour _arenaBehaviour;

    public GameObject curTarget = null;

    private float _acceleration = 0.5f;

    private float _turningFactor = .1f;

    private float time_to_next_target = 0f;

    private void Awake() {
        _carBehaviour = GetComponent<CarBehaviour>();
    }

    private void Start() {
        _arenaBehaviour = GameManager.Instance.Arena;
        _carBehaviour.acceleration -= 2;
        time_to_next_target = Random.Range(5.0f, 10.0f);
    }

    private void FixedUpdate() {
        if (curTarget != null) {
            float angleBetween = Vector3.SignedAngle(transform.up, curTarget.transform.position - gameObject.transform.position, -Vector3.forward);

            //Debug.Log("Angle: " + angleBetween);
            if (Mathf.Abs(angleBetween) > 90) {
                _acceleration = -1;
                angleBetween = Mathf.Abs(angleBetween) - 90;
            } else {
                _acceleration = 1;
            }

            if (_carBehaviour.hasPower == true) {
                _carBehaviour.TriggerPower();
            }

            float turningdir = 0;
            if (Mathf.Abs(angleBetween) > 10) {
                turningdir = Mathf.Sign(angleBetween);
            }

            _carBehaviour.SetAcceleratePercent(_acceleration);
            _carBehaviour.SetTurningPercent(turningdir);
        } else {
            GetNextTarget();
        }

        TimerNextTarget();
    }

    private void TimerNextTarget() { // TODO: Make into yield
        time_to_next_target -= Time.deltaTime;
        if (time_to_next_target <= 0) {
            time_to_next_target = Random.Range(5.0f, 15.0f);
            GetNextTarget();
        }
    }

    private void OnDrawGizmos() {
    }

    public void GetNextTarget() {
        List<CarBehaviour> carsCopy = new List<CarBehaviour>(GameManager.Instance.CarsList);
        carsCopy.Remove(_carBehaviour); // Remove our own car behaviour so we dont target ourselves
        if (carsCopy.Count == 0) {
            return;
        }
        curTarget = carsCopy[Random.Range(0, carsCopy.Count)].gameObject;
        Debug.Log("[" + gameObject.name + "]: " + curTarget);
    }
}