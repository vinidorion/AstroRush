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
    private Rigidbody _rb;
    protected float _agility;
    protected float _targetSpeed;
    protected List<float> _maxSpeed;

    private List<GameObject> waypoints = new List<GameObject>();
    private List<GameObject> optiWaypoints = new List<GameObject>();
    protected List<GameObject> Bots = new List<GameObject>();
    private List<GameObject> targets = new List<GameObject>();
    private List<Vector3> directionRight = new List<Vector3>();
    private List<int> passedTargets = new List<int>();
    public List<float> difficulty = new List<float>();
    [SerializeField] private float accelbot = 0;

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
        for (int i = 0; i < 4; i++)
        {
            _maxSpeed.Add(Bots[i].GetComponent<SpaceShip>().GetMaxSpeed());
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        
        // boucle qui fait les actions de tout les bots 1 par 1
        for (int i = 0; i < Bots.Count; i++)
        {
            _spaceship = Bots[i].GetComponent<SpaceShip>();
            _rb = _spaceship.GetComponent<Rigidbody>();
            _agility = _spaceship.GetAgility();
            _targetSpeed = _spaceship.GetMaxSpeed();

            //Debug.Log(_spaceship.GetMaxSpeed());
            //si le bot est assez proche de sa target on set sa target au prochain waypoint
            if (Vector3.Magnitude(_spaceship.transform.position - targets[i].transform.position) < 2f)
            {
                passedTargets[i] += 1;
                targets[i].transform.position = Vector3.Lerp(waypoints[passedTargets[i]].transform.position,
                    optiWaypoints[passedTargets[i]].transform.position, difficulty[i]);
                if (passedTargets[i] == 37)
                {
                    passedTargets[i] = 0;
                }
                _spaceship.SetWaypoint(passedTargets[i]);
            }
            Plane = new Plane(Bots[i].transform.up, Bots[i].transform.position);
            Vector3 direction = Plane.ClosestPointOnPlane(targets[i].transform.position) - Bots[i].transform.position;

            float angle = Vector3.SignedAngle(Bots[i].transform.forward, direction, Bots[i].transform.up);
            float angleVel = Vector3.SignedAngle(Bots[i].GetComponent<Rigidbody>().velocity, direction, Bots[i].transform.up);

            
            Debug.DrawLine(_spaceship.transform.position, targets[i].transform.position, Color.green, Time.fixedDeltaTime);
            /*
            Debug.DrawLine(_spaceship.transform.position, Vector3.Lerp(waypoints[passedTargets[i] + 1].transform.position,
                    optiWaypoints[passedTargets[i] + 1].transform.position, difficulty[i]), Color.blue, Time.fixedDeltaTime);
            Debug.DrawLine(_spaceship.transform.position, _spaceship.transform.position + Bots[i].GetComponent<Rigidbody>().velocity, Color.red, Time.fixedDeltaTime);

            if (angle2ndP > 20 || angle2ndP < -20)
            {
                _targetSpeed = _maxSpeed - (Mathf.Abs(angle2ndP) / 180 * _maxSpeed / _agility) * (2 - difficulty[i]);
                if (_targetSpeed > _maxSpeed)
                {
                    _targetSpeed = _maxSpeed;
                }
                else if (_targetSpeed < 0)
                {
                    _targetSpeed = 0;
                }
            }
            */
            //Debug.Log(i);

            BotMove(i);
            BotTurn(i, angle, angleVel);
        }
    }

    void BotMove(int i)
    {
        if (Bots[i].GetComponent<Rigidbody>().velocity.magnitude < _targetSpeed)
        {
            _spaceship.Forward();
        }
        else if (Bots[i].GetComponent<Rigidbody>().velocity.magnitude > _targetSpeed)
        {
            Debug.Log("airbraaake");
            _spaceship.AirBrake(false);
            _spaceship.AirBrake(true);
        }
    }

    void BotTurn(int i, float angle, float angleVel)
    {
        if (angleVel > (10 + 100 * difficulty[i]) / _rb.velocity.magnitude)
        {
            //Debug.Log(_rb.velocity.magnitude);
            if (angleVel > (20 + 130 * difficulty[i]) / _rb.velocity.magnitude && _rb.velocity.magnitude > 0.125f * _targetSpeed)
            {
                _spaceship.AirBrake(false);
            }
            else
            {
                _spaceship.Turn(false);
            }
            
        }
        else if (angleVel < -(10 + 100 * difficulty[i]) / _rb.velocity.magnitude)
        {
            if (angleVel < -(20 + 130 * difficulty[i]) / _rb.velocity.magnitude && _rb.velocity.magnitude > 0.125f * _targetSpeed)
            {
                _spaceship.AirBrake(true);
            }
            else
            {
                _spaceship.Turn(true);
            }
        }

        // Air brakes
        else if (angle > 50 / _rb.velocity.magnitude && _rb.velocity.magnitude >= 1)
        {
            _spaceship.AirBrake(false);
        }
        else if (angle < -50 / _rb.velocity.magnitude && _rb.velocity.magnitude >= 1)
        {
            _spaceship.AirBrake(true);
        }

        // Normal turn
        else if (angle > 0.1f)
        {
            _spaceship.Turn(false);
        }
        else if (angle < -0.1f)
        {
            _spaceship.Turn(true);
        }
    }

    // get la position du next waypoint
    private Vector3 GetNextPos()
    {
        return WaypointManager.Instance.GetWaypointPos(_spaceship.GetWaypoint() + 1);
    }

    public float GetDifficulty(int i)
    {
        return difficulty[i];
    }
}