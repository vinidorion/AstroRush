using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
	private SpaceShip _spaceship;

	private float testcooldown = 0f;

	void Awake()
	{
		_spaceship = GetComponent<SpaceShip>();
		if(_spaceship == null) {
			Debug.Log(this.gameObject + " N'A PAS DE SPACESHIP");
		}
	}

	void Start()
	{

	}

	void Update()
	{
		//_spaceship.Forward();
		if(testcooldown < Time.time) {
			//Debug.Log("angle: " + Quaternion.Angle(transform.rotation, Quaternion.LookRotation(GetNextPos() - transform.position, GetGravDir())));
			testcooldown = Time.time + 1f;
		}
	}

	// get la position du next waypoint
	private Vector3 GetNextPos()
	{
		return WaypointManager.Instance.GetWaypointPos(_spaceship.GetWaypoint() + 1);
	}

	// get la direction de la gravité
	private Vector3 GetGravDir()
	{
		return -_spaceship.GetVecGrav(); // pour le moment
	}
}