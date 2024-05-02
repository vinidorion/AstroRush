using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbite : MonoBehaviour
{
    private List<Transform> _listOrbite = new List<Transform>();
    [SerializeField] private Transform _parentOrbite; // l'object parent des orbites
    public static Orbite Instance; // Singleton

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        // trouve tous les waypoints
        foreach (Transform waypoint in _parentOrbite)
        {
            waypoint.GetComponent<MeshRenderer>().enabled = false;
            _listOrbite.Add(waypoint);
        }
    }

    public Vector3 GetOrbitePos(int index)
    {
        return _listOrbite[index].position;
    }

    public int GetListCount() { return _listOrbite.Count;}

    public float GetOrbiteScale(int index) { return _listOrbite[index].localScale.x; }
}
