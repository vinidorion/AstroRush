using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapComplete : MonoBehaviour
{
	void Awake()
	{
		//GetComponent<MeshRenderer>().enabled = false;
	}

	void OnTriggerEnter(Collider other)
	{
		SpaceShip spaceship = other.GetComponent<SpaceShip>();

		if (!spaceship) {
			return;
		}
	
		// ne pas merge ce check de condition
		// la ligne d'arrivée ne se trouvera probablement pas entre le dernier et premier waypoint
		// on peut simplement vérifier que le spaceship a fait au moins le 3/4 de la piste de course
		// (pour empecher le joueur de faire immédiatement demi tour et entrer dans cette triggerbox plusieurs fois)
		// la ligne d'arrivée est une triggerbox et non le moment ou le spaceship passe du dernier au premier waypoint
		// parce que c'est plus facile d'ajuster la position de la triggerbox dans unity que d'ajuster les waypoints dans blender
		if(spaceship.GetComponent<WaypointFinder>().GetWaypoint() / (float)WaypointManager.Instance.GetNbWpt() > 0.75f) {
			spaceship.LapCompleted();
		}
	}
}
