using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Bot : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject listeWaypoint;
    [SerializeField] private GameObject listeOptiWaypoint;
    [SerializeField] private GameObject listeBots;
    [SerializeField] private GameObject listeTargets;
    [SerializeField] private LayerMask _layersToHit;
    [SerializeField] private float difficulty = 1;

    private List<GameObject> waypoints = new List<GameObject>();
    private List<GameObject> optiWaypoints = new List<GameObject>();
    private List<GameObject> Bots = new List<GameObject>();
    private List<GameObject> targets = new List<GameObject>();
    private List<Vector3> directionRight = new List<Vector3>();
    private List<int> passedTargets = new List<int>();

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
        foreach (Transform child in listeBots.transform)
        {
            Bots.Add(child.gameObject);
            directionRight.Add(child.gameObject.transform.right);
            passedTargets.Add(0);
        }
        foreach (Transform child in listeTargets.transform)
        {
            targets.Add(child.gameObject);
        }
        for (int i = 0; i < targets.Count; i++)
        {
            targets[i].transform.position = Vector3.Lerp(waypoints[passedTargets[i]].transform.position, optiWaypoints[passedTargets[i]].transform.position, difficulty);
            directionRight[i] = Bots[i].transform.right;
        }
        
    }

    // Update is called once per frame
    private void Update()
    {

        for (int i = 0; i < Bots.Count; i++)
        {
            if (Vector3.Magnitude(Bots[i].transform.position - targets[i].transform.position) < 1f)
            {
                passedTargets[i] += 1;
                //Debug.Log(passedTargets[i]);
                targets[i].transform.position = Vector3.Lerp(waypoints[passedTargets[i]].transform.position, optiWaypoints[passedTargets[i]].transform.position, difficulty);
            }
            Plane = new Plane(Bots[i].transform.up, Bots[i].transform.position);
            Vector3 direction = Plane.ClosestPointOnPlane(targets[i].transform.position) - Bots[i].transform.position;

            float angle = Vector3.SignedAngle(Bots[i].transform.forward, direction, Bots[i].transform.up);

            if (Bots[i].GetComponent<Rigidbody>().velocity.magnitude <= 5)
            {
                Bots[i].GetComponent<Rigidbody>().AddForce(Bots[i].transform.forward * 1);
            }

            //SpaceShip.Instance.Forward();

            if (angle > 7)
            {
                SpaceShip.Instance.AirBrake(false);
                Bots[i].transform.Rotate(0f, 100f * Time.deltaTime, 0f);
                directionRight[i] = Quaternion.AngleAxis(100f * Time.deltaTime, Bots[i].transform.up) * directionRight[i];
            }
            if (angle < -7)
            {
                SpaceShip.Instance.AirBrake(true);
                Bots[i].transform.Rotate(0f, -100f * Time.deltaTime, 0f);
                directionRight[i] = Quaternion.AngleAxis(-100f * Time.deltaTime, Bots[i].transform.up) * directionRight[i];
            }
            else if (angle > 0.1f)
            {
                Bots[i].transform.Rotate(0f, 60f * Time.deltaTime, 0f);
                directionRight[i] = Quaternion.AngleAxis(60f * Time.deltaTime, Bots[i].transform.up) * directionRight[i];
            }
            else if (angle < -0.1f)
            {
                Bots[i].transform.Rotate(0f, -60f * Time.deltaTime, 0f);
                directionRight[i] = Quaternion.AngleAxis(-60f * Time.deltaTime, Bots[i].transform.up) * directionRight[i];
            }

            Ray ray = new Ray(Bots[i].transform.position, -Bots[i].transform.up);
            Physics.Raycast(ray, out RaycastHit hit, 1, _layersToHit, QueryTriggerInteraction.Ignore);

            if (hit.normal != Vector3.up)
            {
                Bots[i].transform.rotation = Quaternion.RotateTowards(Bots[i].transform.rotation, Quaternion.LookRotation(Vector3.Cross(hit.normal, directionRight[i]), hit.normal),
                90 * Time.deltaTime);
            }
        }
    }
}