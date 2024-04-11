using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitStop : MonoBehaviour
{
	private const float COOLDOWN = 1f;
	private float _nextCooldown;
	
	private bool _testBool = false;

	void Awake()
	{
		GetComponent<MeshRenderer>().enabled = false;
	}

	// on physic timer (called every 0.02s comme dans FixedUpdate())
	void OnTriggerStay(Collider other)
	{
		if(_testBool) {
			return;
		}
		
		_testBool = true;

		/*if(Time.time > _nextCooldown) {
			SpaceShip spaceship = other.GetComponent<SpaceShip>();

			if (spaceship != null) {
				spaceship.GiveHP(5); // tester avec différentes valeurs
			}
			_nextCooldown = Time.time + COOLDOWN;
		}*/
	}
}
