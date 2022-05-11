using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaBehaviour : MonoBehaviour {

    public GameManager _gameManager;

    private bool stopPowerSpawn = false;
    //public bool PowerupSpawns = false;

    private GameObject FrictionZone;

    private GameObject Faux3D;
    public float Thickness = 10.0f;


    public float BaseDrag = 0.3f;
    public float DragMultiplier = 2f;
    public float RangeDivisor = 8f;

    public float ShrinkCooldown = 20.0f;
    private float _shrinkCooldownCounter = 0.0f;
    private bool _shrinking = false;
    public float SecondsToShrink = 10f;
    public float ShrinkFactorPerSecond = 0.5f;
    private float _initialSizeFull;
    public float FinalSize = 0.1f;
    private float _finalSizeFull;

    void Awake() {
        //ObjectList = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start() {
        _gameManager = GameManager.Instance;

        GetComponent<Renderer>().enabled = false;

        FrictionZone = Instantiate(gameObject, Vector3.zero, transform.rotation);

        Destroy(FrictionZone.GetComponent<ArenaBehaviour>());
        FrictionZone.GetComponent<Renderer>().enabled = false;
        FrictionZone.transform.localScale *= 0.5f;
        FrictionZone.transform.parent = transform;

        Faux3D = Instantiate(Resources.Load("Prefabs/3DShapedArena"), Vector3.zero, transform.rotation) as GameObject;
        var mesh = GetMesh();
        mesh.Optimize();
        mesh.RecalculateNormals();
        Faux3D.GetComponent<MeshFilter>().mesh = mesh;
        Faux3D.transform.parent = transform;
        Faux3D.transform.localScale = Vector3.one;

        _initialSizeFull = transform.localScale.x;
        _finalSizeFull = FinalSize * _initialSizeFull;
    }

    private Mesh GetMesh() {
        //get collider bounds points and center
        Vector3 center;
        var points = GetColliderPoints(out center);

        //generate more points
        //top
        var top = AddZVal(points, 0.0f);
        var topCenter = new Vector3(center.x, center.y, 0.0f);
        top.Add(topCenter);

        //bottom
        var bottom = AddZVal(points, Thickness);
        var bottomCenter = new Vector3(center.x, center.y, Thickness);
        bottom.Add(bottomCenter);

        //concatenate lists for final vertex list
        var vertexList = new List<Vector3>();
        foreach (var point in top) {
            vertexList.Add(point);
        }

        foreach (var point in bottom) {
            vertexList.Add(point);
        }

        //triangulation section
        //triangulate top and bottom shapes
        var topIndices = TriangulateShape(top, topCenter);
        var bottomIndices = TriangulateShape(bottom, bottomCenter, true);

        //remove the centers for side triangulation processing
        top.Remove(topCenter);
        bottom.Remove(bottomCenter);

        //triangulate sides
        var sideIndices = TriangulateSides(top, bottom);

        //add sides as separate vertices to get unity to potentially recalculate them correctly...?
        //i also have something in mind if this doesnt work
        foreach (var p in top)
        {
            vertexList.Add(p);
        }
        foreach (var p in bottom)
        {
            vertexList.Add(p);
        }


        //concatenate lists for final triangulated indices
        var indicesList = new List<int>();
        foreach (var index in topIndices) {
            indicesList.Add(index);
        }

        foreach (var index in bottomIndices) {
            indicesList.Add(index + top.Count + 1);
        }

        foreach (var index in sideIndices) {
            indicesList.Add(index + top.Count + bottom.Count + 2);
        }



        Mesh m = new Mesh();

        m.vertices = vertexList.ToArray();
        m.triangles = indicesList.ToArray();
        return m;
    }

    private List<int> TriangulateShape(List<Vector3> toTriangulate, Vector3 center, bool inverted = false) {
        var centerIndex = toTriangulate.IndexOf(center);
        var toReturn = new List<int>();
        for (int i = 0; i < toTriangulate.Count; i++) {
            if (!inverted) {
                toReturn.Add(i);
                toReturn.Add((i + 1) % toTriangulate.Count);
                toReturn.Add(centerIndex);
            } else {
                toReturn.Add(i);
                toReturn.Add(centerIndex);
                toReturn.Add((i + 1) % toTriangulate.Count);
            }
        }

        return toReturn;
    }

    private List<int> TriangulateSides(List<Vector3> top, List<Vector3> bottom) {
        var toReturn = new List<int>();

        var sides = new List<Vector3>();

        foreach (var p in top)
        {
            sides.Add(p);
        }
        foreach (var p in bottom)
        {
            sides.Add(p);
        }


        var topIndices = new List<int>();
        for (int i = 0; i < sides.Count; i++) {
            foreach (var point in top) {
                if (point == sides[i]) {
                    topIndices.Add(i);
                }
            }
        }

        var bottomIndices = new List<int>();
        for (int i = 0; i < sides.Count; i++) {
            foreach (var point in bottom) {
                if (point == sides[i]) {
                    bottomIndices.Add(i);
                }
            }
        }

        for (int i = 0; i < topIndices.Count; i++) {
            //first triangle
            toReturn.Add(bottomIndices[(i + 1) % topIndices.Count]);
            toReturn.Add(topIndices[i]);
            toReturn.Add(bottomIndices[i]);

            //second triangle
            toReturn.Add(topIndices[i]);
            toReturn.Add(bottomIndices[(i + 1) % topIndices.Count]);
            toReturn.Add(topIndices[(i + 1) % topIndices.Count]);
        }

        return toReturn;
    }

    private List<Vector3> AddZVal(List<Vector2> values, float zval) {
        var toReturn = new List<Vector3>();
        foreach (var point in values) {
            toReturn.Add(new Vector3(point.x, point.y, zval));
        }

        return toReturn;
    }

    private List<Vector2> GetColliderPoints(out Vector3 center) {
        var collider = transform.GetComponent<PolygonCollider2D>();
        var points = new List<Vector2>(collider.points);
        center = collider.bounds.center;
        return points;
    }

    // Update is called once per frame
    void Update() {
        _shrinkCooldownCounter += Time.deltaTime;
        if (_shrinkCooldownCounter > ShrinkCooldown) {
            _shrinking = true;
        }

        if (_shrinking && transform.localScale.x > _finalSizeFull) {
            Vector3 newScale = transform.localScale;
            transform.localScale -= (ShrinkFactorPerSecond * (Time.deltaTime / SecondsToShrink) * Vector3.one) * _initialSizeFull;
        }
    }


    private void OnTriggerExit2D(Collider2D other) {
        CarBehaviour carBehaviour = other.gameObject.GetComponent<CarBehaviour>();
        //only care about cars for bounds checking
        if (carBehaviour != null) {
            //will edit to include destruction animation
            _gameManager.DestroyCar(carBehaviour);
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        var car = other.gameObject.GetComponent<CarBehaviour>();
        //only care about cars for bounds checking
        if (car != null) {
            var collider = transform.GetComponent<Collider2D>();
            var rb = car.GetComponent<Rigidbody2D>();
            if (rb != null && collider != null) {
                //do drag calculation: base coefficient of drag on distance from the edge of the map
                //first calculate distance from nearest edge of the FrictionZone (set to 0.5 the scale of the map at the same location as 
                //then set the drag based off ratio from the point to the center
                var closestPoint = FrictionZone.GetComponent<Collider2D>().ClosestPoint(car.transform.position);
                var distanceFromEdge = ((Vector2) car.transform.position - closestPoint).magnitude - car.transform.localScale.y;
                var frictionRange = (collider.bounds.size.x + collider.bounds.size.y) / RangeDivisor;
                if (distanceFromEdge <= frictionRange) {
                    var ratio = distanceFromEdge / frictionRange;
                    var calculated = BaseDrag + DragMultiplier * ratio;
                    if (calculated < BaseDrag) {
                        rb.drag = BaseDrag;
                    } else {
                        rb.drag = calculated;
                    }
                }
            }
        }
    }

    public Vector2 GetPointInArena() {
        //used to generate a point that is in the collider of the arena
        //first, generate point in the total bounds,
        //then check to see if it is within the collider bounds
        //if so, break the loop, and spawn the 
        //if not loop back to 1
        var collider = transform.GetComponent<Collider2D>();
        var bounds = collider.bounds;
        var randomPointInBounds = new Vector2(Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y));
        do {
            if (collider.OverlapPoint(randomPointInBounds)) {
                return randomPointInBounds;
            }

            randomPointInBounds = new Vector2(Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y));
        } while (!collider.OverlapPoint(randomPointInBounds));

        return Vector2.positiveInfinity;
    }

    public void SpawnPowerup() {
        var point = GetPointInArena();
        while (point.Equals(Vector2.positiveInfinity)) {
            point = GetPointInArena();
        }

        //instantiate powerup
        int rand = Random.Range(0, 6);
        if (rand == 0) {
            var newPowerup = Instantiate(Resources.Load("Prefabs/Powerups/PowerupPush")) as GameObject;
            newPowerup.transform.position = point;
        } else if (rand == 1) {
            var newPowerup = Instantiate(Resources.Load("Prefabs/Powerups/PowerupPull")) as GameObject;
            newPowerup.transform.position = point;
        } else if (rand == 2) {
            var newPowerup = Instantiate(Resources.Load("Prefabs/Powerups/PowerupSlow")) as GameObject;
            newPowerup.transform.position = point;
        } else if (rand == 3) {
            var newPowerup = Instantiate(Resources.Load("Prefabs/Powerups/PowerupFast")) as GameObject;
            newPowerup.transform.position = point;
        } else if (rand == 4) {
            var newPowerup = Instantiate(Resources.Load("Prefabs/Powerups/PowerupStop")) as GameObject;
            newPowerup.transform.position = point;
        } else if (rand == 5) {
            var newPowerup = Instantiate(Resources.Load("Prefabs/Powerups/PowerupBoost")) as GameObject;
            newPowerup.transform.position = point;
        } //else if (rand == 6) {
            //var newPowerup = Instantiate(Resources.Load("Prefabs/Powerups/PowerupDrag")) as GameObject;
            //newPowerup.transform.position = point;
        //}
    }

    public void StartPowerupTimer() {
        stopPowerSpawn = false;
        StartCoroutine(PowerupTimer());
    }

    public void StopPowerupTimer() {
        stopPowerSpawn = true;
    }

    IEnumerator PowerupTimer() {
        float timeToNextSpawn = 5f; //Inital spawn time
        float minInterval = 5f;
        float maxInterval = 10f;
        // Infinitely looping currently
        while (timeToNextSpawn > 0f) {
            if (stopPowerSpawn == true) {
                break;
            }

            timeToNextSpawn -= Time.deltaTime;
            if (timeToNextSpawn < 0) {
                SpawnPowerup();
                timeToNextSpawn = Random.Range(minInterval, maxInterval);
            }

            yield return null;
        }
    }
}