using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace poly
{
	[AddComponentMenu("POLYMORPHISM: Missile")]
	public class Missile : Rocket
	{
		private WaypointFinder _waypoint;
		protected Transform _target = null;
		protected SpaceShip _ship;

		protected override void Awake()
		{
			//_lifeTime = ;
			_speed = 30f;
		}

		protected override void Start()
		{
			base.Start();
			_ship = _owner.GetComponent<SpaceShip>();

			if (_target == null) FindTarget();
			_waypoint = GetComponent<WaypointFinder>();
			_waypoint.SetWaypoint(_ship.GetComponent<WaypointFinder>().GetWaypoint());
		}

		protected override void Update()
		{
			base.Update();

			if(_target) {
				Vector3 vecTarget = _target.transform.position - transform.position;
				Vector3 vecNxtWpt = WaypointManager.Instance.GetWaypointPos(_waypoint.GetWaypoint() + 1) - transform.position;

				// pas besoin de sqrt quand c'est une comparaison de distance
				if (vecTarget.sqrMagnitude <= vecNxtWpt.sqrMagnitude) {
					SetDirection(vecTarget.normalized);
				} else {
					SetDirection(vecNxtWpt.normalized);
				}
			}
			else
			{
				Vector3 vecNxtWpt = WaypointManager.Instance.GetWaypointPos(_waypoint.GetWaypoint() + 1) - transform.position;
				SetDirection(vecNxtWpt.normalized);
			}
		}

		protected virtual void FindTarget()
		{
			if (_owner.GetComponent<SpaceShip>().GetPosition() != 0)
			{
				_target = PosManager.Instance.GetShipFromPos(_ship.GetPosition() - 1).transform;
			}
			else
			{
				_target = PosManager.Instance.GetLastShip().transform;
			}
		}

		public void SetTarget(int target)
		{
			_target = PosManager.Instance.GetShipFromPos(target).transform;
		}
	}
}