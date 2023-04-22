using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    private Transform LookAtPoint;
    // Start is called before the first frame update
    void Start()
    {
        LookAtPoint = transform.Find("LookAtPoint");

        RangeCollider WaypointTrigger = GetComponent<RangeCollider>();
        WaypointTrigger.onTriggerEnter_Action += WaypointTrigger_TriggerEnter;
    }

    private void WaypointTrigger_TriggerEnter(Collider other)
    {
        PathfinderComponent OtherAI;
        if (other.transform.TryGetComponent<PathfinderComponent>(out OtherAI))
        {
            OtherAI.WaypointReached(transform.position, LookAtPoint);
        }
    }
}
