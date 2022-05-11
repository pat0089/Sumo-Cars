using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundLineEndController : MonoBehaviour
{

    public GameObject BoundObject = null;

    void Start()
    {
        Debug.Assert(BoundObject != null);
    }
    
    public Vector3 GetPos() {
        return BoundObject.transform.position;
    }

    public void SetPos(Vector3 newPos)
    {
        BoundObject.transform.position = newPos;
    }
}
