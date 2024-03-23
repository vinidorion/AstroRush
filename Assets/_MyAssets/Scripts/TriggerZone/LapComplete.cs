using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapComplete : MonoBehaviour
{
	void Awake()
	{
		//GetComponent<MeshRenderer>().enabled = false;
	}

	void OnTriggerEnter(Collider other)
	{
		SpaceShip spaceship = other.GetComponent<SpaceShip>();

		if (spaceship != null) { // ne pas mettre ces deux vérifications de conditions dans le même if
			if(spaceship.GetWaypoint() / (float)WaypointManager.Instance.GetLapNb() > 0.75f) {
				spaceship.LapCompleted();
				Debug.Log("LAP COMPLETED CALLED");
			}
		}
	}
}
