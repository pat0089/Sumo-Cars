using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    private CarBehaviour _carBehaviour;
    private ArenaBehaviour _arenaBehaviour;

    public Key accelerateKey = Key.W;
    public Key reverseKey = Key.S;
    public Key leftKey = Key.A;
    public Key rightKey = Key.D;
    public Key powerUp = Key.F;

    private void Awake() {
        _carBehaviour = GetComponent<CarBehaviour>();
    }

    private void Start() {
        _arenaBehaviour = GameManager.Instance.Arena;
    }

    private float damping = 0.1f;

    private void Update() {
        float forward = 0;
        float horizontal = 0;
        if (Keyboard.current[accelerateKey].isPressed) {
            forward += 1;
        }

        if (Keyboard.current[reverseKey].isPressed) {
            forward += -1;
        }

        if (Keyboard.current[rightKey].isPressed) {
            horizontal += 1;
        }

        if (Keyboard.current[leftKey].isPressed) {
            horizontal += -1;
        }

        if (_carBehaviour.GetForwardVelocity().sqrMagnitude < 0) {
            horizontal *= -1;
        }

        if (Keyboard.current[powerUp].isPressed) {
            _carBehaviour.TriggerPower();
        }

        _carBehaviour.SetAcceleratePercent(forward);
        _carBehaviour.SetTurningPercent(horizontal);
    }
}