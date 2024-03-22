using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitStop : MonoBehaviour
{
	void Awake()
	{
		GetComponent<MeshRenderer>().enabled = false;
	}

	// on physic timer (called every 0.02s comme dans FixedUpdate())
	void OnTriggerStay(Collider other)
	{
		// rajouter un cooldown ici
		SpaceShip spaceship = other.GetComponent<SpaceShip>();

		if (spaceship != null) {
			spaceship.GiveHP(1);
		}
	}
}
