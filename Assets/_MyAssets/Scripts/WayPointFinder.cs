using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointFinder : MonoBehaviour
{
    private int wayPoint;

    // Update is called once per frame
    void Update()
    {
        Waypoints();
        SendInfo();
    }

    private void Waypoints()
    {
        Vector3 waypointPos = WaypointManager.Instance.GetWaypointPos(wayPoint);           // position du current waypoint
        Vector3 nextwaypointPos = WaypointManager.Instance.GetWaypointPos(wayPoint + 1);   // position du next waypoint

        //Debug.Log("waypointPos: " + waypointPos);
        //Debug.Log("nextwaypointPos: " + nextwaypointPos);

        // pour visualiser à quel waypoint le spaceship est rendu
        Debug.DrawLine(transform.position, waypointPos, Color.green, Time.fixedDeltaTime);
        Debug.DrawLine(transform.position, nextwaypointPos, Color.blue, Time.fixedDeltaTime);

        // sqrt inutile ici, on compare deux distances
        float distCurrWaypoint = (transform.position - waypointPos).sqrMagnitude;
        float distNextWaypoint = (transform.position - nextwaypointPos).sqrMagnitude;

        if (distNextWaypoint < distCurrWaypoint)
        {
            wayPoint++;
            if (WaypointManager.Instance.IsFinalWaypoint(wayPoint))
            {
                wayPoint = 0;
            }
        }
    }

    private void SendInfo()
    {
        if (gameObject.GetComponent<SpaceShip>() == null)
        {
            gameObject.GetComponent<Projectile>().SetWaypoint(wayPoint);
        }
        else
        {
            gameObject.GetComponent<SpaceShip>().SetWaypoint(wayPoint);
        }
    }
}
