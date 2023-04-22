using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Fields
    private Rigidbody Character;
    private Vector3 UpdatedMovementVector;
    private float PreviousStep;
    private float Speed;
    private bool Crouched;
    private int RunModifier;
    [SerializeField]
    private LayerMask GroundLayer;

    #endregion
    #region Unity Standard Methods
    void Start()
    {
        Character = GetComponent<Rigidbody>();
    }
    #endregion
    #region Public Methods
    public void UpdateMovement(Vector3 inputVector)
    {
        Vector3 verticalVelocity = new Vector3(0, Character.velocity.y, 0);
        Vector3 movementVector = (Character.rotation * inputVector);

        if (!grounded)
        {
            movementVector.x = movementVector.x / 2;
        }
        UpdatedMovementVector = movementVector; // + verticalVelocity;
        //UpdatedMovementVector = movementVector - Character.velocity;
        //Character.AddForce(movementVector - Character.velocity, ForceMode.VelocityChange);
    }
    #endregion

    #region Unfinished Code

    private bool grounded;
    public float maxStepHeight;
    public float stepSearchOvershoot = 0.01f;

    private List<ContactPoint> allCps = new List<ContactPoint>();

    private Vector3 lastVelocity;


    private void FixedUpdate()
    {
        Vector3 velocity;
        ContactPoint groundCp = default(ContactPoint);
        grounded = FindGround(out groundCp, allCps);

        Vector3 stepUpOffset = default(Vector3);
        bool stepUp = false;
        if (grounded)
        {
            Character.AddForce(UpdatedMovementVector - new Vector3(Character.velocity.x, 0, Character.velocity.z), ForceMode.VelocityChange);
            velocity = Character.velocity;
            stepUp = FindStep(out stepUpOffset, allCps, groundCp, velocity);
            Debug.DrawRay(transform.position, Vector3.down * (transform.localScale.y + 0.1f), Color.green);
        }
        else
        {
            velocity = lastVelocity;
            Debug.DrawRay(transform.position, Vector3.down * (transform.localScale.y + 0.1f), Color.red);
        }

        if (stepUp)
        {
            Character.position += stepUpOffset;
            Character.velocity = lastVelocity;
        }
        else
        {
            if (Character.velocity.y > 0 && !grounded) Character.velocity = new Vector3(Character.velocity.x, 0, Character.velocity.z);
        }
        allCps.Clear();
        lastVelocity = velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        allCps.AddRange(collision.contacts);
    }
    private void OnCollisionStay(Collision collision)
    {
        allCps.AddRange(collision.contacts);
    }
    /// Finds the MOST grounded (flattest y component) ContactPoint
    /// \param allCPs List to search
    /// \param groundCP The contact point with the ground
    /// \return If grounded
    private bool FindGround(out ContactPoint groundCp, List<ContactPoint> allCps)
    {
        groundCp = default(ContactPoint);
        bool found = false;
        foreach (ContactPoint contactPoint in allCps)
        {
            if (contactPoint.normal.y > 0.01f && (found == false || contactPoint.normal.y > groundCp.normal.y))
            {
                groundCp = contactPoint;
                found = true;
            }
        }
        Debug.Log(groundCp.normal.y + found.ToString());
        return found;
    }

    /// Find the first step up point if we hit a step
    /// \param allCPs List to search
    /// \param stepUpOffset A Vector3 of the offset of the player to step up the step
    /// \return If we found a step
    private bool FindStep(out Vector3 stepUpOffset, List<ContactPoint> allCps, ContactPoint groundCp, Vector3 currentVelocity)
    {
        stepUpOffset = default(Vector3);

        Vector2 velocityXZ = new Vector2(currentVelocity.x, currentVelocity.z);
        if (velocityXZ.sqrMagnitude < 0.0001f) return false;

        foreach (ContactPoint contactPoint in allCps)
        {
            bool test = ResolveStepUp(out stepUpOffset, contactPoint, groundCp);
            if (test) return test;
        }
        return false;
    }

    /// Takes a contact point that looks as though it's the side face of a step and sees if we can climb it
    /// \param stepTestCP ContactPoint to check.
    /// \param groundCP ContactPoint on the ground.
    /// \param stepUpOffset The offset from the stepTestCP.point to the stepUpPoint (to add to the player's position so they're now on the step)
    /// \return If the passed ContactPoint was a step
    private bool ResolveStepUp(out Vector3 stepUpOffset, ContactPoint stepTestCp, ContactPoint groundCp)
    {
        stepUpOffset = default(Vector3);
        Collider stepCollider = stepTestCp.otherCollider;

        if (Mathf.Abs(stepTestCp.normal.y) >= 0.01f) return false;
        if (!(stepTestCp.point.y - groundCp.point.y < maxStepHeight))
        {
            return false;
        }

        RaycastHit hitInfo;
        float stepHeight = groundCp.point.y + maxStepHeight + 0.0001f;
        Vector3 stepTestInverseDirection = new Vector3(-stepTestCp.normal.x, 0, -stepTestCp.normal.z).normalized;
        Vector3 origin = new Vector3(stepTestCp.point.x, stepHeight, stepTestCp.point.z) + (stepTestInverseDirection * stepSearchOvershoot);
        Vector3 direction = Vector3.down;
        if (!(stepCollider.Raycast(new Ray(origin, direction), out hitInfo, maxStepHeight))) return false;

        Vector3 stepUpPoint = new Vector3(stepTestCp.point.x, hitInfo.point.y + 0.0001f, stepTestCp.point.z) + (stepTestInverseDirection * stepSearchOvershoot);
        Vector3 stepUpPointOffset = stepUpPoint - new Vector3(stepTestCp.point.x, groundCp.point.y, stepTestCp.point.z);

        stepUpOffset = stepUpPointOffset;
        return true;
    }

    #endregion
}
