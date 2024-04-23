using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFinder : MonoBehaviour
{
	private int _waypoint;

	void FixedUpdate()
	{
		Waypoints(); // expensive, ne pas mettre dans Update()
	}

	private void Waypoints()
	{
		Vector3 waypointPos = WaypointManager.Instance.GetWaypointPos(_waypoint);			// position du current waypoint
		Vector3 nextwaypointPos = WaypointManager.Instance.GetWaypointPos(_waypoint + 1);	// position du next waypoint

		//Debug.Log($"waypointPos: {waypointPos}");
		//Debug.Log($"nextwaypointPos: {nextwaypointPos}");

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
			_waypoint++;
			if (WaypointManager.Instance.IsFinalWaypoint(_waypoint)) {
				_waypoint = 0;
			}
		}
	}

	public void SetWaypoint(int waypoint)
	{
		_waypoint = waypoint;
	}

	public int GetWaypoint()
	{
		return _waypoint;
	}
}
