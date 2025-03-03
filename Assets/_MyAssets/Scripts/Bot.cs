using System.Collections;
using System.Collections.Generic;
// using Unity.VisualScripting; -- ne pas importer de package inutile
//using UnityEditor.PackageManager; -- empêche de build: Assets\_MyAssets\Scripts\Bot.cs(4,19): error CS0234: The type or namespace name 'PackageManager' does not exist in the namespace 'UnityEditor' (are you missing an assembly reference?)
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Bot : MonoBehaviour
{
	public static Bot Instance; // Singleton

	// Start is called before the first frame update
	[SerializeField] private GameObject listeWaypoint;
	[SerializeField] private GameObject listeOptiWaypoint;
	[SerializeField] private GameObject listeBots;
	[SerializeField] private GameObject listeTargets;
	[SerializeField] private float difficulty1 = 1;
	[SerializeField] private float difficulty2 = 1;
	[SerializeField] private float difficulty3 = 0.5f;
	[SerializeField] private float difficulty4 = 0;
	[SerializeField] private float difficulty5 = 0;
	private SpaceShip _spaceship;
	private Rigidbody _rb;
	protected float _agility;
	private List<float> _maxSpeed = new List<float>();
	private float time = 0;

	private List<GameObject> waypoints = new List<GameObject>();
	private List<GameObject> optiWaypoints = new List<GameObject>();
	protected List<GameObject> Bots = new List<GameObject>();
	private List<GameObject> targets = new List<GameObject>();
	private List<int> passedTargets = new List<int>();
	private List<float> difficulty = new List<float>();
	[SerializeField] private float accelbot = 0;

	private int nbBots;		// Nombre de bots sans compter le joueur

	public Plane Plane
	{
		private set;
		get;
	}

	void Awake()
	{
		if(Instance == null) {
			Instance = this;
		} else {
			Destroy(this.gameObject);
		}
	}

	private void Start()
	{
		difficulty.Add(difficulty1);
		difficulty.Add(difficulty2);
		difficulty.Add(difficulty3);
		difficulty.Add(difficulty4);
		difficulty.Add(difficulty5);
		difficulty.Add(0);			// difficulté du player
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
			passedTargets.Add(0);
		}

		nbBots = Bots.Count;
		Bots.Add(Player.Instance.gameObject);

		foreach (Transform child in listeTargets.transform)
		{
			targets.Add(child.gameObject);
		}
		for (int i = 0; i < targets.Count; i++)
		{
			targets[i].transform.position = Vector3.Lerp(waypoints[passedTargets[i]].transform.position,
				optiWaypoints[passedTargets[i]].transform.position, difficulty[i]);
		}
		for (int i = 0; i < Bots.Count; i++)
		{
			_maxSpeed.Add(Bots[i].GetComponent<SpaceShip>().GetMaxSpeed());
		}
		time = Time.time;
	}

	// Update is called once per frame
	void Update()
	{
		// boucle qui fait les actions de tout les bots 1 par 1
		for (int i = 0; i < nbBots; i++)
		{
			_spaceship = Bots[i].GetComponent<SpaceShip>();
			if(_spaceship.isFrozen()) {
				return;
			}
			_rb = _spaceship.GetComponent<Rigidbody>();
			_agility = _spaceship.GetAgility();

			
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
					Debug.Log(Time.time - time);
				}
				//_spaceship.SetWaypoint(passedTargets[i]);
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
			*/
			BotMove(i);
			BotTurn(i, angle, angleVel);
			//Debug.Log(_rb.velocity.magnitude);
		}
	}


	void BotMove(int i)
	{
		if (Bots[i].GetComponent<Rigidbody>().velocity.magnitude < _maxSpeed[i])
		{
			_spaceship.Forward();
		}
		else if (Bots[i].GetComponent<Rigidbody>().velocity.magnitude > _maxSpeed[i])
		{
			_spaceship.AirBrake(false);
			_spaceship.AirBrake(true);
		}
	}

	void BotTurn(int i, float angle, float angleVel)
	{
		if (angleVel > (50 + 70 * difficulty[i]) / _rb.velocity.magnitude)
		{
			if (angleVel > (60 + 100 * difficulty[i]) / _rb.velocity.magnitude && _rb.velocity.magnitude > 0.125f * _maxSpeed[i])
			{
				_spaceship.AirBrake(false);
			}
			else
			{
				_spaceship.Turn(false);
			}
			
		}
		else if (angleVel < -(50 + 70 * difficulty[i]) / _rb.velocity.magnitude)
		{
			if (angleVel < -(60 + 100 * difficulty[i]) / _rb.velocity.magnitude && _rb.velocity.magnitude > 0.125f * _maxSpeed[i])
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
		else if (angle > 0.5f)
		{
			_spaceship.Turn(false);
		}
		else if (angle < -0.5f)
		{
			_spaceship.Turn(true);
		}
	}

	public float GetDifficulty(int i)
	{
		return difficulty[i];
	}

	public void SetTSpeed(float TSpeed, int index)
	{
		_maxSpeed[index] = TSpeed;
	}

	// méthode publique qui augmente de un le nombre d'itération de la boucle dans l'Update()
	public void AddPlayerToBots()
	{
		nbBots++;
	}
}