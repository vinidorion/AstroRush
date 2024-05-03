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
			if(!_spaceship) {
				Debug.Log(this.gameObject + " N'A PAS DE SPACESHIP");
			}
			if(!_waypoint) {
				Debug.Log(this.gameObject + " N'A PAS DE WAYPOINTFINDER");
			}
		}

		void Update()
		{
			if(_spaceship.isFrozen()) {
				return;
			}

			//Debug.DrawLine(transform.position, transform.position + (GetGravDir() * 2f), Color.red, Time.deltaTime);

			Debug.DrawLine(transform.position, transform.position + (transform.forward * 2f), Color.red, Time.deltaTime);

			float angNxtWpt = Vector3.SignedAngle(transform.forward, GetNextPos() - transform.position, GetGravDir());
			//Debug.Log($"angNxtWpt: {angNxtWpt.ToString("F2")}");

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

		// get la position du next waypoint
		private Vector3 GetNextPos()
		{
			return WaypointManager.Instance.GetWaypointPos(_waypoint.GetWaypoint() + 1);
		}

		// get la direction de la gravité
		private Vector3 GetGravDir()
		{
			return -_spaceship.GetVecGrav();
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


