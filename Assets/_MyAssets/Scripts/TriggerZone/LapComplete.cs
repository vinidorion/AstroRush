using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapComplete : MonoBehaviour
{
	void Awake()
	{
		GetComponent<MeshRenderer>().enabled = false;
	}

	void OnTriggerEnter(Collider other)
	{
		SpaceShip spaceship = other.GetComponent<SpaceShip>();

		if (spaceship != null) {
			// check if le spaceship a fait au moins 3/4 des waypoints
			// utiliser méthode qui _lap++ sur le spaceship
		}
	}
}
