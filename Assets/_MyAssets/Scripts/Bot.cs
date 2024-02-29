using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bot : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject listeWaypoint;
    [SerializeField] private GameObject listeOptiWaypoint;
    [SerializeField] private GameObject target2;
    [SerializeField] private LayerMask _layersToHit;
    [SerializeField] private float difficulty = 1;

    private List<GameObject> waypoints = new List<GameObject>();
    private List<GameObject> optiWaypoints = new List<GameObject>();
    private int passedtargets = 0;
    [SerializeField] private GameObject target;
    public Plane Plane
    {
        private set;
        get;
    }

    private void Start()
    {
        foreach (Transform child in listeWaypoint.transform)
        {
            waypoints.Add(child.gameObject);
        }
        foreach (Transform child in listeOptiWaypoint.transform)
        {
            optiWaypoints.Add(child.gameObject);
        }
        target.transform.position = Vector3.Lerp(waypoints[passedtargets].transform.position, optiWaypoints[passedtargets].transform.position, difficulty);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Magnitude(transform.position - target.transform.position) < 1f)
        {
            passedtargets += 1;
            target.transform.position = Vector3.Lerp(waypoints[passedtargets].transform.position, optiWaypoints[passedtargets].transform.position, difficulty);
        }

        Plane = new Plane(transform.up, transform.position);
        Vector3 direction = Plane.ClosestPointOnPlane(target.transform.position) - transform.position;
        //target2.transform.position = direction;

        float angle = Vector3.SignedAngle(transform.forward, direction, transform.up);
        Debug.Log(angle);
        if (angle > -(7 * gameObject.GetComponent<Rigidbody>().velocity.magnitude) && angle < (7 * gameObject.GetComponent<Rigidbody>().velocity.magnitude))
        {
            SpaceShip.Instance.Forward();
        }
        if (angle > (7 * gameObject.GetComponent<Rigidbody>().velocity.magnitude))
        {
            SpaceShip.Instance.AirBrake(false);
            transform.Rotate(0f, 70f * Time.deltaTime, 0f);
        }
        else if (angle < (-7 * gameObject.GetComponent<Rigidbody>().velocity.magnitude))
        {
            SpaceShip.Instance.AirBrake(true);
            transform.Rotate(0f, -70f * Time.deltaTime, 0f);
        }
        else if (angle > 0.1f)
        {
            transform.Rotate(0f, 40f * Time.deltaTime, 0f);
        }
        else if (angle < -0.1f)
        {
            transform.Rotate(0f, -40f * Time.deltaTime, 0f);
        }
    }
}