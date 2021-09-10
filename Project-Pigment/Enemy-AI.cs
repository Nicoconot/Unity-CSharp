// Script Currently in development. The initial AI has been created, now each enemy color has to be tweaked to comply with the expected behavior
// in the GDD. The system is very simple: We have 3 detectors attached to each enemy gameobject: A hole detector, a wall detector and a jumpable 
// space detector. The wall and jump detectors have a short raycast forward, while the hole detector shoots a raycast down, and all are slightly 
// in front of the enemy in varying heights. Depending on if the raycast detects an object in the layermask provided(in this case, "Ground") for 
// each detector, an assumption will be made. The enemy can then turn around, jump or simply move forward.
// The grounded detection system is based off of Brackey's code.
// I realize it would be much better to use an OOP system for the different enemies, and something like that will be created later on. Right now,
// it's easier to manage and design a big chunk of mixed code for fast iteration, and I think it's a bit easier to understand from an outside 
//perspective as well.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public EnemyColor enemyColor;
    public float speed;
    public float distance;    
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck, m_GroundCheck2;                           // A position marking where to check if the player is grounded.
    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded

    [Header("Very sensitive number. Reference that works: 10")]
    public float jumpHeight = 10;

    private bool walking = true;
    private bool isDashing = false;
    private bool isGrounded;
    [HideInInspector]public bool followSomething = false;
    private bool canJump = false;
    private bool canFall = false;
    [HideInInspector] public string followTag = "Player";

    private SpriteRenderer sr;
    private Animator anim;
    private Rigidbody2D rb;
    public FollowCollisionCheck followCollisionCheck;

    private bool movingRight = true;

    [SerializeField]private Transform groundDetection, wallDetection, jumpDetection;

    

    private void Awake()
    {
        groundDetection = transform.GetChild(0).transform;
        wallDetection = transform.GetChild(1).transform;
        jumpDetection = transform.GetChild(2).transform;
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GroundedCheck();

        if (enemyColor == EnemyColor.white)
        {
            anim.SetBool("IsWalking", false);
            sr.color = Color.white;
        }

        // We've temporarily set the red and orange enemies as the "guinea pigs" for this code. They'll be much more restricted later on.
        if (enemyColor == EnemyColor.red)
        {
            sr.color = Color.red;
            anim.SetBool("IsWalking", true);

            followSomething = false;                        
            canJump = false;
            canFall = false;

            MoveAround();               
        }

        if (enemyColor == EnemyColor.orange)
        {
            anim.SetBool("IsWalking", true);
            sr.color = new Color(1, 0.65f, 0);

            followSomething = true;
            followTag = "Player";
            canJump = true;
            canFall = false;

            MoveAround();
        }
        if (enemyColor == EnemyColor.yellow)
        {
            anim.SetBool("IsWalking", false);
            sr.color = Color.yellow;
        }
        if (enemyColor == EnemyColor.green)
        {
            anim.SetBool("IsWalking", false);
            sr.color = Color.green;
        }
        if (enemyColor == EnemyColor.blue)
        {
            anim.SetBool("IsWalking", false);
            sr.color = Color.blue;
        }
        if (enemyColor == EnemyColor.purple)
        {
            anim.SetBool("IsWalking", false);
            sr.color = new Color(0.5f, 0, 0.8f);
        }
    }

    private void GroundedCheck()
    {
        bool wasGrounded = isGrounded;

        isGrounded = false;

        bool tempGrounded1 = false;
        bool tempGrounded2 = false;

        // The enemy is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.

        //Here I've chosen to use two circlecasts so that you can still jump if your character is at the edge of a tile.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        Collider2D[] colliders2 = Physics2D.OverlapCircleAll(m_GroundCheck2.position, k_GroundedRadius, m_WhatIsGround);
        //Collider2D[] colliders = Physics2D.OverlapBoxAll(box1.position, transform.localScale * 2, 0, m_WhatIsGround); tested using a box, didn't work

        for (int i = 0; i < colliders.Length; i++) //checking for the first collider
        {
            if (colliders[i].gameObject != gameObject)
            {
                tempGrounded1 = true;
            }
        }

        for (int i = 0; i < colliders2.Length; i++) //checking the second
        {
            if (colliders2[i].gameObject != gameObject)
            {
                tempGrounded2 = true;
            }
        }

        if (tempGrounded1 || tempGrounded2) //if either is overlapping with the ground...
        {
            isGrounded = true;

            // If the player wasn't grounded(i.e. in the air), we invoke the landing event(which is basically the animation switch)
            if (!wasGrounded)
                anim.SetBool("IsJumping", false);
        }


        if (!isGrounded)  // If, after all the checks, we're still not grounded, then we can consider that the player is jumping/falling
        {
            anim.SetBool("IsJumping", true);
        }
    }
    private IEnumerator PauseAndJump()
    {
        //This is related to the size of the enemy and the environment. Waiting a little bit before the next jump looks better.
        rb.velocity += Vector2.up * jumpHeight;
        
        yield return new WaitForSeconds(2f);       
    }


    public void TurnRight()
    {
        transform.eulerAngles = new Vector3(0, 0, 0);
        movingRight = true;
    }

    public void TurnLeft()
    {
        transform.eulerAngles = new Vector3(0, -180, 0);
        movingRight = false;
    }

    private void MoveAround()
    {
        if (walking) transform.Translate(Vector2.right * speed * Time.deltaTime);

        //This navigation system works by using 3 Raycast sources directly ahead of the enemy. One is at the bottom,
        // and is used to detect holes ahead. One is at the middle, and detects walls. One is above the head and 
        // detects if there's an empty space above any walls, which would mean the enemy can jump. Together
        // they allow the enemy to move back and forth within any walkable space and jump up to 1 block high platforms.
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distance, m_WhatIsGround);
        RaycastHit2D wallInfo = Physics2D.Raycast(wallDetection.position, Vector2.right, distance, m_WhatIsGround);
        RaycastHit2D jumpInfo = Physics2D.Raycast(jumpDetection.position, Vector2.right, distance, m_WhatIsGround);

        // First, we look for holes ahead. There must not be any walls, because that wouldn't be a hole!
        if (groundInfo.collider == false && wallInfo.collider == false && isGrounded)
        {
            if (isDashing)
            {
                StartCoroutine(FallFromDash());
                isDashing = false;
            } else if (!canFall && !isDashing)
            {
                if (movingRight == true)
                {
                    TurnLeft();
                }
                else
                {
                    TurnRight();
                }
            }
            
        }

        //Checks for a jumpable space, in case there is a wall ahead.
        if (jumpInfo.collider == false && wallInfo.collider == true && canJump)
        {   
            StartCoroutine(PauseAndJump());
        }

        //Check For wall ahead
        if (wallInfo.collider == true && jumpInfo.collider == true)
        {
            if (isDashing)
            {
                // See notes on HitWallDash
                //StartCoroutine(HitWallDash());
            } else if (!isDashing)
            {
                if (movingRight == true)
                {
                    TurnLeft();
                }
                else
                {
                    TurnRight();
                }
            }
            
        }
    }

    //----------------------------------------------
    //------------------RED ENEMY-------------------
    //----------------------------------------------
    public IEnumerator RedDash(bool dashToTheRight)
    {
        if (dashToTheRight)
        {
            isDashing = true;
            walking = false;
            TurnRight();
            yield return new WaitForSeconds(2f);
            speed = speed * 2;
            walking = true;

        } else if (!dashToTheRight)
        {
            isDashing = true;
            walking = false;
            TurnLeft();
            yield return new WaitForSeconds(2f);
            speed *= 2;
            walking = true;
        }
    }

    //If the red enemy is currently dashing, it'll fall from platforms
    private IEnumerator FallFromDash()
    {
        Debug.Log("fallfromdash");
        if (movingRight)
        {
            walking = false;
            rb.AddForce(Vector2.right * jumpHeight, ForceMode2D.Impulse);
            yield return new WaitForSeconds(5f);
            speed /= 2;
            walking = true;
            yield return new WaitForSeconds(3f);
            followCollisionCheck.DashIsFinished();
        }
        else
        {
            walking = false;
            rb.velocity += -Vector2.right * jumpHeight;
            yield return new WaitForSeconds(5f);
            speed /= 2;
            walking = true;
            yield return new WaitForSeconds(3f);
            followCollisionCheck.DashIsFinished();
        }
    }

    // Since currently, because of collider distances, the enemy is going to interpret hitting the wall as a hole anyway,
    // FallFromDash gets called naturally and works better than having a different coroutine. Yay!
    /*private IEnumerator HitWallDash()
    {
        walking = false;
        yield return new WaitForSeconds(5f);
        speed /= 2;
        walking = true;
    }*/

    private void OnDrawGizmos()
    {
        // Gizmos are used to draw some representations in the editor to things happening elsewhere in the code. These represent the circle-
        // casts used in the groundcheck.
        Gizmos.DrawWireSphere(m_GroundCheck.position, k_GroundedRadius);
        Gizmos.DrawWireSphere(m_GroundCheck2.position, k_GroundedRadius);
        
    }
}
