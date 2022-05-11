using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BoundLineController : MonoBehaviour
{
    public GameObject BoundObject = null;

    public BoundLineEndController First = null;

    public BoundLineEndController Last = null;

    private Vector3 _vector;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(BoundObject != null);
    }

    // Update is called once per frame
    void Update()
    {
        if (First != null && Last != null)
        {
            UpdateDirectionVector();
            UpdateQuad();
        }
    }

    private void UpdateDirectionVector() 
    {
        _vector = Last.GetPos() - First.GetPos();
    }

    private void UpdateQuad()
    {
        BoundObject.transform.localScale = new Vector3(5f, _vector.magnitude/2, 5f);
        BoundObject.transform.position = First.GetPos() + 0.5f * _vector + new Vector3(0f, 0f, 0.5f);
        BoundObject.transform.localRotation = Quaternion.LookRotation(Vector3.forward, Quaternion.Euler(0f, 0f, 0f) * _vector);
    }

    public BoundLineEndController GetFirst() {
        return First;
    }

    public void SetFirst(BoundLineEndController point)
    {
        First = point;
    }

    public BoundLineEndController GetLast()
    {
        return Last;
    }

    public void SetLast(BoundLineEndController point)
    {
        Last = point;
    }
}
