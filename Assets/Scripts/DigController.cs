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
    private GameObject terrainGameObject;
    private Terrain terrain;
    private TerrainData terrainData;

    // Terrain Layer Index

    public TerrainLayer digLayer;
    int digLayerIndex = 1; // Set this to the index of your digging layer in the Unity Editor

    #endregion

    #region Variables

    // Digging
    private Vector3 hitPoint;
    public bool isDigging = false;
    public bool canDig = false;

    public float diggableDistance = 3;

    // textureLayer paintStrength
    public float paintStrength = 1.5f; 

    // Define the digging range and radius
    public float radius = 2f; // The radius of the area to affect
    public float digDepth = 0.0002f;  // How deep to dig

    // Digging Cooldown
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

        // Assign terrain refs
        terrainGameObject = GameObject.FindGameObjectWithTag("Terrain");
        terrain = terrainGameObject.GetComponent<Terrain>();
        terrainData = terrain.terrainData;

        // Check if digLayerIndex is within bounds
        if (digLayerIndex < 0 || digLayerIndex >= terrain.terrainData.terrainLayers.Length)
        {
            Debug.LogError("Invalid digLayerIndex. Please ensure it's within the range of available terrain layers.");
        }
    }

    void CheckDiggable()
    {
        Vector3 origin = playerCamera.position;
        Vector3 direction = playerCamera.forward;
        

        if (Physics.Raycast(origin, direction, out RaycastHit hit, diggableDistance))
        {
            Debug.DrawRay(origin, direction * diggableDistance, Color.red);

            if (hit.collider.gameObject == terrainGameObject)
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

    void Dig(float depth)
    {
        if (hitPoint == Vector3.zero)
        {
            Debug.LogWarning("No valid hit point found. Cannot dig.");
            return;
        }

        Debug.Log("Im Digging!");

        // Convert world position to terrain coordinates
        Vector3 terrainPos = hitPoint - terrain.transform.position;
        float terrainX = terrainPos.x / terrainData.size.x * terrainData.heightmapResolution;
        float terrainY = terrainPos.z / terrainData.size.z * terrainData.heightmapResolution;

        // Get the heightmap
        float[,] heights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);

        // Calculate the center of the digging area
        int centerX = Mathf.RoundToInt(terrainX);
        int centerY = Mathf.RoundToInt(terrainY);

        // Apply the digging effect within the radius
        for (int x = Mathf.Max(0, centerX - Mathf.RoundToInt(radius)); x < Mathf.Min(terrainData.heightmapResolution, centerX + Mathf.RoundToInt(radius)); x++)
        {
            for (int y = Mathf.Max(0, centerY - Mathf.RoundToInt(radius)); y < Mathf.Min(terrainData.heightmapResolution, centerY + Mathf.RoundToInt(radius)); y++)
            {
                float distance = Vector2.Distance(new Vector2(centerX, centerY), new Vector2(x, y));

                if (distance <= radius)
                {
                    float heightChange = Mathf.Lerp(depth, 0, distance / radius);
                    heights[y, x] -= heightChange;
                }
            }
        }

        // Set the updated heights back to the terrain
        terrainData.SetHeights(0, 0, heights);

        // Paint the terrain at the digging spot
        PaintTerrain();
    }

    void PaintTerrain()
    {
        if (digLayer == null)
        {
            Debug.LogWarning("No terrain layer assigned for painting.");
            return;
        }

        // Get the terrain texture alpha map
        float[,,] splatmapData = terrain.terrainData.GetAlphamaps(0, 0, terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);

        // Get the index of the digLayer in the terrain layers array
        int digLayerIndex = -1;
        TerrainLayer[] terrainLayers = terrain.terrainData.terrainLayers;

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
        Vector3 terrainPos = hitPoint - terrain.transform.position;
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

                    for (int i = 0; i < terrain.terrainData.terrainLayers.Length; i++)
                    {
                        currentTotalAlpha += splatmapData[y, x, i];
                    }

                    // Normalize the current layers before blending
                    for (int i = 0; i < terrain.terrainData.terrainLayers.Length; i++)
                    {
                        splatmapData[y, x, i] /= currentTotalAlpha;
                    }

                    // Blend the digging layer with existing layers
                    for (int i = 0; i < terrain.terrainData.terrainLayers.Length; i++)
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

                    for (int i = 0; i < terrain.terrainData.terrainLayers.Length; i++)
                    {
                        newTotalAlpha += splatmapData[y, x, i];
                    }

                    for (int i = 0; i < terrain.terrainData.terrainLayers.Length; i++)
                    {
                        splatmapData[y, x, i] /= newTotalAlpha;
                    }
                }
            }
        }

        // Set the updated alpha map back to the terrain
        terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    void Update()
    {
        #region Digging
        CheckDiggable();

        if (canDig)
        {
            digCooldownTimer -= Time.deltaTime;

            // If left click and cooldown has elapsed
            if (Input.GetMouseButton(0) && digCooldownTimer <= 0f)
            {
                Dig(digDepth);
                digCooldownTimer = digCooldown; // Reset the cooldown timer
            }

            // If right click and cooldown has elapsed
            if (Input.GetMouseButton(1) && digCooldownTimer <= 0f)
            {
                Dig(-digDepth);
                digCooldownTimer = digCooldown; // Reset the cooldown timer
            }
        }
        #endregion
    }
}