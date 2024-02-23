using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bot : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject listeWaypoint;

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
        //GameObject[] allChildren = new GameObject[listeWaypoint.transform.childCount];
        foreach (Transform child in listeWaypoint.transform)
        {
            waypoints.Add(child.gameObject);
        }
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