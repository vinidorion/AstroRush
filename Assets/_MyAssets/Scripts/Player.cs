using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public static Player Instance;
	private SpaceShip _spaceship;

	void Awake()
	{
		_spaceship = GetComponent<SpaceShip>();
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(this.gameObject);
		}
	}

	// controles dans l'Update() et non dans FixedUpdate()
	void Update()
	{
		if(!_spaceship.isFrozen()) {
			Move();
			UsePU();
			ChangeCameraMode();
		}
	}

	// Envoie l'information au spaceship pour si il doit bouger d'une certaine facon
	private void Move()
	{
		float inputx = Input.GetAxis("Horizontal");
		float inputz = Input.GetAxis("Vertical");
		if (inputz > 0) _spaceship.Forward();
		if (inputz < 0) _spaceship.Backward();
		if (inputx > 0) _spaceship.Turn(false);
		if (inputx < 0) _spaceship.Turn(true);
		if (Input.GetKey(KeyCode.Mouse0)) _spaceship.AirBrake(true);
		if (Input.GetKey(KeyCode.Mouse1)) _spaceship.AirBrake(false);
		if (Input.GetKey(KeyCode.X)) _spaceship.Orbit();
	}

	// Dit au spaceship qu'il peut utiliser son PU
	private void UsePU()
	{
		if (Input.GetKeyDown(KeyCode.Space)) { // deja un check isFrozen() avant l'utilisation de cette méthode
			_spaceship.UsePU();
		}
	}

	// Permet au joueur de changer le point de vue
	private void ChangeCameraMode()
	{
		if (Input.GetKeyDown(KeyCode.C)) { CameraController.Instance.RotateCameraMode(); }
	}
}
