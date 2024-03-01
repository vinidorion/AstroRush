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
		move();
		UsePU();
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

			//if (current_PU == 0)
			//{
			//	if (timer_atk > 0.2f)
			//	{
			//		Instantiate(PU[current_PU], new Vector3(transform.position.x + gunPosition.x, transform.position.y + gunPosition.y, transform.position.z + gunPosition.z), transform.rotation * Quaternion.Euler(90, 0, 0), transform);
			//		Instantiate(PU[current_PU], new Vector3(transform.position.x - gunPosition.x, transform.position.y + gunPosition.y, transform.position.z + gunPosition.z), transform.rotation * Quaternion.Euler(90, 0, 0), transform);
			//		timer_atk = 0;
			//		laser_uses++;
			//		if (laser_uses >= 5) current_PU = -1;
			//	}
			//}
			//else if (current_PU == 1)
			//{
			//	Instantiate(PU[current_PU], new Vector3(transform.position.x + canonPosition.x, transform.position.y + canonPosition.y, transform.position.z + canonPosition.z), transform.rotation * Quaternion.Euler(90, 0, 0), transform);
			//	current_PU = -1;
			//}
			//else if (current_PU == 2)
			//{
			//	Instantiate(PU[current_PU], new Vector3(transform.position.x + canonPosition.x, transform.position.y + canonPosition.y, transform.position.z + canonPosition.z), transform.rotation * Quaternion.Euler(0, 0, 0), transform);
			//	current_PU = -1;
			//}
		}
	}


}
