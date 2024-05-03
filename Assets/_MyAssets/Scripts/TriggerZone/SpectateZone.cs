using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectateZone : MonoBehaviour
{
	private Vector3 _camPos;
	
	void Awake()
	{
		GetComponent<MeshRenderer>().enabled = false;
		foreach(Transform child in transform) {
			_camPos = child.position;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		SpaceShip spaceship = other.GetComponent<SpaceShip>();

		// ne pas merge	
		if (!spaceship) {
			return;
		}

		if(!spaceship.IsPlayer()) {
			return;
		}
	
		if(CameraController.Instance.GetCameraMode() == CameraMode.Spectate) {
			CameraController.Instance.transform.position = _camPos;
			CameraController.Instance.transform.LookAt(other.transform, Vector3.up);
			CameraController.Instance.GetComponent<Camera>().fieldOfView = 60f;
		}
	}
}
