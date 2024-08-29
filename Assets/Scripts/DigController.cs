using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigController : MonoBehaviour
{
    #region References
    FirstPersonController firstPersonController;
    public Terrain terrain;
    TerrainData terrainData;
    #endregion

    #region 
    public Transform playerCamera;
    public bool isDigging = false;
    public bool canDig = false;
    public KeyCode digKey = KeyCode.Mouse0;


    #endregion

    void Start()
    {
        //assign first person controller
        firstPersonController = GetComponent<FirstPersonController>();
        terrainData = terrain.terrainData;

    }

    void CheckDiggable()
    {
        Vector3 origin = playerCamera.transform.position;
        Vector3 direction = playerCamera.forward;
        float distance = 3f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            Debug.DrawRay(origin, direction * distance, Color.red);
            canDig = true;
        }
        else
        {
            canDig = false;
        }
    }    

    void Dig()
    {
        
        Debug.Log("Im Digging!");
    }


    void Update()
    {
        #region Dig
        CheckDiggable();

        if (canDig)
        {
            if (Input.GetKeyDown(digKey))
            {
                Dig();
            }
            
        }
    }
    #endregion
}

