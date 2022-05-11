using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTurn : MonoBehaviour
{
    private CarBehaviour _carBehaviour;
    private Vector3 rotation = new Vector3(0, 0, 90);
    private float originalZ = 90;
    // Start is called before the first frame update
    void Awake()
    {
        _carBehaviour = gameObject.GetComponentInParent<CarBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = _carBehaviour.gameObject.transform.rotation * Quaternion.AngleAxis(90f - 0.7f * _carBehaviour.GetTurningDegree(), -Vector3.forward);
    }
}
