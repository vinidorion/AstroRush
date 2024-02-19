using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
	void Awake()
	{
		GetComponent<MeshRenderer>().enabled = false;
	}

	void OnTriggerEnter(Collider other)
	{
		SpaceShip spaceship = other.GetComponent<SpaceShip>();

		if (spaceship != null) {
			StartCoroutine(Respawn(spaceship));
		}
	}

	IEnumerator Respawn(SpaceShip spaceship)
	{
		// fade to black ici
		yield return new WaitForSeconds(1f);

		int shipWayptIndex = spaceship.GetWaypoint();
		Vector3 respawnPos = WaypointManager.Instance.GetWaypointPos(shipWayptIndex);
		Vector3 nextWaypoint = WaypointManager.Instance.GetWaypointPos(shipWayptIndex + 1);

		Rigidbody rb = spaceship.GetComponent<Rigidbody>();
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;

		spaceship.transform.position = respawnPos;
		spaceship.transform.rotation = Quaternion.LookRotation(nextWaypoint - respawnPos);

		yield return null;
	}
}