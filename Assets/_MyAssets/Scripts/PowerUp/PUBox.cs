using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PUBox : MonoBehaviour
{
	private const float COOLDOWN = 2f;
	private bool _isActive = true;

	private void OnTriggerEnter(Collider other)
	{
		SpaceShip spaceship = other.GetComponent<SpaceShip>();
		if (spaceship != null && _isActive) {
			spaceship.GivePU();
			_isActive = false;
			StartCoroutine(PUBoxCooldownCoroutine());
		}
	}

	IEnumerator PUBoxCooldownCoroutine()
	{
		yield return new WaitForSeconds(COOLDOWN);
		_isActive = true;
	}

	// fonction qui change apparence quand _isActive ou non
}
