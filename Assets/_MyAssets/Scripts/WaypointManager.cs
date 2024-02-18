using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
	public static WaypointManager Instance; // Singleton

	[SerializeField] private Transform _parentWaypoint; // l'object parent des waypoints

	private List<Vector3> _listWaypoint = new List<Vector3>();

	void Awake()
	{
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(this.gameObject);
		}

		// trouve tout les waypoints
		foreach (Transform waypoint in _parentWaypoint) {
			waypoint.GetComponent<MeshRenderer>().enabled = false;
			_listWaypoint.Add(waypoint.position);
		}
	}

	// donner le membre _waypoint du spaceship par argument pour obtenir sa position
	public Vector3 GetWaypointPos(int index)
	{
		if(index >= _listWaypoint.Count) {
			return _listWaypoint[0];
		} else {
			return _listWaypoint[index];
		}
	}

	// pour savoir si c'est le waypoint final, si oui ça reset à 0 dans la classe spaceship
	public bool IsFinalWaypoint(int index)
	{
		if(index == _listWaypoint.Count) {
			return true;
		} else {
			return false;
		}
	}

	// pour le hud
	public int GetLapNb()
	{
		return _listWaypoint.Count;
	}
}