using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHoleController : MonoBehaviour
{
    public GameObject Hole = null;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(Hole != null);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        var car = collision.gameObject.GetComponent<CarBehaviour>();
        if (car != null)
        {
            var vectorFromCenter = (car.transform.position - Hole.transform.position);
            var distanceFromCenter = vectorFromCenter.magnitude;
            if (distanceFromCenter < Hole.transform.localScale.x/2 * 0.8f)
            {
                //will edit to include destruction animation
                Destroy(car.gameObject);
            }
        }
    }


}
