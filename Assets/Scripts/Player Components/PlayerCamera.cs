using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    #region Fields
    // Range collider representing the player's "reach"
    private RangeCollider interactionReach;
    // Debug/Release Switch for locking mouse cursor in game window
    [SerializeField]
    private bool f_lockCursor;
    private Vector2 f_smoothMouse;
    private Vector2 f_absoluteMouse;
    private Vector2 f_clampInDegrees = new(360, 180);
    #endregion
    #region Properties
    public Vector2 mouseDelta;
    // Reference to player for positioning and accessing PlayerController public methods
    public Transform PlayerTransform;
    // Reference to the item the player is looking at (may be null)
    public GameObject FocusedWorldItem;
    #endregion
    #region Unity Standard Methods
    void Start()
    {
        interactionReach = GetComponentInChildren<RangeCollider>();
        interactionReach.onTriggerEnter_Action += InteractionReachEnter;
        interactionReach.onTriggerStay_Action += InteractionReachStay;
        interactionReach.onTriggerExit_Action += InteractionReachExit;
    }
    void Update()
    {
        if (f_lockCursor) Cursor.lockState = CursorLockMode.Locked; // Final mode
        else Cursor.lockState = CursorLockMode.None; // DEV mode

        ApplyMouseSmoothing();
        f_absoluteMouse.x = ClampMouse(f_clampInDegrees.x, f_absoluteMouse.x);

        Quaternion xRotation = Quaternion.AngleAxis(-f_absoluteMouse.y, Vector3.right);
        transform.localRotation = xRotation;

        f_absoluteMouse.y = ClampMouse(f_clampInDegrees.y, f_absoluteMouse.y);

        Quaternion yRotation = Quaternion.AngleAxis(f_absoluteMouse.x, Vector3.up);
        PlayerTransform.localRotation = yRotation;

    }
    private void LateUpdate()
    {
        // Keep camera attached to the player, at the top of the player object.
        transform.position = PlayerTransform.position + Vector3.up * PlayerTransform.localScale.y / 2;
    }
    #endregion
    #region Public Methods
    public GameObject GetFocusedWorldItem()
    {
        if (FocusedWorldItem != null)
        {
            GameObject item = FocusedWorldItem.GetComponent<Interactable>().Interaction();
            if (item != null)
            {
                return item;
            }
        }
        return null;
    }
    #endregion
    #region Private Methods
    private void ApplyMouseSmoothing()
    {
        Vector2 mouseSmoothing = new(2f, 2f);

        f_smoothMouse.x = Mathf.Lerp(f_smoothMouse.x, mouseDelta.x, 1f / mouseSmoothing.x);
        f_smoothMouse.y = Mathf.Lerp(f_smoothMouse.y, mouseDelta.y, 1f / mouseSmoothing.y);

        // Accumulate smoothed mouse movement over several frames
        f_absoluteMouse += f_smoothMouse;
    }
    private float ClampMouse(float clampValue, float mouseMovement)
    {
        if (clampValue < 360)
        {
            return Mathf.Clamp(mouseMovement, -clampValue * 0.5f, clampValue * 0.5f);
        }
        return mouseMovement;
    }
    #endregion
    #region Delegate Methods
    private void InteractionReachEnter(Collider other) { }
    private void InteractionReachStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            if (FocusedWorldItem == null) FocusedWorldItem = other.gameObject;
            other.transform.GetComponent<Interactable>().Highlight();
        }
    }
    private void InteractionReachExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            if (FocusedWorldItem != null) FocusedWorldItem = null;
            other.transform.GetComponent<Interactable>().Nolight();
        }
    }
    #endregion
}
