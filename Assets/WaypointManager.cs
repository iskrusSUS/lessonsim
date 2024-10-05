using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public Transform[] waypoints;  // Массив точек для движения

    public Transform GetWaypoint(int index)
    {
        if (index >= 0 && index < waypoints.Length)
        {
            return waypoints[index];
        }

        return null;
    }

    public int GetWaypointCount()
    {
        return waypoints.Length;
    }
}
