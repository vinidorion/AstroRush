using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour
{
	public static SpaceShip Instance;

	bool freeze = false;
	Vector3 current_speed = Vector3.zero;
	int current_pu = 0;

	[Header("Stats")]
	[SerializeField] float max_speed = default;
	[SerializeField] float acceleration = default;
	[SerializeField] float airbrake_power = default;
	[SerializeField] int hp = default;
	[SerializeField] float weigth = default;
	[SerializeField] float agility = default;

	[SerializeField] Rigidbody rb = default;

	// variables pour détection du sol
	[SerializeField] private LayerMask _layersToHit;    // dans l'inspecteur mettre la layer "track"
	private float _maxDist = 5f;                        // distance maximale du raycast
	private Vector3 _rayDir = new Vector3(0, 0, -1);    // par défaut cherche vers le bas

	private void Awake()
	{
		Instance = this; //associe l'instance de Player au script
	}

	private void Start()
	{

	}

	void FixedUpdate()
	{
		CheckForGround();
	}

	// méthode privée qui trouve la direction de la gravité
	private void CheckForGround()
	{
		Ray ray = new Ray(transform.position, _rayDir);
		if(Physics.Raycast(ray, out RaycastHit hit, _maxDist, _layersToHit, QueryTriggerInteraction.Ignore)) {
			//Debug.Log("found ground");
			_rayDir = -hit.normal;
			Debug.DrawLine(transform.position, hit.point, Color.red, Time.fixedDeltaTime);
			Debug.DrawLine(transform.position, transform.position + (hit.normal * 0.3f), Color.blue, Time.fixedDeltaTime);
		} else {
			//Debug.Log("ground not found");
		}
	}

	private void Update()
	{
		AirResistance();
		current_speed = rb.velocity;
		Debug.Log(current_speed);
	}

	private void AirResistance()
	{
		Vector3 force = new Vector3(-max_speed * (current_speed.x / max_speed), -max_speed * (current_speed.y / max_speed), -max_speed * (current_speed.z / max_speed))/2;
		rb.AddForce(force);
	}

	public void Forward()
	{
		if (!freeze && current_speed.y < max_speed)
		{
			Vector3 force = new Vector3(-1 * acceleration, 0, 0);
			rb.AddForce(force);
		}
	}

	public void Turn(bool left)
	{
		if (!freeze)
		{
			if (left)
			{
				float rotation = agility - current_speed.y;
			}
			if (!left)
			{
				float rotation = (agility - current_speed.y) * -1;
			}
		}
	}

	public void AirBrake(bool left)
	{
		if (!freeze)
		{
			if (left)
			{
				float rotation = agility - current_speed.y + airbrake_power;
				Vector3 force = new Vector3(1 * current_speed.y + airbrake_power - weigth, 0, 0);
				rb.AddForce(force);
			}
			if (!left)
			{
				float rotation = (agility - current_speed.y + airbrake_power) * -1;
				Vector3 force = new Vector3(1 * current_speed.y + airbrake_power - weigth, 0, 0);
				rb.AddForce(force);
			}
		}
	}

	public void UsePU()
	{
		if (!freeze)
		{

		}
	}

	void RemovePU()
	{

	}

	void AddPU()
	{

	}
}


