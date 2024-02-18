using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bot : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject waypoint1;
    [SerializeField] private GameObject waypoint2;
    [SerializeField] private GameObject waypoint3;
    [SerializeField] private GameObject waypoint4;
    [SerializeField] private GameObject waypoint5;
    [SerializeField] private GameObject waypoint6;
    [SerializeField] private GameObject waypoint7;
    [SerializeField] private GameObject waypoint8;
    [SerializeField] private GameObject waypoint9;
    [SerializeField] private GameObject waypoint10;
    [SerializeField] private GameObject waypoint11;

    [SerializeField] private GameObject target2;

    private List<GameObject> waypoints = new List<GameObject>();
    private int passedtargets = 0;
    private GameObject target;
    public Plane Plane
    {
        private set;
        get;
    }

    private void Start()
    {
        waypoints.Add(waypoint1);
        waypoints.Add(waypoint2);
        waypoints.Add(waypoint3);
        waypoints.Add(waypoint4);
        waypoints.Add(waypoint5);
        waypoints.Add(waypoint6);
        waypoints.Add(waypoint7);
        waypoints.Add(waypoint8);
        waypoints.Add(waypoint9);
        waypoints.Add(waypoint10);
        waypoints.Add(waypoint11);

    }

    // Update is called once per frame
    void Update()
    {
        target = waypoints[passedtargets];
        if (Vector3.Magnitude(transform.position - target.transform.position) < 1f)
        {
            passedtargets += 1;
        }

        Plane = new Plane(transform.up, transform.position);
        Vector3 direction = Plane.ClosestPointOnPlane(target.transform.position) - transform.position;
        //target2.transform.position = direction;

        float angle = Vector3.SignedAngle(transform.forward, direction, transform.up);
        Debug.Log(angle);
        SpaceShip.Instance.Forward();
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