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

		// trouve tous les waypoints
		foreach (Transform waypoint in _parentWaypoint) {
			waypoint.GetComponent<MeshRenderer>().enabled = false;
			_listWaypoint.Add(waypoint.position);
		}
	}

	// méthode publique qui permet d'obtenir la position d'un waypoint
	// (donner le numéro du waypoint par argument pour obtenir sa position)
	public Vector3 GetWaypointPos(int index)
	{
		if(index >= _listWaypoint.Count) {
			return _listWaypoint[0];
		} else {
			return _listWaypoint[index];
		}
	}

	// méthode publique qui sert à savoir si c'est le waypoint final
	// (si oui ça reset à 0 dans la classe spaceship)
	public bool IsFinalWaypoint(int index)
	{
		return index >= _listWaypoint.Count;
	}

	// méthode publique qui retourne le nombre de waypoint total
	public int GetNbWpt()
	{
		return _listWaypoint.Count;
	}
}