using System;
using UnityEngine;

public class PlayerFXController : MonoBehaviour
{
    [Serializable]
    public class FXEntry
    {
        public string id;                          // Unique identifier for this effect
        public GameObject fxPrefab;                // Prefab to spawn for this effect
        public float scaleMultiplier = 1f;         // Scale factor for the effect
        public Vector3 offsetPosition = Vector3.zero; // Offset from the calculated position
        public Quaternion offsetRotation = Quaternion.identity; //default rot
        public float lifeSpan = 1f;                // Duration before the effect is destroyed
        public bool spawnAtFeet = false;           // Toggle to spawn FX at feet
        public bool spawnAtToolFloor = false;      // Toggle to spawn FX at tool floor level
    }

    // FX entries categorized for easier Inspector navigation
    [Header("Movement FX Entries")]
    [SerializeField] private FXEntry[] movementFXEntries;

    [Header("Digging FX Entries")]
    [SerializeField] private FXEntry[] diggingFXEntries;

    [Header("Detecting FX Entries")]
    [SerializeField] private FXEntry[] detectingFXEntries;

    private CapsuleCollider playerCollider;

    private void Start()
    {
        EventManager.current.onSpawnFX += SpawnFX;

        // Get the CapsuleCollider for player height reference if available
        playerCollider = GetComponent<CapsuleCollider>();
        if (playerCollider == null)
        {
            Debug.LogWarning("CapsuleCollider not found on player. 'spawnAtFeet' option may not work as expected.");
        }
    }

    private void OnDestroy()
    {
        EventManager.current.onSpawnFX -= SpawnFX;
    }

    // Method to spawn the FX based on ID
    public void SpawnFX(string id)
    {
        // Search across all FX categories for a matching ID
        FXEntry fxEntry = FindFXEntryById(id);

        if (fxEntry == null || fxEntry.fxPrefab == null)
        {
            Debug.LogWarning($"FX with ID '{id}' not found or has no assigned prefab.");
            return;
        }

        // Calculate the spawn position
        Vector3 spawnPosition = transform.position + fxEntry.offsetPosition;
        Quaternion spawnRotation = fxEntry.offsetRotation;

        // Adjust the position to spawn at feet if toggle is enabled and player has a CapsuleCollider
        if (fxEntry.spawnAtFeet && playerCollider != null)
        {
            spawnPosition -= new Vector3(0, playerCollider.height / 2, 0);
        }

        // Apply hardcoded tool floor offset on Z-axis if spawnAtToolFloor is true
        if (fxEntry.spawnAtToolFloor)
        {
            float toolOffset = 2f; // Fixed Z-axis offset
            spawnPosition += transform.forward * toolOffset;
        }

        // Instantiate the FX object
        GameObject instantiatedFX = Instantiate(fxEntry.fxPrefab, spawnPosition, spawnRotation);

        // Apply the scale multiplier
        instantiatedFX.transform.localScale = Vector3.one * fxEntry.scaleMultiplier;

        // Adjust scale specifically for particle systems if they exist on this object
        ParticleSystem particleSystem = instantiatedFX.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            var mainModule = particleSystem.main;
            mainModule.startSizeMultiplier *= fxEntry.scaleMultiplier;
        }

        // Destroy after the defined lifespan
        Destroy(instantiatedFX, fxEntry.lifeSpan);
    }

    // Helper method to find the FXEntry by ID across all categories
    private FXEntry FindFXEntryById(string id)
    {
        // Search in Movement FX Entries
        foreach (var fxEntry in movementFXEntries)
        {
            if (fxEntry.id == id) return fxEntry;
        }

        // Search in Digging FX Entries
        foreach (var fxEntry in diggingFXEntries)
        {
            if (fxEntry.id == id) return fxEntry;
        }

        // Search in Detecting FX Entries
        foreach (var fxEntry in detectingFXEntries)
        {
            if (fxEntry.id == id) return fxEntry;
        }

        return null; // Return null if no matching FX entry is found
    }
}
