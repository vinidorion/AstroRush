using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace poly
{
	[AddComponentMenu("POLYMORPHISM: Missile")]
	public class Missile : Rocket
	{
		protected Transform _target;
		private int _waypoint;
		protected SpaceShip _ship;

		protected override void Awake()
		{
			base.Awake();

			_speed = 30f;
		}

        protected virtual void Start()
        {
			_ship = _owner.GetComponent<SpaceShip>();

            if (_owner)
            {
                FindTarget();
                _waypoint = _ship.GetWaypoint();
                GetComponent<WaypointFinder>().SetWaypoint(_waypoint);
            }
        }

        protected override void Update()
		{
			base.Update();

			if(_target) {
                Vector3 targetPos = _target.transform.position;
                Vector3 nextWaypointPos = WaypointManager.Instance.GetWaypointPos(_waypoint + 1);
                Vector3 pos = transform.position;
                if ((targetPos - pos).magnitude <= (nextWaypointPos - pos).magnitude)
				{
					SetDirection((targetPos - pos).normalized);
                }
				else
				{
					SetDirection((nextWaypointPos - pos).normalized);
				}
			}
		}

		private void FindTarget()
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

		// utilisé dans WaypointFinder
		public void SetWaypoint(int waypoint)
		{
			_waypoint = waypoint;
		}
	}
}