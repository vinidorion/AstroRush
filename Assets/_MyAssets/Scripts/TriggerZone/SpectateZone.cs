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
		if (spaceship != null) { // ne pas merge ce check de condition
			if(Camera.Instance.GetCameraMode() == CameraMode.Spectate) {
				Camera.Instance.transform.position = _camPos;
				Camera.Instance.transform.LookAt(other.transform, Vector3.up);
			}
		}
	}
}
