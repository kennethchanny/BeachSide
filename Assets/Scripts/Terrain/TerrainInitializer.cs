using UnityEngine;

public class TerrainInitializer : MonoBehaviour
{
    // References to the terrain components
    private Terrain terrain;
    private TerrainData terrainData;
    private TerrainCollider terrainCollider;
    private TerrainData terrainColliderData;

    void Start()
    {
        // Directly assign the terrain and collider components since they're on the same GameObject
        terrain = GetComponent<Terrain>();
        terrainCollider = GetComponent<TerrainCollider>();

        // Check if the Terrain component is found
        if (terrain == null)
        {
            Debug.LogError("Terrain component not found on this GameObject.");
            return;
        }

        // Create a new copy of the terrain data to use during Play mode
        terrainData = Instantiate(terrain.terrainData);
        terrain.terrainData = terrainData; // Assign the copy to the Terrain component

        // Create a new copy of the terrain collision data to use during Play mode
        terrainColliderData = Instantiate(terrainCollider.terrainData);
        terrainCollider.terrainData = terrainColliderData;
    }

    public TerrainData GetTerrainData()
    {
        return terrainData;
    }

    public TerrainCollider GetTerrainCollider()
    {
        return terrainCollider;
    }
}
