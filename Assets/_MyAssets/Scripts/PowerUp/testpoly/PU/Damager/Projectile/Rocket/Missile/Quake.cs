using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace poly
{
    [AddComponentMenu("POLYMORPHISME: Quake")]
    public class Quake : Damager
    {
        protected Vector3 _direction = Vector3.zero;
        protected float _speed = 30f;
        private int _waypoint;
        protected SpaceShip _ship;

        protected void Awake()
        {
            _lifeTime = 10;
            _speed = 30f;
        }

        protected override void Start()
        {
            base.Start();
            _ship = _owner.GetComponent<SpaceShip>();
            _waypoint = _ship.GetWaypoint();
            GetComponent<WaypointFinder>().SetWaypoint(_waypoint);
        }

        protected virtual void Update()
        {
            transform.position += _direction * _speed * Time.deltaTime;
            Vector3 vecNxtWpt = WaypointManager.Instance.GetWaypointPos(_waypoint + 1) - transform.position; 
            _direction = vecNxtWpt.normalized;
            transform.LookAt(transform.position + _direction);
        }

        public void SetWaypoint(int waypoint)
        {
            _waypoint = waypoint;
        }
    }
}
