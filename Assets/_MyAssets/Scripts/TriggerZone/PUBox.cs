using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PUBox : MonoBehaviour
{
	private const float COOLDOWN = 2f;
	private bool _isActive = true;

	void OnTriggerEnter(Collider other)
	{
		if(!_isActive) { // éviter de GetComponent si le PUBox n'est même pas active
			return;
		}

		SpaceShip spaceship = other.GetComponent<SpaceShip>();
		if (spaceship != null) {
			// ne pas merge ce check de condition avec celui en haut
			if(spaceship.GetPU() != -1) {  
				return;
			}

            spaceship.GivePU();
			_isActive = false;
			StartCoroutine(PUBoxCooldownCoroutine());
		}
	}

	// coroutine pour reset l'état actif après un cooldown
	IEnumerator PUBoxCooldownCoroutine()
	{
		yield return new WaitForSeconds(COOLDOWN);
		_isActive = true;
	}

	// fonction qui change apparence quand _isActive ou non
}
