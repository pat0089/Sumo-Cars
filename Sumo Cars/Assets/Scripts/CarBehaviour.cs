using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarBehaviour : MonoBehaviour {

    public float MaxVelocity = 30;
    public float MaxAnglerVelocity = 150;

    public float acceleration; // Meters/Second/Second
    public float turningSpeed; // Measured in degrees/second
    public float maxTurningDegree;
    public float driftFactor;

    public bool speedChanged = false; // For use in powerup controller
    public bool hasPower = false;
    public int powerUpType = 0; // Moving
    public int carNum = 0;

    public PowerupBehaviour.PowerupType? HeldPowerup = null;

    public float _engineForce = 0;
    private float _turningPercent = 0;
    private float _turningDegree = 0;
    private Vector2 _lastVelocity;
    private bool _skidding = false;
    private Coroutine _stopSkidding;

    private BoxCollider2D _collider2D;
    private Rigidbody2D _rigidbody2D;

    private GameManager _gameManager;

    private void Awake() {
        _collider2D = GetComponent<BoxCollider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _gameManager = FindObjectOfType<GameManager>();
        Debug.Assert(_gameManager != null);
    }

    void Start() {
        _rigidbody2D.inertia = 3;
    }

    private void FixedUpdate() {
        _turningDegree += turningSpeed * _turningPercent;
        if (_turningDegree > 0) {
            _turningDegree -= 1f;
        } else {
            _turningDegree += 1f;
        }


        _turningDegree = Mathf.Clamp(_turningDegree, -maxTurningDegree, maxTurningDegree);

        float carLength = 6;
        float turningRadius = carLength / Mathf.Sin((_turningDegree) * Mathf.Deg2Rad);

        float angularVelocityRads = GetForwardVelocity().magnitude / turningRadius;

        _rigidbody2D.AddForce(transform.up * _engineForce, ForceMode2D.Force);

        float tireAngularVelocity = angularVelocityRads * _rigidbody2D.inertia * Mathf.Rad2Deg;

        _rigidbody2D.angularVelocity = Mathf.Lerp(_rigidbody2D.angularVelocity, tireAngularVelocity, 0.05f);

        if (!_skidding) {
            DampenDrift();
        }

        _rigidbody2D.velocity = Vector2.ClampMagnitude(_rigidbody2D.velocity, MaxVelocity);
        _rigidbody2D.angularVelocity = Mathf.Clamp(_rigidbody2D.angularVelocity, -MaxAnglerVelocity, MaxAnglerVelocity);

        _lastVelocity = _rigidbody2D.velocity;
    }

    private void DampenDrift() {
        Vector2 carVelocityForward = GetForwardVelocity();
        Vector2 carVelocityRight = GetRightVelocity();

        _rigidbody2D.velocity = carVelocityForward + (carVelocityRight * driftFactor);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        // Debug.Log($"I {name} just got hit by {other.gameObject.name}");
        CarBehaviour otherCarBehaviour = other.gameObject.GetComponent<CarBehaviour>();
        if (otherCarBehaviour != null) {
            if (_lastVelocity.magnitude < otherCarBehaviour._lastVelocity.magnitude) {
                StartSkidding(1);
            }

            _rigidbody2D.AddForce(other.contacts[0].normal * 20, ForceMode2D.Impulse);
        }
    }

    public void StartSkidding(float timeout) {
        _skidding = true;
        if (_stopSkidding != null) StopCoroutine(_stopSkidding); // Reset skidding coroutine
        _stopSkidding = StartCoroutine(ResetSkid(timeout));
    }

    IEnumerator ResetSkid(float timeout) {
        yield return new WaitForSeconds(timeout);
        _skidding = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("PowerUp")) {
            powerUpType = other.GetComponent<PowerUpDataHolder>().getPower();
            hasPower = true;
        }
    }

    public Vector2 GetForwardVelocity() {
        return transform.up * Vector2.Dot(_rigidbody2D.velocity, transform.up);
    }

    public Vector2 GetRightVelocity() {
        return transform.right * Vector2.Dot(_rigidbody2D.velocity, transform.right);
    }

    public Vector2 GetCarFrontPoint() {
        Transform carTransform = transform;
        float carHeight = carTransform.localScale.y / 2 * _collider2D.size.y;
        return carTransform.position + (carTransform.up * carHeight);
    }

    public void TriggerPower() {
        if (hasPower == true) {
            hasPower = false;
            //Debug.Log("before powerup");
            _gameManager.Powerup(gameObject);
        }
    }

    public float GetTurningDegree() {
        return _turningDegree;
    }

    #region CarControls

    /**
     * -1 for full reverse, 1 for full forward
     */
    public void SetAcceleratePercent(float powerPercent) {
        _engineForce = powerPercent * acceleration;
    }

    /**
     * -1 to turn left, 1 to turn right
     */
    public void SetTurningPercent(float turningPercent) {
        _turningPercent = -turningPercent;
    }

    public void TriggerPowerup() {
        if (HeldPowerup == null) {
            return;
        }
    }

    #endregion

    #region Debug

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if (Application.isPlaying) {
            Gizmos.color = Color.red;
            Vector3 frontOfCar = GetCarFrontPoint();
            Vector3 direction = Quaternion.Euler(0, 0, _turningDegree) * transform.up;
            Gizmos.DrawLine(frontOfCar, frontOfCar + direction * 10);
        }

        GUIStyle debugStyle = new GUIStyle();
        if (_skidding) {
            debugStyle.normal.textColor = Color.green;
        } else {
            debugStyle.normal.textColor = Color.red;
        }

        Handles.Label(transform.position + new Vector3(10, 10), "Skidding: " + _skidding, debugStyle);
    }
#endif

    #endregion

}