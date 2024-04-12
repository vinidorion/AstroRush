using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	private float inputx = 0;
	private float inputz = 0;

	private SpaceShip _spaceship;

    public static Player Instance;

    void Awake()
	{
		_spaceship = GetComponent<SpaceShip>();
        Instance = this;
    }

	void Update() //garder a Update sinon les PU ne sortent pas
	{
		if(!_spaceship.isFrozen()) {
			move();
			UsePU();
			ChangeCameraMode();
        }
	}

	private void move()
	{
		inputx = Input.GetAxis("Horizontal");
		inputz = Input.GetAxis("Vertical");
		if (inputz > 0) _spaceship.Forward();
		if (inputz < 0) _spaceship.backward();
		if (inputx > 0) _spaceship.Turn(false);
		if (inputx < 0) _spaceship.Turn(true);
		if (Input.GetKey(KeyCode.Mouse0)) _spaceship.AirBrake(true);
		if (Input.GetKey(KeyCode.Mouse1)) _spaceship.AirBrake(false);
	}

	private void UsePU()
	{
		if (Input.GetKeyDown(KeyCode.Space) && !_spaceship.isFrozen()) {
			_spaceship.UsePU();
		}
	}

	private void ChangeCameraMode()
	{
		if (Input.GetKeyDown(KeyCode.C)) { Camera.Instance.RotateCameraMode(); }
	}

	public SpaceShip GetSpaceShip()
	{
		return _spaceship;
	}
}
