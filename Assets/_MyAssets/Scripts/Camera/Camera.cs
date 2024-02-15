using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
	private CameraMode _currentMode;

	void Start()
	{
		_currentMode = CameraMode.Intro;
	}

	void Update()
	{
		switch (_currentMode)
		{
			case CameraMode.Intro:
				Intro();
				break;
			case CameraMode.FirstPerson:
				FirstPerson();
				break;
			case CameraMode.ThirdPerson:
				ThirdPerson();
				break;
			case CameraMode.Spectate:
				Spectate();
				break;
		}
	}

	// enum CameraMode:
		// Intro
		// FirstPerson
		// ThirdPerson
		// Spectate
	public void SetCameraMode(CameraMode mode)
	{
		_currentMode = mode;
	}

	private void Intro() {}
	private void FirstPerson() {}
	private void ThirdPerson() {}
	private void Spectate() {
		//transform.LookAt(target); // target = player

		// TODO: .LookAt() regarde directement vers l'argument target, changer pour que ça smooth, comme le cinematic mode dans minecraft
		/* -- comme ceci:
		 *		var targetObj : GameObject;
		 *		var speed : int = 5;
		 *
		 *		// doit être called dans Update()
		 *		function Update() {
		 *			var targetRotation = Quaternion.LookRotation(targetObj.transform.position - transform.position);
		 *
		 *			// smoothly rotate towards the target point.
		 *			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
		 *		}
		*/
	}
	
}
