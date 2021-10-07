//This script regulates the basic behavior of boxes in our game. Tht behavior can be changed using the events described in
// BoxEvents.cs. It is also a sort of extension of the box-pushing mechanic describer in PlayerController.cs.

using UnityEngine;

public class BoxScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private FixedJoint2D joint;
    public bool isGrounded = false;
    public bool isInteractable = true;
    public bool shouldIGround = true;

    public float raycastDistance = .1f;

    public Transform groundCheck1, groundCheck2, groundCheck3, groundCheck4;
    public LayerMask m_WhatIsGround;

    private PlayerSFX sfxController;




    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        joint = GetComponent<FixedJoint2D>();
        sfxController = GameObject.FindGameObjectWithTag("AudioSource").GetComponent<PlayerSFX>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Here we use a fixed joint to move the box when it is being pushed by the player. Since the player is
        // not allowed to jump while pushing, this guarantees that the box will keep a horizontal movement while 
        // being pushed and will not display erratic behavior.

        if (shouldIGround) GroundedCheck();

        if (joint.connectedBody == null && isGrounded)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.velocity = Vector3.zero;
        }
        if (joint.connectedBody != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;           
        }
    }



    private void GroundedCheck()
    {
        // This groundcheck method is very similar to the one used in other scripts int his project.
        // The variable starts out as false, and gets turned on if one of the raycasts, which is always pointing down,
        // hits a ground object. There is one raycast originating from each of the box's faces. 

        bool wasGrounded = isGrounded;

        isGrounded = false;

        bool tempGrounded1 = false;




        // Previously I used circlecasts, but decided ultimately to go ahead with raycasts since theyre slightly more reliable.
        // Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck1.position, .1f, m_WhatIsGround);
        //Collider2D[] colliders2 = Physics2D.OverlapCircleAll(groundCheck2.position, .1f, m_WhatIsGround);
       // Collider2D[] colliders3 = Physics2D.OverlapCircleAll(groundCheck3.position, .1f, m_WhatIsGround);
      //  Collider2D[] colliders4 = Physics2D.OverlapCircleAll(groundCheck4.position, .1f, m_WhatIsGround);

        RaycastHit2D check1 = Physics2D.Raycast(groundCheck1.position, Vector2.down, raycastDistance, m_WhatIsGround);
        RaycastHit2D check2 = Physics2D.Raycast(groundCheck2.position, Vector2.down, raycastDistance, m_WhatIsGround);
        RaycastHit2D check3 = Physics2D.Raycast(groundCheck3.position, Vector2.down, raycastDistance, m_WhatIsGround);
        RaycastHit2D check4 = Physics2D.Raycast(groundCheck4.position, Vector2.down, raycastDistance, m_WhatIsGround);

       /* for (int i = 0; i < colliders.Length; i++) //checking for the first collider
        {
            if (colliders[i].gameObject != gameObject)
            {
               // tempGrounded1 = true;
            }
        }*/

        if (check1 || check2 || check3 || check4)
        {
            tempGrounded1 = true;
        }



        if (tempGrounded1) //if either is overlapping with the ground...
        {
            isGrounded = true;

            // If the box wasn't grounded in the last frame(i.e. in the air), we can assume it's landing
            if (!wasGrounded)
            {
                sfxController.PlaySFX("boxLand");                
                rb.bodyType = RigidbodyType2D.Static;
                
            }
            // else isGrounded = false;
        }


        if (!isGrounded)  // If, after all the checks, we're still not grounded, then we can consider that the box is falling
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    private void OnDrawGizmos()
    {
        // Gizmos are used to draw some representations in the editor to things happening elsewhere in the code. These represent the circle-
        // casts used in the groundcheck.
        //Gizmos.DrawWireSphere(groundCheck1.position, .1f);
        //Gizmos.DrawWireSphere(groundCheck2.position, .1f);
        //Gizmos.DrawWireSphere(groundCheck3.position, .1f);
        //Gizmos.DrawWireSphere(groundCheck4.position, .1f);

        Gizmos.DrawRay(groundCheck1.position, Vector2.down * raycastDistance);
        Gizmos.DrawRay(groundCheck2.position, Vector2.down * raycastDistance);
        Gizmos.DrawRay(groundCheck3.position, Vector2.down * raycastDistance);
        Gizmos.DrawRay(groundCheck4.position, Vector2.down * raycastDistance);
    }
}
