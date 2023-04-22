using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting.ReorderableList;

public class PathfinderComponent : MonoBehaviour
{
    private NavMeshAgent agent;
    private Queue<Transform> waypoints;
    [SerializeField]
    private GameObject WaypointManager;
    [SerializeField]
    private int MaxNumDistractions = 4;
    [SerializeField]
    private float SECONDS_PER_DISTRACTION = 2.0f;
    [SerializeField]
    private bool f_inRange;
    private bool f_distracted;
    private bool f_targetInSight;
    private bool f_moving;
    private bool f_alerted;
    public bool Alerted
    {
        get
        {
            return f_alerted;
        }
        set
        {
            f_alerted = true;
            f_rotationSpeed = 10f;
            agent.speed = 8;
        }
    }

    private Vector2 f_originalDirection;
    private float f_partialRotation;
    private float f_rotationSpeed = 100f;
    private float f_targetAngle;
    private float f_timeSinceLastCheck = 0;
    private int f_timesDistracted;
    [Range(1.0f, 10.0f)]
    public int f_discipline;

    #region Unity standard functions
    // Start is called before the first frame update
    void Start()
    {
        // Initialise
        agent = transform.GetComponent<NavMeshAgent>();
        waypoints = new Queue<Transform>(WaypointManager.GetComponent<WaypointManager>().nodes);
        InitialiseRotationVectors();
    }
    // Update is called once per frame
    void Update()
    {
        if (f_moving) return;
        UpdateRotation();

        if (Alerted) return;
        // Distraction routine
        // We want the guard to spend more time looking in it's scripted direction. Half the length we wait to reset back.
        if ((f_timeSinceLastCheck += Time.deltaTime) > SECONDS_PER_DISTRACTION / (f_timesDistracted % 2 + 1))
        {
            if (f_timesDistracted == MaxNumDistractions)
            {
                GoToNextWaypoint();
                f_timesDistracted = 0;
                return;
            }
            Distract();
            f_timesDistracted++;
        }
    }
    #endregion
    #region Private functions
    private void InitialiseRotationVectors()
    {
        f_originalDirection = transform.localRotation.eulerAngles;
        f_partialRotation = f_originalDirection.y;
        f_targetAngle = f_originalDirection.y;
    }
    private void UpdateRotation()
    {
        f_partialRotation = Mathf.LerpAngle(f_partialRotation, f_targetAngle, 1f / f_rotationSpeed);
        Quaternion yRotation = Quaternion.AngleAxis(f_partialRotation, Vector3.up);
        transform.localRotation = yRotation;
    }
    private void GoToNextWaypoint()
    {
        f_moving = true;
        Transform nextWaypoint = waypoints.Dequeue();
        agent.SetDestination(nextWaypoint.position);
        waypoints.Enqueue(nextWaypoint);
    }
    private void Distract()
    {
        f_timeSinceLastCheck = 0;
        if (!f_distracted)
        {
            if (Random.value * 10 > f_discipline)
            {
                float yComponent = Random.Range(-180 + (45 / 2), 180 - (45 / 2));

                f_targetAngle = yComponent;
                f_distracted = true;
            }
        }
        else
        {
            f_targetAngle = f_originalDirection.y;
            f_distracted = false;
        }
    }
    #endregion
    #region Public functions
    public void WaypointReached(Vector3 signalPosition, Transform OrientToPoint)
    {
        float distanceLeft = (signalPosition - agent.destination).sqrMagnitude;
        if (distanceLeft > 0.15) return; // Not our destiantion, ignore ping.

        // Determine where enemy is meant to look..
        Vector3 orientation = OrientToPoint.position - transform.position;
        LookToward(orientation);

        // Reinitialise vars for distraction routine
        f_originalDirection = new Vector2(f_originalDirection.x, f_targetAngle);
        f_timeSinceLastCheck = 0;
        f_moving = false;
    }
    public void GoToLocation(Vector3 location)
    {
        agent.SetDestination(location);
    }
    public void LookToward(Vector3 orientation)
    {
        f_targetAngle = Vector3.Angle(Vector3.forward, orientation);
        if (orientation.x < 0) f_targetAngle *= -1;
    }

    public void InvestigateSound(Vector3 position)
    {
        if (f_targetInSight) return; // Already found the source..
        if (f_inRange) return; // No need to close in anymore..

        f_targetAngle = Vector3.Angle(Vector3.forward, position - transform.position);
        agent.SetDestination(position);
    }
    #endregion

}
