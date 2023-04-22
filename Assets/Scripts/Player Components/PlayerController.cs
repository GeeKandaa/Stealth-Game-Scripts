using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    #region Fields
    #region Player Components
    PlayerMovement MoveComponent;
    PlayerNoise NoiseComponent;
    PlayerInventory InventoryComponent;
    PlayerUI UIComponent;
    PlayerCamera CameraComponent;
    #endregion
    private Vector3 LastStepPosition;
    private float DefaultSpeed = 300;
    [SerializeField]
    private float Speed;

    private Vector2 f_inputVector;
    private bool f_crouched;
    private int RunModifier = 1;
    #endregion
    #region Unity Standard Methods
    void Start()
    {
        MoveComponent = GetComponent<PlayerMovement>();
        NoiseComponent = GetComponent<PlayerNoise>();
        InventoryComponent = GetComponent<PlayerInventory>();
        UIComponent = GetComponent<PlayerUI>();
        CameraComponent = GetComponentInChildren<PlayerCamera>();

        Speed = DefaultSpeed;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        float ModifiedSpeed = Speed * RunModifier;
        // Move the player
        Vector3 inputVector = new Vector3(f_inputVector.x, 0, f_inputVector.y) * ModifiedSpeed * Time.fixedDeltaTime;
        MoveComponent.UpdateMovement(inputVector);

        // Determine if we've moved enough to warrant a footstep
        if ((transform.position - LastStepPosition).magnitude > 5f)
        {
            // We have moved a step distance, calculate noise level and make the noise.
            float VolumeModifier = f_inputVector.magnitude * ModifiedSpeed / DefaultSpeed;
            NoiseComponent.MakeNoise(VolumeModifier, transform.position);
            // Reset last step position
            LastStepPosition = transform.position;
        }
    }
    #endregion
    #region Player Input
    public void OnMovement(InputValue input)
    {
        f_inputVector = input.Get<Vector2>();
        if (f_inputVector.magnitude > 1) f_inputVector.Normalize();
    }
    public void OnRun(InputValue input)
    {
        RunModifier = input.isPressed ? 2 : 1;
    }
    public void OnCrouch(InputValue input)
    {
        if (f_crouched)
        {
            float SizeShift = transform.localScale.y;

            transform.position += Vector3.up * SizeShift;
            transform.localScale += Vector3.up * SizeShift;
            Speed = Speed * 2;
        }
        else
        {
            float SizeShift = transform.localScale.y / 2;

            transform.position -= Vector3.up * SizeShift;
            transform.localScale -= Vector3.up * SizeShift;
            Speed = Speed / 2;
        }
        f_crouched = !f_crouched;
    }
    void OnNextInventoryItem(InputValue input)
    {
        if (input.isPressed == true)
        {
            InventoryComponent.SelectNextItem();
            UIComponent.UpdateInventory();
        }
    }
    void OnPreviousInventoryItem(InputValue input)
    {
        if (input.isPressed == true)
        {
            InventoryComponent.SelectPreviousItem();
            UIComponent.UpdateInventory();
        }
    }
    public void OnInteract(InputValue input)
    {
        GameObject item = CameraComponent.GetFocusedWorldItem();
        if (item != null) InventoryComponent.StoreItem(item);
    }
    public void OnLookAround(InputValue input)
    {
        // Eventually we want to override this using some kind of end user settings menu.
        Vector2 f_mouseSensitivity = new(0.5f, 0.5f);
        Vector2 mouseDelta = input.Get<Vector2>();
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(f_mouseSensitivity.x, f_mouseSensitivity.y));

        // Update camera
        CameraComponent.mouseDelta = mouseDelta;
    }
    #endregion
}