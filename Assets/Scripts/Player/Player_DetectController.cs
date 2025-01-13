using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_DetectController : MonoBehaviour
{
    #region References

    // Player
    FirstPersonController firstPersonController;
    private GameObject playerGameObject;
    private Vector3 playerPosition;
    public Transform playerCamera;

    #endregion

    #region Variables

    //Detecting
    public bool isEquipped = false;
    public bool isDetecting = false;

    //circle cast parameters to detect layer collision with items
    public float radius = 1f;          // Radius of the circle
    public float maxDistance = 10f;   // Maximum distance of the cast
    public LayerMask collisionLayer;  // Layer mask for filtering collisions

    // Detect Cooldown
    public float detectCooldown = 0.2f; // Time between detects
    private float detectCooldownTimer = 0f;

    #endregion

    void Start()
    {
        // Get player
        playerGameObject = gameObject;
        playerPosition = playerGameObject.transform.position;

        // Assign first person controller
        firstPersonController = GetComponent<FirstPersonController>();

        //need to do a 3d circle cast to detect objects within radius
    }

    void Detect()
    {
        // Origin of the cast
        Vector3 origin = transform.position;

        // Direction of the cast (e.g., forward)
        Vector3 direction = transform.forward;

        // Store hit info
        RaycastHit hit;

        // Perform the sphere cast
        if (Physics.SphereCast(origin, radius, direction, out hit, maxDistance, collisionLayer))
        {
            // Log the name of the object hit
            Debug.Log($"Hit: {hit.collider.name}");

            // Perform your collision logic here
        }
        else
        {
            Debug.Log("No collision detected.");
        }

        // Debug visualization in Scene view
        Debug.DrawRay(origin, direction * maxDistance, Color.red);
    }

    void DetectObjects()
    {
        // Origin of the sphere
        Vector3 origin = transform.position;

        // Perform the overlap sphere
        Collider[] hitColliders = Physics.OverlapSphere(origin, radius, collisionLayer);

        foreach (var collider in hitColliders)
        {
            Debug.Log($"Detected object: {collider.name}");

            // Perform your collision logic here
        }
    }

    void Update()
    {
        if (isEquipped)
        {
            detectCooldownTimer -= Time.deltaTime;

            if (Input.GetMouseButton(0) && detectCooldownTimer <= 0f)
            {
                Detect();

                //change the below line
                //EventManager.current.SpawnFX("shovelFX"); // Trigger dig event with "shovelFX" ID

                detectCooldownTimer = detectCooldown;
            }
        }
    }
}