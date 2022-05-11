using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpinner : MonoBehaviour
{

    public float AngularVelocity = 30.0f;

    public BoundLineController BoundLine = null;

    void Start()
    {
        Debug.Assert(BoundLine != null);
    } 

    // Update is called once per frame
    void Update()
    {
        BoundLine.First.transform.RotateAround(BoundLine.transform.position, Vector3.forward, AngularVelocity * Time.deltaTime);
        BoundLine.Last.transform.RotateAround(BoundLine.transform.position, Vector3.forward, AngularVelocity * Time.deltaTime);
    }
}
