using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTerraforming : MonoBehaviour
{   
    
    //terrain
    private Terrain terrain;
    private TerrainData terrainData;


    void Start()
    {
        //Get terrain
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
    }

    void Update()
    {
        
    }

    void TerraformTerrain(float height, float range)
    {

    }
}
