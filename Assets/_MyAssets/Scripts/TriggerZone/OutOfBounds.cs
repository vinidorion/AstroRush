using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
	private const float DELAY = 1f;

	void Awake()
	{
		GetComponent<MeshRenderer>().enabled = false;
	}

	void OnTriggerEnter(Collider other)
	{
		SpaceShip spaceship = other.GetComponent<SpaceShip>();

		if (spaceship != null) {
			StartCoroutine(RespawnCoroutine(spaceship));
		}
	}

	IEnumerator RespawnCoroutine(SpaceShip spaceship)
	{
		yield return new WaitForSeconds(DELAY);

		int shipWayptIndex = spaceship.GetComponent<WaypointFinder>().GetWaypoint();
		Vector3 respawnPos = WaypointManager.Instance.GetWaypointPos(shipWayptIndex);
		Vector3 nextWaypoint = WaypointManager.Instance.GetWaypointPos(shipWayptIndex + 1);

		Rigidbody rb = spaceship.GetComponent<Rigidbody>();
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;

		spaceship.transform.position = respawnPos;
		spaceship.transform.rotation = Quaternion.LookRotation(nextWaypoint - respawnPos, -(spaceship.GetVecGrav()));

		yield return null;
	}
}