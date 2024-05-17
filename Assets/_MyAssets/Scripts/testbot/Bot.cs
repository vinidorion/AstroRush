using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace testbot {
	[AddComponentMenu("TEST BOT")]
	public class Bot : MonoBehaviour
	{
		private SpaceShip _spaceship;
		private WaypointFinder _waypoint;

		private float _angAccel		= 45f;
		private float _angTurn		= 12f;
		private float _angAirBrake	= 30f;

		void Awake()
		{
			_spaceship = GetComponent<SpaceShip>();
			_waypoint = GetComponent<WaypointFinder>();
		}

		void Update()
		{
			if(_spaceship.isFrozen()) {
				return;
			}

			//Debug.DrawLine(transform.position, transform.position + (transform.forward * 2f), Color.red, Time.deltaTime);
			float angNxtWpt = GetAngNxtWpt();

			// FORWARD
			if (angNxtWpt > -_angAccel && angNxtWpt < _angAccel) {
				//Debug.Log("forward");
				_spaceship.Forward();
			}
			
			// TURN
			if(angNxtWpt < -_angTurn) {
				_spaceship.Turn(true);
			} else if(angNxtWpt > _angTurn) {
				_spaceship.Turn(false);
			}

			// AIRBRAKE
			if(angNxtWpt < -_angAirBrake) {
				_spaceship.AirBrake(true);
			} else if(angNxtWpt > _angAirBrake) {
				_spaceship.AirBrake(false);
			}
		}

		// méthode privée qui retourne l'angle vers le prochain waypoint
		private float GetAngNxtWpt()
		{
			Vector3 vecNextPos = WaypointManager.Instance.GetWaypointPos(_waypoint.GetWaypoint() + 1);
			Vector3 vecGravDir = -_spaceship.GetVecGrav();
			return Vector3.SignedAngle(transform.forward, vecNextPos - transform.position, vecGravDir);
		}

		// méthode publique qui assigne niveau de difficulté
		public void InitializeDifficulty(float size)
		{
			SetNewDifficulty(ref _angAccel, size);
			SetNewDifficulty(ref _angTurn, size);
			SetNewDifficulty(ref _angAirBrake, size);
			//Debug.Log($"{this.gameObject}: _angAccel({_angAccel}), _angTurn({_angTurn}), _angAirBrake({_angAirBrake})");
		}

		private void SetNewDifficulty(ref float value, float size)
		{
			value += Random.Range(-size, size);
		}
	}
}