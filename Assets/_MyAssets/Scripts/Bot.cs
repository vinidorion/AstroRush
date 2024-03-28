using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Bot : MonoBehaviour
{
/* -- NE PAS SUPPRIMER

	private float testcooldown = 0f;

	void Awake()
	{
		_spaceship = GetComponent<SpaceShip>();
		if(_spaceship == null) {
			Debug.Log(this.gameObject + " N'A PAS DE SPACESHIP");
		}
	}

	void Update()
	{
		//_spaceship.Forward();
		if(testcooldown < Time.time) {
			//Debug.Log("angle: " + Quaternion.Angle(transform.rotation, Quaternion.LookRotation(GetNextPos() - transform.position, GetGravDir())));
			testcooldown = Time.time + 1f;
		}
	}

	

	// get la direction de la gravité
	private Vector3 GetGravDir()
	{
		return -_spaceship.GetVecGrav(); // pour le moment
	}
*/

    // Start is called before the first frame update
    [SerializeField] private GameObject listeWaypoint;
    [SerializeField] private GameObject listeOptiWaypoint;
    [SerializeField] private GameObject listeBots;
    [SerializeField] private GameObject listeTargets;
    [SerializeField] private LayerMask _layersToHit;
    [SerializeField] private float difficulty1 = 1;
    [SerializeField] private float difficulty2 = 1;
    [SerializeField] private float difficulty3 = 0.5f;
    [SerializeField] private float difficulty4 = 0;
    [SerializeField] private float difficulty5 = 0;
    private SpaceShip _spaceship;

    private List<GameObject> waypoints = new List<GameObject>();
    private List<GameObject> optiWaypoints = new List<GameObject>();
    private List<GameObject> Bots = new List<GameObject>();
    private List<GameObject> targets = new List<GameObject>();
    private List<Vector3> directionRight = new List<Vector3>();
    private List<int> passedTargets = new List<int>();
    private List<float> difficulty = new List<float>();
    [SerializeField] private float accelbot = 0;
    private float targetspeed;

    public Plane Plane
    {
        private set;
        get;
    }

    private void Awake()
    {
        _layersToHit = 1 << LayerMask.NameToLayer("track");
    }

    private void Start()
    {
        difficulty.Add(difficulty1);
        difficulty.Add(difficulty2);
        difficulty.Add(difficulty3);
        difficulty.Add(difficulty4);
        difficulty.Add(difficulty5);
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
            targets[i].transform.position = Vector3.Lerp(waypoints[passedTargets[i]].transform.position,
                optiWaypoints[passedTargets[i]].transform.position, difficulty[i]);
            directionRight[i] = Bots[i].transform.right;
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        
        // boucle qui fait les actions de tout les bots 1 par 1
        for (int i = 0; i < Bots.Count; i++)
        {
            _spaceship = Bots[i].GetComponent<SpaceShip>();

            targetspeed = 15;
            //si le bot est assez proche de sa target on set sa target au prochain waypoint
            if (Vector3.Magnitude(Bots[i].transform.position - targets[i].transform.position) < 1f)
            {
                passedTargets[i] += 1;
                targets[i].transform.position = Vector3.Lerp(waypoints[passedTargets[i]].transform.position,
                    optiWaypoints[passedTargets[i]].transform.position, difficulty[i]);
            }
            Plane = new Plane(Bots[i].transform.up, Bots[i].transform.position);
            Vector3 direction = Plane.ClosestPointOnPlane(targets[i].transform.position) - Bots[i].transform.position;
            Vector3 direction2nd = Plane.ClosestPointOnPlane(Vector3.Lerp(waypoints[passedTargets[i] + 1].transform.position,
                optiWaypoints[passedTargets[i] + 1].transform.position, difficulty[i])) - Bots[i].transform.position;

            float angle = Vector3.SignedAngle(Bots[i].transform.forward, direction, Bots[i].transform.up);
            float angleVel = Vector3.SignedAngle(Bots[i].GetComponent<Rigidbody>().velocity, direction, Bots[i].transform.up);
            float angle2ndP = Vector3.SignedAngle(Bots[i].GetComponent<Rigidbody>().velocity, direction2nd, Bots[i].transform.up);

            Debug.DrawLine(_spaceship.transform.position, targets[i].transform.position, Color.green, Time.fixedDeltaTime);
            Debug.DrawLine(_spaceship.transform.position, Vector3.Lerp(waypoints[passedTargets[i] + 1].transform.position,
                    optiWaypoints[passedTargets[i] + 1].transform.position, difficulty[i]), Color.blue, Time.fixedDeltaTime);
            Debug.DrawLine(_spaceship.transform.position, _spaceship.transform.position + Bots[i].GetComponent<Rigidbody>().velocity, Color.red, Time.fixedDeltaTime);

            if (angle2ndP > 5 || angle2ndP < -5)
            {
                targetspeed = 15 - ((Mathf.Abs(angle2ndP) / 180) * 15);
            }

            BotMove(i, angleVel);
            if (i >= 3)
            {
                Debug.Log(angle);
            }
            BotTurn(i, angle);
            SetPitch(i);
        }
    }

    void BotMove(int i, float angleVel)
    {
        if (Bots[i].GetComponent<Rigidbody>().velocity.magnitude <= targetspeed)
        {
            Bots[i].GetComponent<SpaceShip>().Forward();
        }
        else if (Bots[i].GetComponent<Rigidbody>().velocity.magnitude > targetspeed)
        {
            Bots[i].GetComponent<SpaceShip>().backward();
        }
        if (angleVel > 7)
        {
            Bots[i].GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(20f * Time.deltaTime, Bots[i].transform.up) * Bots[i].GetComponent<Rigidbody>().velocity;
        }
        if (angleVel < -7)
        {
            Bots[i].GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(-20f * Time.deltaTime, Bots[i].transform.up) * Bots[i].GetComponent<Rigidbody>().velocity;
        }
    }

    void BotTurn(int i, float angle)
    {
        if (angle > 50 / Bots[i].GetComponent<Rigidbody>().velocity.magnitude && Bots[i].GetComponent<Rigidbody>().velocity.magnitude >= 1)
        {
            Debug.Log("Turn");
            Bots[i].GetComponent<SpaceShip>().AirBrake(false);
            Bots[i].transform.Rotate(0f, 90f * Time.deltaTime, 0f);
            directionRight[i] = Quaternion.AngleAxis(90f * Time.deltaTime, Bots[i].transform.up) * directionRight[i];
        }
        if (angle < -50 / Bots[i].GetComponent<Rigidbody>().velocity.magnitude && Bots[i].GetComponent<Rigidbody>().velocity.magnitude >= 1)
        {
            Debug.Log("Turn");
            Bots[i].GetComponent<SpaceShip>().AirBrake(true);
            Bots[i].transform.Rotate(0f, -90f * Time.deltaTime, 0f);
            directionRight[i] = Quaternion.AngleAxis(-90f * Time.deltaTime, Bots[i].transform.up) * directionRight[i];
        }
        else if (angle > 0.1f)
        {
            //_spaceship.Turn(true);
            Bots[i].transform.Rotate(0f, 60f * Time.deltaTime, 0f);
            directionRight[i] = Quaternion.AngleAxis(60f * Time.deltaTime, Bots[i].transform.up) * directionRight[i];
        }
        else if (angle < -0.1f)
        {
            //_spaceship.Turn(false);
            Bots[i].transform.Rotate(0f, -60f * Time.deltaTime, 0f);
            directionRight[i] = Quaternion.AngleAxis(-60f * Time.deltaTime, Bots[i].transform.up) * directionRight[i];
        }
    }

    void SetPitch(int i)
    {
        Ray ray = new Ray(Bots[i].transform.position, -Bots[i].transform.up);
        Physics.Raycast(ray, out RaycastHit hit, 1, _layersToHit, QueryTriggerInteraction.Ignore);

        if (hit.normal != Vector3.up)
        {
            Bots[i].transform.rotation = Quaternion.RotateTowards(Bots[i].transform.rotation, Quaternion.LookRotation(Vector3.Cross(hit.normal, directionRight[i]), hit.normal),
            90 * Time.deltaTime);
        }
    }

    // get la position du next waypoint
    private Vector3 GetNextPos()
    {
        return WaypointManager.Instance.GetWaypointPos(_spaceship.GetWaypoint() + 1);
    }
}