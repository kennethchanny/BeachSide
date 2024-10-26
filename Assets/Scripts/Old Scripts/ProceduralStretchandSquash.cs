using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProceduralStretchandSquash : MonoBehaviour
{
   
    //declare rigidbody
    private Rigidbody2D _rb;
    //declareforce
    private Vector2     m_Force = Vector2.zero;
    //declarespeed
    public float        m_Speed = 100f;

    //declare jump force
    private float       m_JumpForce = 500f;
   
    public float        m_Mass = 70f;


    //Procedural Anim var

    private CapsuleCollider2D   m_CapsuleCollider2D     = null;
    private Vector2             m_Acceleration          = Vector2.zero;
    private Vector3             m_OriginalScale;
    private Vector2             m_OriginalSize          = Vector2.zero;
    private Vector2             m_OriginalOffset        = Vector2.zero;
    private Vector2             m_PrevVelocity          = Vector2.zero;
  

    private void Start()
    {
        //reference rigidbody2d
        _rb = GetComponent<Rigidbody2D>();

        m_CapsuleCollider2D =    GetComponent<CapsuleCollider2D>();
        m_OriginalSize      =    m_CapsuleCollider2D.size;
        m_OriginalOffset    =    m_CapsuleCollider2D.offset;
        m_OriginalScale     =    transform.localScale;
        m_PrevVelocity      =    _rb.velocity;
    }
  
    

  

    
    void ProceduralAnimation()
    {
        //Do rotation by transforming the velocity of X to be the rotation of our object
        transform.rotation = Quaternion.Euler(0, 0, -_rb.velocity.x/3);

        //scale factors
        const float S1 = 0.02f;
        const float S2 = 0.02f;

        //Conservation of Mass (Volume, Stretch and Squash)

        var V = new Vector2(

            //if acceleration is 0, the scale is 1
            //adjust the scale if there is acceleration
            //if you are moving in thhe horizontal, scale up the x and the y down
            //if you are moving in the vertical, scale up the y, and the x down.
            1 - m_Acceleration.x * S1 + m_Acceleration.y * S2,
            1 - m_Acceleration.y * S2 + m_Acceleration.x * S1
            );

        //Scale the sprite which means we can to inversely scale the collider
        //Becase we dont want our colision to change
        
        transform.localScale = m_OriginalScale * V;
        m_CapsuleCollider2D.size = m_OriginalSize / V;

        //adjust the capsule offsets
        var NewOffset   = m_CapsuleCollider2D.offset;
        //Compare diff betweenn new capsulecollider and original, multiply by V.y (Change capsule size as sprite size change)
        NewOffset.y = m_OriginalOffset.y + (m_CapsuleCollider2D.size.y - m_OriginalSize.y) * V.y;
        m_CapsuleCollider2D.offset = NewOffset;

        //Decay accelration
        m_Acceleration *= 0.9f;
    }

    


   

    private void Update()
    {
        
        ProceduralAnimation();
    }

    void FixedUpdate()
    {
        //Procedural
        m_Acceleration.x += Mathf.Abs(m_PrevVelocity.x - _rb.velocity.x);
        m_Acceleration.y += Mathf.Abs(m_PrevVelocity.y - _rb.velocity.y);
        m_PrevVelocity = _rb.velocity;



        _rb.AddForce(m_Force);
        m_Force = Vector2.zero;

      
    }
}

