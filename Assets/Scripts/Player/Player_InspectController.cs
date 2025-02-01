using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_InspectController : MonoBehaviour
{
    #region References

    // Player
    FirstPersonController firstPersonController;
    private GameObject playerGameObject;
    private Vector3 playerPosition;
    public Transform playerCamera;
    public Transform inspectObjectLocation;

    // Pickup
    private GameObject PickupFocusObject;
    private GameObject InspectingObject;

    #endregion

    #region Variables

    public bool isEquipped { get; set; }
    public bool isInspecting { get; set; }

    public bool CanPickup { get => CheckPickupable(); }
    public bool DetectInput
    {
        get
        {
            if (inputDelay > 0.0f) { inputDelay -= Time.deltaTime; return false; }
            else { inputDelay = 0.1f; return Input.GetMouseButton(0); }
        }
    }

    [SerializeField] float _pickupDistance = 3;
    public float PickupDistance { get => _pickupDistance; }

    float inputDelay;

    #endregion

    void Start()
    {
        InitializePlayerReferences();
    }
    void Update()
    {
        //if (isEquipped == false) { return; }

        TryHandleObjectHighlighting();
        TryPickupObject();

        ChangeObjectsLayer();

        if (isInspecting) { InspectingObject.transform.position = inspectObjectLocation.position; }
        if (isInspecting)
        {
            float RotateSensitivity = 2.0f;
            float RotateAmount = 0;

            if (Input.GetKey(KeyCode.E)) { RotateAmount = 1.0f; }
            else if (Input.GetKey(KeyCode.Q)) { RotateAmount = -1.0f; }
            else { RotateAmount = 0.0f; }

            if (Mathf.Abs(RotateAmount) > 0.1f)
            {
                Vector3 newRotation = InspectingObject.transform.rotation.eulerAngles;
                newRotation.y += RotateAmount * RotateSensitivity;

                InspectingObject.transform.rotation = Quaternion.Euler(newRotation);
            }
        }

        ThrowObject();
    }

    void InitializePlayerReferences()
    {
        // Get player
        playerGameObject = gameObject;
        playerPosition = playerGameObject.transform.position;

        // Assign first person controller
        firstPersonController = GetComponent<FirstPersonController>();
    }
    void TryHandleObjectHighlighting()
    {
        if (isInspecting == true) { return; }
    }
    void TryPickupObject()
    {
        if (isInspecting == true) { return;}
        if (DetectInput == false) { return; }
        if (CanPickup == false) { return; }
        InspectingObject = PickupFocusObject;
        InspectingObject.transform.rotation = inspectObjectLocation.rotation;
        if (InspectingObject.GetComponent<Rigidbody>() != null) { InspectingObject.GetComponent<Rigidbody>().isKinematic = true; }
        isInspecting = true;
    }
    void ChangeObjectsLayer(bool isReset = false)
    {
        if (isInspecting == false) { return; }

        int layerToChange = (isReset) ? 0 : 5;

        InspectingObject.layer = layerToChange;
        foreach (Transform child in InspectingObject.transform)
        {
            child.gameObject.layer = layerToChange;
        }
    }
    void ThrowObject()
    {
        if (isInspecting == false) { return; }
        if (DetectInput == false) { return;}

        InspectingObject.transform.rotation = playerGameObject.transform.rotation;
        float offset = InspectingObject.GetComponent<Collider>().bounds.extents.z;
        Vector3 direction = playerGameObject.transform.forward;
        InspectingObject.transform.position = playerGameObject.transform.position + direction * offset;
        if (InspectingObject.GetComponent<Rigidbody>() != null) { InspectingObject.GetComponent<Rigidbody>().isKinematic = false; }
        ChangeObjectsLayer(true);
        InspectingObject = null;
        isInspecting = false;
    }
    bool CheckPickupable()
    {
        Vector3 origin = playerCamera.position;
        Vector3 direction = playerCamera.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, PickupDistance))
        {
            Debug.DrawRay(origin, direction * PickupDistance, Color.red);

            if (hit.collider.gameObject.GetComponent<IPickupable>() != null)
            {
                PickupFocusObject = hit.collider.gameObject;
                return true;
            }
            else
            {
                PickupFocusObject = null;
                return false;
            }
        }
        else
        {
            PickupFocusObject = null;
            return false;
        }
    }
}
