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
    private float _agility;
    private float _targetSpeed;
    private float _maxSpeed;

    private List<GameObject> waypoints = new List<GameObject>();
    private List<GameObject> optiWaypoints = new List<GameObject>();
    private List<GameObject> Bots = new List<GameObject>();
    private List<GameObject> targets = new List<GameObject>();
    private List<Vector3> directionRight = new List<Vector3>();
    private List<int> passedTargets = new List<int>();
    private List<float> difficulty = new List<float>();
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
            _maxSpeed = _spaceship.GetMaxSpeed();
            _targetSpeed = _maxSpeed;

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
            Vector3 direction2nd = Plane.ClosestPointOnPlane(Vector3.Lerp(waypoints[passedTargets[i] + 1].transform.position,
                optiWaypoints[passedTargets[i] + 1].transform.position, difficulty[i])) - Bots[i].transform.position;

            float angle = Vector3.SignedAngle(Bots[i].transform.forward, direction, Bots[i].transform.up);
            float angleVel = Vector3.SignedAngle(Bots[i].GetComponent<Rigidbody>().velocity, direction, Bots[i].transform.up);
            float angle2ndP = Vector3.SignedAngle(Bots[i].GetComponent<Rigidbody>().velocity, direction2nd, Bots[i].transform.up);

            Debug.DrawLine(_spaceship.transform.position, targets[i].transform.position, Color.green, Time.fixedDeltaTime);
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
            
            //Debug.Log(i);
            Debug.Log(_targetSpeed);

            BotMove(i);
            if (i == 1)
            {
                //Debug.Log(angle);
            }
            BotTurn(i, angle, angleVel);
        }
    }

    void BotMove(int i)
    {
        if (Bots[i].GetComponent<Rigidbody>().velocity.magnitude <= _targetSpeed)
        {
            _spaceship.Forward();
        }
        else if (Bots[i].GetComponent<Rigidbody>().velocity.magnitude > _targetSpeed)
        {
            _spaceship.backward();
        }

        /*
        if (angleVel > 20 && _rb().velocity.magnitude > 3)
        {
            _spaceship.backward();
        }
        if (angleVel < -20 && _rb().velocity.magnitude > 3)
        {
            _spaceship.backward();
        }
        */
    }

    void BotTurn(int i, float angle, float angleVel)
    {

        // OVERSTEER // A FIX
        if (angleVel > (5 + 60 * difficulty[i]) / _rb.velocity.magnitude)
        {
            Debug.Log(_rb.velocity.magnitude);
            if (angleVel > (10 + 120 * difficulty[i]) / _rb.velocity.magnitude && _rb.velocity.magnitude > 0.125f * _maxSpeed)
            {
                _spaceship.AirBrake(false);
                _spaceship.transform.Rotate(0f, 80f * Time.deltaTime * _agility, 0f);
                directionRight[i] = Quaternion.AngleAxis(80f * Time.deltaTime * _agility, _spaceship.transform.up) * directionRight[i];
                Debug.Log("oversteer right");
            }
            else
            {
                _spaceship.transform.Rotate(0f, 50f * Time.deltaTime * _agility, 0f);
                directionRight[i] = Quaternion.AngleAxis(50f * Time.deltaTime * _agility, _spaceship.transform.up) * directionRight[i];
                //Debug.Log("oversteer right");
            }
            
        }
        else if (angleVel < -(5 + 60 * difficulty[i]) / _rb.velocity.magnitude && _rb.velocity.magnitude > 0.125f * _maxSpeed)
        {
            if (angleVel < -(10 + 120 * difficulty[i]) / _rb.velocity.magnitude)
            {
                _spaceship.AirBrake(true);
                _spaceship.transform.Rotate(0f, -50f * Time.deltaTime * _agility, 0f);
                directionRight[i] = Quaternion.AngleAxis(-80f * Time.deltaTime * _agility, _spaceship.transform.up) * directionRight[i];
                Debug.Log("oversteer left");
            }
            else
            {
                _spaceship.transform.Rotate(0f, -50f * Time.deltaTime * _agility, 0f);
                directionRight[i] = Quaternion.AngleAxis(-80f * Time.deltaTime * _agility, _spaceship.transform.up) * directionRight[i];
                //Debug.Log("oversteer left");
            }
        }

        // Air brakes
        else if (angle > 50 / _rb.velocity.magnitude && Bots[i].GetComponent<Rigidbody>().velocity.magnitude >= 1)
        {
            _spaceship.AirBrake(false);
            _spaceship.transform.Rotate(0f, 100f * Time.deltaTime * _agility, 0f);
            directionRight[i] = Quaternion.AngleAxis(100f * Time.deltaTime * _agility, _spaceship.transform.up) * directionRight[i];
            Debug.Log("air brakes right");
        }
        else if (angle < -50 / _rb.velocity.magnitude && _rb.velocity.magnitude >= 1)
        {
            _spaceship.AirBrake(true);
            _spaceship.transform.Rotate(0f, -100f * Time.deltaTime * _agility, 0f);
            directionRight[i] = Quaternion.AngleAxis(-100f * Time.deltaTime * _agility, _spaceship.transform.up) * directionRight[i];
            Debug.Log("air brakes left");
        }
        // Normal turn
        else if (angle > 0.1f)
        {
            //_spaceship.Turn(true);
            _spaceship.transform.Rotate(0f, 66f * Time.deltaTime * _agility, 0f);
            directionRight[i] = Quaternion.AngleAxis(66f * Time.deltaTime * _agility, _spaceship.transform.up) * directionRight[i];
            Debug.Log("turn right");
        }
        else if (angle < -0.1f)
        {
            //_spaceship.Turn(false);
            _spaceship.transform.Rotate(0f, -66f * Time.deltaTime * _agility, 0f);
            directionRight[i] = Quaternion.AngleAxis(-66f * Time.deltaTime * _agility, _spaceship.transform.up) * directionRight[i];
            Debug.Log("turn right");
        }
    }

    void SetPitch()
    {
        /*
        Ray rayfront = new Ray(front.transform.position, -transform.up);
        if(Physics.Raycast(rayfront, out RaycastHit hit, 0.5f, _layersToHit, QueryTriggerInteraction.Ignore)) {
			
		} else {
			//Debug.Log("ground not found");
			Debug.DrawLine(transform.position, transform.position + _rayDir, Color.red, Time.fixedDeltaTime);		// direction de la gravité (sans utiliser hit.point)
			_onGround = false;
		}
        Ray raybackleft = new Ray(backleft.transform.position, -transform.up);
        Ray raybackright = new Ray(backright.transform.position, -transform.up);
        Vector3.Cross(FrontVec - BackLeftvec, FrontVec - BackRightVec)
        */
    }

    // get la position du next waypoint
    private Vector3 GetNextPos()
    {
        return WaypointManager.Instance.GetWaypointPos(_spaceship.GetWaypoint() + 1);
    }
}