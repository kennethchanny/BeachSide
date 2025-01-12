using UnityEngine;

public class Player_SandDisplacementController : MonoBehaviour
{
    public float displacementRadius = 0.5f; // Smaller radius to control localized displacement
    public float displacementAmount = 0.0002f; // Reduced amount for subtle displacement
    public float smoothingStrength = 0.3f; // Lower smoothing strength to avoid over-smoothing
    public float randomDisplacementFactor = 0.005f; // Small random factor for variation

    // Terrain
    private GameObject terrainGameObject;
    private Terrain terrain;
    private TerrainData terrainData;
    private TerrainCollider terrainCollider;
    private Vector3 playerPosition;


    void Start()
    {
        // Assign terrain refs from TerrainInitializer
        TerrainInitializer terrainInitializer = FindObjectOfType<TerrainInitializer>();
        terrainGameObject = terrainInitializer.gameObject;
        terrain = terrainGameObject.GetComponent<Terrain>();
        terrainData = terrainInitializer.GetTerrainData();
        terrainCollider = terrainInitializer.GetTerrainCollider();

    }

    void Update()
    {
        // Get the player's current position
        playerPosition = transform.position;

        // Cast multiple rays around the player to check if they're walking on sand
        RaycastSandDisplacement();
    }

    void RaycastSandDisplacement()
    {
        Vector3[] directions = {
            transform.forward, // Check forward
            -transform.forward, // Check backward
            transform.right, // Check right
            -transform.right // Check left
        };

        foreach (var direction in directions)
        {
            Ray ray = new Ray(playerPosition, direction);
            RaycastHit hit;

            // Perform raycast
            if (Physics.Raycast(ray, out hit, displacementRadius))
            {
                // Check if the hit object is the terrain
                if (hit.collider.gameObject == terrainCollider.gameObject)
                {
                    Vector3 hitPoint = hit.point;

                    // Perform sand displacement and smoothing at the hit point
                    DisplaceSand();
                }
            }
        }
    }

    void DisplaceSand()
    {

    }

}
