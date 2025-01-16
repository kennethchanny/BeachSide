using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigController : MonoBehaviour
{
    #region References
    // Player
    FirstPersonController firstPersonController;
    private GameObject playerGameObject;
    private Vector3 playerPosition;
    public Transform playerCamera;

    // Terrain
    private TerrainData terrainData;
    private TerrainCollider terrainCollider;

    // Terrain Layer Index
    public TerrainLayer digLayer;
    int digLayerIndex = 1; // Set this to the index of your digging layer in the Unity Editor
    #endregion

    #region Variables
    // Digging
    private Vector3 hitPoint;
    public bool isDigging = false;
    public bool canDig = false;
    public bool isEquipped = false;
    public float diggableDistance = 3;
    public float paintStrength = 1.5f;
    public float radius = 2f; // The radius of the area to affect
    public float digDepth = 2f; // How deep to dig
    private float digMultiplier = 0.0001f; // Dig multiplier
    public float pileRange = 1.5f;
    public float digCooldown = 0.2f; // Time between digs
    private float digCooldownTimer = 0f;
    #endregion

    void Start()
    {
        // Get player
        playerGameObject = gameObject;
        playerPosition = playerGameObject.transform.position;

        // Assign first person controller
        firstPersonController = GetComponent<FirstPersonController>();

        // Assign terrain refs from TerrainInitializer
        TerrainInitializer terrainInitializer = FindObjectOfType<TerrainInitializer>();
        terrainData = terrainInitializer.GetTerrainData();
        terrainCollider = terrainInitializer.GetTerrainCollider();

        // Check if digLayerIndex is within bounds
        if (digLayerIndex < 0 || digLayerIndex >= terrainData.terrainLayers.Length)
        {
            Debug.LogError("Invalid digLayerIndex. Please ensure it's within the range of available terrain layers.");
        }
    }

    void CheckEquipped()
    {
        if (isEquipped == false)
        {
            return;
        }
    }

    void CheckDiggable()
    {
        Vector3 origin = playerCamera.position;
        Vector3 direction = playerCamera.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, diggableDistance))
        {
            Debug.DrawRay(origin, direction * diggableDistance, Color.red);

            if (hit.collider.gameObject == terrainCollider.gameObject)
            {
                canDig = true;
                hitPoint = hit.point; // Store the hit point
            }
            else
            {
                canDig = false;
            }
        }
        else
        {
            canDig = false;
        }
    }

    public void Dig(float depth)
    {
        if (hitPoint == Vector3.zero)
        {
            Debug.LogWarning("No valid hit point found. Cannot dig.");
            return;
        }

        Debug.Log("I'm Digging!");

        Vector3 terrainPos = hitPoint - terrainCollider.transform.position;
        float terrainX = terrainPos.x / terrainData.size.x * terrainData.heightmapResolution;
        float terrainY = terrainPos.z / terrainData.size.z * terrainData.heightmapResolution;

        float[,] heights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);

        int centerX = Mathf.RoundToInt(terrainX);
        int centerY = Mathf.RoundToInt(terrainY);

        float displacedVolume = 0f; // Track displaced volume

        for (int x = Mathf.Max(0, centerX - Mathf.RoundToInt(radius)); x < Mathf.Min(terrainData.heightmapResolution, centerX + Mathf.RoundToInt(radius)); x++)
        {
            for (int y = Mathf.Max(0, centerY - Mathf.RoundToInt(radius)); y < Mathf.Min(terrainData.heightmapResolution, centerY + Mathf.RoundToInt(radius)); y++)
            {
                float distance = Vector2.Distance(new Vector2(centerX, centerY), new Vector2(x, y));

                if (distance <= radius)
                {
                    float heightChange = Mathf.Lerp(depth * digMultiplier, 0, distance / radius);
                    displacedVolume += heightChange * terrainData.size.x * terrainData.size.z / terrainData.heightmapResolution;
                    heights[y, x] -= heightChange;
                }
            }
        }

        terrainData.SetHeights(0, 0, heights);

        // Create the dirt pile
        UpdateDirtPile(hitPoint, displacedVolume);

        // Paint the terrain at the digging spot
        UpdatePaintTerrain();

        // Update Terrain Collision
        UpdateCollision();
    }

    void UpdateDirtPile(Vector3 digPosition, float displacedVolume)
    {
        // Proceduralize the dirt pile position based on dig radius and distance
        Vector3 pileDirection = new Vector3(1f, 0f, 1f).normalized; // Direction for pile placement
        float pileDistance = radius * pileRange; // Offset distance relative to dig radius
        Vector3 pilePosition = digPosition + pileDirection * pileDistance; // Offset the pile from the dig site

        Vector3 terrainPos = pilePosition - terrainCollider.transform.position;
        float terrainX = terrainPos.x / terrainData.size.x * terrainData.heightmapResolution;
        float terrainY = terrainPos.z / terrainData.size.z * terrainData.heightmapResolution;

        float[,] heights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);

        int pileCenterX = Mathf.RoundToInt(terrainX);
        int pileCenterY = Mathf.RoundToInt(terrainY);

        float pileRadius = radius * 0.75f; // Scale the pile radius relative to the dig radius

        // Calculate the area of the pile for normalization
        float pileArea = Mathf.PI * pileRadius * pileRadius;

        // Normalize displacedVolume by area and terrain height scale to avoid excessive height
        float heightContribution = displacedVolume / pileArea / terrainData.size.y;

        for (int x = Mathf.Max(0, pileCenterX - Mathf.RoundToInt(pileRadius)); x < Mathf.Min(terrainData.heightmapResolution, pileCenterX + Mathf.RoundToInt(pileRadius)); x++)
        {
            for (int y = Mathf.Max(0, pileCenterY - Mathf.RoundToInt(pileRadius)); y < Mathf.Min(terrainData.heightmapResolution, pileCenterY + Mathf.RoundToInt(pileRadius)); y++)
            {
                float distance = Vector2.Distance(new Vector2(pileCenterX, pileCenterY), new Vector2(x, y));

                if (distance <= pileRadius)
                {
                    // Apply Gaussian-like falloff for a smoother slope
                    float falloff = Mathf.Exp(-distance * distance / (2 * pileRadius * pileRadius));

                    // Calculate height change considering both falloff and volume scaling
                    float heightChange = heightContribution * falloff;

                    // Ensure that the resulting height change is within a reasonable range
                    heights[y, x] = Mathf.Clamp(heights[y, x] + heightChange, 0, 1); // Clamp to valid height range (0 to 1)
                }
            }
        }

        terrainData.SetHeights(0, 0, heights);

        Debug.Log($"Dirt pile created at {pilePosition}, Dig Radius: {radius}, Dig Depth: {digDepth}");
    }

    void UpdatePaintTerrain()
    {
        if (digLayer == null)
        {
            Debug.LogWarning("No terrain layer assigned for painting.");
            return;
        }

        // Get the terrain texture alpha map
        float[,,] splatmapData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);

        // Get the index of the digLayer in the terrain layers array
        int digLayerIndex = -1;
        TerrainLayer[] terrainLayers = terrainData.terrainLayers;

        for (int i = 0; i < terrainLayers.Length; i++)
        {
            if (terrainLayers[i] == digLayer)
            {
                digLayerIndex = i;
                break;
            }
        }

        if (digLayerIndex == -1)
        {
            Debug.LogWarning("Dig layer not found in terrain layers.");
            return;
        }

        // Convert world position to terrain coordinates
        Vector3 terrainPos = hitPoint - terrainCollider.transform.position;
        float terrainX = terrainPos.x / terrainData.size.x * terrainData.alphamapWidth;
        float terrainY = terrainPos.z / terrainData.size.z * terrainData.alphamapHeight;

        int centerX = Mathf.RoundToInt(terrainX);
        int centerY = Mathf.RoundToInt(terrainY);

        // Apply the texture painting effect within the radius
        for (int x = Mathf.Max(0, centerX - Mathf.RoundToInt(radius)); x < Mathf.Min(terrainData.alphamapWidth, centerX + Mathf.RoundToInt(radius)); x++)
        {
            for (int y = Mathf.Max(0, centerY - Mathf.RoundToInt(radius)); y < Mathf.Min(terrainData.alphamapHeight, centerY + Mathf.RoundToInt(radius)); y++)
            {
                float distance = Vector2.Distance(new Vector2(centerX, centerY), new Vector2(x, y));

                if (distance <= radius)
                {
                    // Adjust the strength of the alpha (higher values will paint more intensely)
                    float strength = 1.5f; // You can adjust this value
                    float alpha = Mathf.Lerp(1f, 0f, distance / radius) * strength;
                    alpha = Mathf.Clamp01(alpha); // Ensure alpha stays within valid range

                    // Calculate the total current alpha sum to maintain blending
                    float currentTotalAlpha = 0f;

                    for (int i = 0; i < terrainData.terrainLayers.Length; i++)
                    {
                        currentTotalAlpha += splatmapData[y, x, i];
                    }

                    // Normalize the current layers before blending
                    for (int i = 0; i < terrainData.terrainLayers.Length; i++)
                    {
                        splatmapData[y, x, i] /= currentTotalAlpha;
                    }

                    // Blend the digging layer with existing layers
                    for (int i = 0; i < terrainData.terrainLayers.Length; i++)
                    {
                        if (i == digLayerIndex)
                        {
                            splatmapData[y, x, i] = Mathf.Clamp01(splatmapData[y, x, i] + alpha);
                        }
                        else
                        {
                            splatmapData[y, x, i] *= (1f - alpha);
                        }
                    }

                    // Re-normalize to ensure the total alpha is still 1
                    float newTotalAlpha = 0f;

                    for (int i = 0; i < terrainData.terrainLayers.Length; i++)
                    {
                        newTotalAlpha += splatmapData[y, x, i];
                    }

                    for (int i = 0; i < terrainData.terrainLayers.Length; i++)
                    {
                        splatmapData[y, x, i] /= newTotalAlpha;
                    }
                }
            }
        }

        // Apply the updated alpha map back to the terrain
        terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    void UpdateCollision()
    {
        //update collision on dig
        terrainCollider.terrainData = terrainData;
    }

    void Update()
    {
        #region Digging
        CheckEquipped();
        CheckDiggable();

        if (canDig && isEquipped)
        {
            digCooldownTimer -= Time.deltaTime;

            if (Input.GetMouseButton(0) && digCooldownTimer <= 0f)
            {
                Dig(digDepth);
                EventManager.current.DigEvent(); // Trigger dig
                EventManager.current.SpawnFXEvent("shovelFX"); // Trigger dig FX with "shovelFX" ID
                digCooldownTimer = digCooldown;
            }

            if (Input.GetMouseButton(1) && digCooldownTimer <= 0f)
            {
                Dig(-digDepth);
                EventManager.current.DigEvent(); // Trigger dig
                EventManager.current.SpawnFXEvent("shovelFX"); // Trigger dig FX with "shovelFX" ID
                digCooldownTimer = digCooldown;
            }
        }
        #endregion
    }
}
