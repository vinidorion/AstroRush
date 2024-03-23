using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poly
{
	[AddComponentMenu("POLYMORPHISM: Missile")]
	public class Missile : Rocket
	{
		private Transform _target;
		private int _waypoint;

		protected override void Awake()
		{
			base.Awake();

			_speed = 50f;
		}

		protected override void Update()
		{
			base.Update();

			if(_owner) {
				FindTarget();
				_waypoint = _owner.GetComponent<SpaceShip>().GetWaypoint();
				GetComponent<WaypointFinder>().SetWaypoint(_waypoint);
				_owner = null;
			}

			if(_target) {
				SetDirection((_target.position - transform.position).normalized);
			} else {
				// vers le prochain waypoint
			}
		}

		private void FindTarget()
		{
			// trouver la cible ici
		}

		// utilisé dans WaypointFinder
		public void SetWaypoint(int waypoint)
		{
			_waypoint = waypoint;
		}

		// get la position du next waypoint
		private Vector3 GetNextPos()
		{
			return WaypointManager.Instance.GetWaypointPos(_waypoint + 1);
		}
	}
}