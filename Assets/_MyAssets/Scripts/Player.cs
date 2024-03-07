using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	private float inputx = 0;
	private float inputz = 0;

	private SpaceShip _spaceship;

	void Awake()
	{
		_spaceship = GetComponent<SpaceShip>();
	}

	void Update()
	{
		if(!_spaceship.isFrozen()) {
			move();
			UsePU();
		}
	}

	private void move()
	{
		inputx = Input.GetAxis("Horizontal");
		inputz = Input.GetAxis("Vertical");
		if (inputz > 0) _spaceship.Forward();
		if (inputx > 0) _spaceship.Turn(false);
		if (inputx < 0) _spaceship.Turn(true);
		if (Input.GetKey(KeyCode.Mouse0)) _spaceship.AirBrake(true);
		if (Input.GetKey(KeyCode.Mouse1)) _spaceship.AirBrake(false);
	}

	private void UsePU()
	{
		if (Input.GetKey(KeyCode.Space))
		{
			_spaceship.UsePU();
		}
	}


}
