using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitStop : MonoBehaviour
{
	private bool _isActive = true;

	void Awake()
	{
		GetComponent<MeshRenderer>().enabled = false;
		StartCoroutine(CooldownCoroutine());
	}

	IEnumerator CooldownCoroutine()
	{
		yield return new WaitForSeconds(_isActive ? 0.1f : 1f);
		_isActive = !_isActive;
		StartCoroutine(CooldownCoroutine());
	}

	void OnTriggerStay(Collider other)
	{
		if(!_isActive) {
			return;
		}
		
		SpaceShip spaceship = other.GetComponent<SpaceShip>();

		if (spaceship != null) {
			spaceship.GiveHP(5);
		}
	}
}
