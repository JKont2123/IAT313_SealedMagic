using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Player components
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;

    //Player Input
    Vector2 moveInput;
    private bool isFacingRight;

    //Movement
    public float moveSpeed, acceleration, decceleration, velPower;

    //Jumping
    public float jumpForce;
    public float doubleJumpForce;    
    public bool isGrounded;
    private bool doubleJump;

    //Wall Sliding
    public bool isWallSliding;
    public float wallSlidingSpeed = 2f;

    //Wall Jumping
    public bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(5f, 3f);

    //Serialize Fields
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    
    

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        isFacingRight = true;
        
    }

    private void Update()
    {
        if (!DialogueManager.GetInstance().dialogueIsPlaying)
        {
            //Set move input values to reflect player horizontal and vertical inputs
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");

            if (moveInput.x > 0 || moveInput.x < 0 && isGrounded)

            {
                FindObjectOfType<AudioManager>().Play("Moving");
            }

            else if (moveInput.x == 0 || !isGrounded)
            {
                FindObjectOfType<AudioManager>().Stop("Moving");
            }

            //Allowing jumps and double jumps when space is pressed
            if (Input.GetButtonDown("Jump"))
            {
                if (isGrounded || doubleJump)
                {
                    Jump();
                    doubleJump = !doubleJump;
                }
            }

            //Disable double jump once player hits the ground
            if (isGrounded && !Input.GetButtonDown("Jump"))
            {
                doubleJump = false;
            }

            WallSlide();
            WallJump();

            if (!isWallJumping)
            {
                Flip();
            }
        }

    }

    private void FixedUpdate()
    {
        if (!DialogueManager.GetInstance().dialogueIsPlaying)
        {
            //calculate the direction we want to move in and our desired velocity
            float targetSpeed = moveInput.x * moveSpeed;
            //calculate difference between current velocity and desired velocity
            float speedDif = targetSpeed - rb.velocity.x;
            //change acceleration rate depending on situation
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
            //applies acceleration to speed difference, the raises to a set power so acceleration increases with higher speeds
            //finally multiplies by sign to reapply direction
            float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

            //applies force to rigidbody, multiplying by Vector2.right so that it only affects X axis
            if (!isWallJumping)
            {
                rb.AddForce(movement * Vector2.right);
            }
            anim.SetFloat("xVelocity", Mathf.Abs(moveInput.x));
            anim.SetFloat("yVelocity", rb.velocity.y);
        }

        if (DialogueManager.GetInstance().dialogueIsPlaying)
        {
            rb.velocity = Vector2.zero;
            anim.SetFloat("xVelocity", 0);
            anim.SetFloat("yVelocity", 0);
        }
            
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Platform")
        {
            if (rb.velocity.y <= 0.01)
            {
                isGrounded = true;
                anim.SetBool("isJumping", !isGrounded);
            }
        }
    }

    

    void Jump()
    {
        rb.AddForce(Vector2.up * (doubleJump? doubleJumpForce : jumpForce), ForceMode2D.Impulse);
        isGrounded = false;
        anim.SetBool("isJumping", !isGrounded);
        FindObjectOfType<AudioManager>().Play("Jump");
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if(IsWalled() && !isGrounded)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -wallSlidingSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if(Input.GetButtonDown("Jump") && wallJumpingCounter > 0)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;
            FindObjectOfType<AudioManager>().Play("Jump");

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    //Flip character sprite depending on direction
    private void Flip()
    {
        if(isFacingRight && moveInput.x < 0f || !isFacingRight && moveInput.x > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}
