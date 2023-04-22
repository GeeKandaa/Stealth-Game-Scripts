using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using System.Linq;
using UnityEngine.Windows.Speech;

public class DetectionComponent : MonoBehaviour
{
    [SerializeField]
    public LayerMask f_ignoreLayer;
    private PathfinderComponent pathfinder;
    private SpeechController speechComponent;

    private bool f_inRange;
    private bool f_targetInSight;

    // Sense colliders are assigned on seperate child GameObjects of this GameObject
    private Transform VisualTransform;
    private RangeCollider VisualCollider;
    private Transform CombatRange;
    private RangeCollider CombatCollider;

    #region Unity standard functions
    // Start is called before the first frame update
    void Start()
    {
        pathfinder = GetComponent<PathfinderComponent>();
        speechComponent = GetComponentInChildren<SpeechController>();
        // ignore layer is actually everything but ignore layer..
        f_ignoreLayer = ~f_ignoreLayer;

        // Connect collider for AI "vision"
        VisualTransform = transform.Find("RangeVision");
        VisualCollider = VisualTransform.GetComponent<RangeCollider>();
        VisualCollider.onTriggerEnter_Action += VisualCollider_TriggerEnter;
        VisualCollider.onTriggerStay_Action += VisualCollider_TriggerStay;
        VisualCollider.onTriggerExit_Action += VisualCollider_TriggerExit;

        // Connect Collider for AI combat reach.
        CombatRange = transform.Find("RangeMelee");
        CombatCollider = CombatRange.GetComponent<RangeCollider>();
        CombatCollider.onTriggerEnter_Action += CombatCollider_TriggerEnter;
        CombatCollider.onTriggerStay_Action += CombatCollider_TriggerStay;
        CombatCollider.onTriggerExit_Action += CombatCollider_TriggerExit;
    }
    #endregion
    #region Delegate functions
    private void VisualCollider_TriggerEnter(Collider other) { }
    private void VisualCollider_TriggerStay(Collider other)
    {
        // Raycast in case something is blocking sight of Player..
        if (Physics.Raycast(transform.position, (other.transform.position - transform.position).normalized, out RaycastHit hit, VisualTransform.GetComponent<SphereCollider>().radius * 2, f_ignoreLayer))
        {
            if (CanSeePlayer(hit))
            {
                pathfinder.Alerted = true;
                speechComponent.Status = SpeechController.speechStatus.Chasing;
                ChasePlayer(hit);
            }
            else
            {
                //Debug.DrawRay(transform.position, (other.transform.position - transform.position).normalized * vTransform.GetComponent<SphereCollider>().radius * 2, Color.blue);
                f_targetInSight = false;
            }
        }
        else
        {
            f_targetInSight = false;
        }
    }
    private void VisualCollider_TriggerExit(Collider other) { }
    private void CombatCollider_TriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return; // not the player, ignore it
        if (!pathfinder.Alerted) return; // player has not been spotted yet..

        // The player is in range and the guard is in chase. Stop moving and start combat.
        f_inRange = true;
        pathfinder.GoToLocation(transform.position);
    }
    private void CombatCollider_TriggerStay(Collider other)
    {
        // Combat...
    }
    private void CombatCollider_TriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) f_inRange = false;
        // don't set destination, this should be handled by vCollider or investigateSound()
    }
    #endregion
    #region Private functions
    private bool CanSeePlayer(RaycastHit hit)
    {
        if (hit.transform.CompareTag("Player"))
        {
            float maximumVisualDistance = VisualTransform.GetComponent<SphereCollider>().radius * 2;
            //Debug.DrawRay(transform.position, (hit.transform.position - transform.position).normalized * maximumVisualDistance, Color.yellow);

            // Calculate whether player is seen
            float distanceModifier = maximumVisualDistance / hit.distance;
            float targetVisibility = distanceModifier * hit.transform.GetComponent<PlayerVisibility>().Visibility; ;
            // If guard has been alerted it should be easier to spot player.
            if (pathfinder.Alerted) targetVisibility *= 2;

            Debug.Log("Check = " + targetVisibility);
            if ((targetVisibility >= 1 || f_targetInSight)) return true;
        }
        return false;
    }
    private void ChasePlayer(RaycastHit hit)
    {
        f_targetInSight = true;

        Vector3 direction = hit.transform.position - transform.position;

        pathfinder.LookToward(direction);

        if (f_inRange) return; // if we are in attack range, we do not need to close in anymore
        pathfinder.GoToLocation(hit.transform.position);
    }
    #endregion
}
