using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicsManager: MonoBehaviour
{
    //Handles Cinematics
    public static CinematicsManager current;
    private Animator anim;
    private void Awake()
    {
        current = this;

    }

    void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(CueGame());
    }


    public enum CinematicsState
    {
        //In Dialogue state
        InGame,
        //In Cutscene state
        InCutscene,

    }

    public IEnumerator CueGame()
    {
        
        yield return new WaitForSeconds(1f);
        CinematicManagerEnterDefault();
    }
    //set state to InGame
    public CinematicsState currentState = CinematicsState.InCutscene;

    //Function to move to Cutscene
    public void CinematicManagerEnterCutsccene()
    {
        currentState = CinematicsState.InCutscene;

        
        anim.SetTrigger("InCutscene");

    }


    //Function to move to Default
    public void CinematicManagerEnterDefault()
    {
        currentState = CinematicsState.InGame;


        anim.SetTrigger("InGame");

    }

    void Update()
    {
        
    }
}
