using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace poly
{
	[AddComponentMenu("POLYMORPHISME: Quake")]
	public class Quake : Damager
	{
		// Quake qui suit la piste et fait des degats a tout sur son chemin
		protected Vector3 _direction = Vector3.zero; // direction dans laquelle le Quakese deplace
		protected float _speed = 30f; // vitesse de deplacement du quake
		private WaypointFinder _waypoint; // WaypointFinder pour suivre la piste
		protected SpaceShip _ship; // spaceship du proprietaire

		protected void Awake()
		{
			_lifeTime = 10;
			_speed = 30f;
		}

		protected override void Start()
		{
			base.Start();
			_ship = _owner.GetComponent<SpaceShip>(); // va chercher le SpaceShip du proprietaire
			_waypoint = GetComponent<WaypointFinder>(); // va chercher le WaypointFinder
			_waypoint.SetWaypoint(_ship.GetComponent<WaypointFinder>().GetWaypoint()); // va chercher le waypoint actuel du spaceship pour le mettre au quake
		}

		protected virtual void Update()
		{
			transform.position += _direction * _speed * Time.deltaTime; // deplace le quake a travert la piste
			Vector3 vecNxtWpt = WaypointManager.Instance.GetWaypointPos(_waypoint.GetWaypoint() + 1) - transform.position; // prend la position du prochain waypoint
			_direction = vecNxtWpt.normalized; // met la direction vers le prochain waypoint
			transform.LookAt(transform.position + _direction); // se tourne vers la direction
		}
	}
}
