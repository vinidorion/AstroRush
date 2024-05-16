using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace poly
{
	[AddComponentMenu("POLYMORPHISM: Missile")]
	public class Missile : Rocket
	{
		// script por tout les PU de type missile qui vont chercher une cible
		private WaypointFinder _waypoint; // WaypointFinder pour suivre la piste
		protected Transform _target = null; // cible a toucher
		protected SpaceShip _ship; // SpaceShip du proprietaire

		protected override void Awake()
		{
			//_lifeTime = ;
			_speed = 30f;
		}

		protected override void Start()
		{
			base.Start();
			_ship = _owner.GetComponent<SpaceShip>(); // prend le spaceship du proprietaire

			if (_target == null) FindTarget(); // trouve une cible
			_waypoint = GetComponent<WaypointFinder>(); // va chercher le WaypointFinder
			_waypoint.SetWaypoint(_ship.GetComponent<WaypointFinder>().GetWaypoint()); // donne le waypoint du spaceship au missile
		}
		
		protected override void Update()
		{
			base.Update();

			if(_target) {
				Vector3 vecTarget = _target.transform.position - transform.position; // distance vers la cible
				Vector3 vecNxtWpt = WaypointManager.Instance.GetWaypointPos(_waypoint.GetWaypoint() + 1) - transform.position; // distance du prochain waypoint

				// pas besoin de sqrt quand c'est une comparaison de distance
				if (vecTarget.sqrMagnitude <= vecNxtWpt.sqrMagnitude) { // met la diretion sur le plus cours des 2
					SetDirection(vecTarget.normalized);
				} else {
					SetDirection(vecNxtWpt.normalized);
				}
			}
			else
			{
				Vector3 vecNxtWpt = WaypointManager.Instance.GetWaypointPos(_waypoint.GetWaypoint() + 1) - transform.position; // prend le prochain waypoint si il n'y a pas de cible
				SetDirection(vecNxtWpt.normalized);
			}
		}

		// permet de trouver la cible a viser
		protected virtual void FindTarget()
		{
			if (_owner.GetComponent<SpaceShip>().GetPosition() != 0) // va chercher la cible selon le spaceship en avant 
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
			_target = PosManager.Instance.GetShipFromPos(target).transform; // va chercher le spaceship selon sa position
		}
	}
}