using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointFinder : MonoBehaviour
{
	private int _wayPoint;

	void Update()
	{
		SendInfo();
	}

	void FixedUpdate()
	{
		Waypoints(); // expensive, ne pas mettre dans Update()
	}

	private void Waypoints()
	{
		Vector3 waypointPos = WaypointManager.Instance.GetWaypointPos(_wayPoint);			// position du current waypoint
		Vector3 nextwaypointPos = WaypointManager.Instance.GetWaypointPos(_wayPoint + 1);	// position du next waypoint

		//Debug.Log("waypointPos: " + waypointPos);
		//Debug.Log("nextwaypointPos: " + nextwaypointPos);

		// pour visualiser à quel waypoint le spaceship est rendu
		Debug.DrawLine(transform.position, waypointPos, Color.green, Time.fixedDeltaTime);
		Debug.DrawLine(transform.position, nextwaypointPos, Color.blue, Time.fixedDeltaTime);

		// sqrt inutile ici, on compare deux distances
		float distCurrWaypoint = (transform.position - waypointPos).sqrMagnitude;
		float distNextWaypoint = (transform.position - nextwaypointPos).sqrMagnitude;

		// si la distance entre le spaceship et le prochain waypoint
		// et plus petite que
		// la distance entre le spaceship et son waypoint actuel
		if (distNextWaypoint < distCurrWaypoint) {
			_wayPoint++;
			if (WaypointManager.Instance.IsFinalWaypoint(_wayPoint)) {
				_wayPoint = 0;
			}
		}
	}

	private void SendInfo()
	{
		if (GetComponent<SpaceShip>() == null) {
			GetComponent<Projectile>().SetWaypoint(_wayPoint);
		} else if (GetComponent<Projectile>() == null) {
			GetComponent<SpaceShip>().SetWaypoint(_wayPoint);
		} else {
			Debug.Log(gameObject + " HAS WAYPOINTFINDER BUT ISNT A SPACESHIP NOR A PROJECTILE");
		}
	}
}
