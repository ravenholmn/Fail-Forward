using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


// This is a highly modified version of the 2D Character Controller script by Brackeys. Source: https://github.com/Brackeys/2D-Character-Controller/blob/master/CharacterController2D.cs
public class CharacterController2D : MonoBehaviour
{
    public Rigidbody2D RB
    {
        get
        {
            return rb;
        }
    }
    [SerializeField] private float jumpForce = 700f;
    [SerializeField] private Vector2 wallJumpForce = new Vector2(500f, 500f);
    [SerializeField] private float dashForce = 400f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float wallJumpTime = 0.4f;
    [SerializeField] private float jumpBufferTime = 0.2f;
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float wallSlideTime = 0.2f;
    [Range(0, 1)][SerializeField] private float crouchSpeed = 0.36f;
    [Range(0, .3f)][SerializeField] private float movementSmoothing = 0.05f;
    [SerializeField] private bool airControl = false;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform ceilingCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Collider2D crouchDisableCollider;
    [SerializeField] private AudioClip jumpAudioClip;
    [SerializeField] private AudioClip dashAudioClip;
    [SerializeField] private AudioClip[] walkAudioClips;

    const float groundedRadius = 0.4f;
    private bool grounded;
    const float ceilingRadius = 0.2f;
    const float wallRadius = 0.2f;
    private bool onWall;
    private bool wallSliding;
    private bool jumped;
    private Vector2 dashDir;
    private bool dashing;
    private bool canDash;
    private float coyoteTimeCounter;
    private float wallSlideCounter;
    private float wallJumpCounter;
    private float jumpBufferCounter;
    private Rigidbody2D rb;
    private bool facingRight = true;
    private Vector3 velocity = Vector3.zero;
    private Vector3 lastImagePos;

    private float walkTimer = 0f;
    private float walkTime = 0.3f;
    private int step = 0;

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    private bool wasCrouching = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        OnLandEvent ??= new UnityEvent();

        OnCrouchEvent ??= new BoolEvent();
    }

    private void Update()
    {
        if (grounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        if (wallSliding)
        {
            wallSlideCounter = wallSlideTime;
        }
        else
        {
            wallSlideCounter -= Time.deltaTime;
        }
        if (wallSlideCounter > 0)
        {
            wallJumpCounter = wallJumpTime;
        }
        else
        {
            wallJumpCounter -= Time.deltaTime;
        }

        if (jumped)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (dashing)
        {
            rb.velocity = dashDir.normalized * dashForce;
            return;
        }
    }

    private void FixedUpdate()
    {
        bool wasGrounded = grounded;
        grounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                grounded = true;
                if (!dashing)
                {
                    canDash = true;
                }
                if (!wasGrounded)
                {
                    step = 0;
                    OnLandEvent.Invoke();
                }
            }
        }

        onWall = Physics2D.OverlapCircle(wallCheck.position, wallRadius, whatIsWall);
    }


    public void Move(float move, bool crouch, bool jump, bool dash, Vector2 dir)
    {
        jumped = jump;

        if (dash && canDash)
        {
            if (grounded && crouch)
                return;

            dashing = true;
            canDash = false;
            dashDir = dir;

            PlayerAfterImagePool.Instance.GetFromPool();
            lastImagePos = transform.position;

            StartCoroutine(StopDashing());
        }

        // if (!crouch && !jump)
        // {
        //     if (Physics2D.OverlapCircle(ceilingCheck.position, ceilingRadius, whatIsGround))
        //     {
        //         crouch = true;
        //     }
        // }

        if (grounded || airControl)
        {

            // If crouching
            if (crouch && grounded && !dash)
            {
                if (!wasCrouching)
                {
                    wasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }

                move *= crouchSpeed;

                if (crouchDisableCollider != null)
                    crouchDisableCollider.enabled = false;
            }
            else
            {
                if (crouchDisableCollider != null)
                    crouchDisableCollider.enabled = true;

                if (wasCrouching)
                {
                    wasCrouching = false;
                    OnCrouchEvent.Invoke(false);
                }
            }

            Vector3 targetVelocity = new Vector2(move * 10f, rb.velocity.y);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);

            if (walkTimer < walkTime)
            {
                walkTimer += Time.deltaTime;
            }
            else
            {
                walkTimer = 0;
            }

            if (!crouch && grounded && Mathf.Abs(move) > 0 && walkTimer == 0)
            {
                AudioManager.Instance.PlaySFX(walkAudioClips[step], gameObject.transform, 1f);
                step++;
                if (step > 3)
                {
                    step = 0;
                }
            }

            if (move > 0 && !facingRight)
            {
                Flip();
            }
            else if (move < 0 && facingRight)
            {
                Flip();
            }
        }

        if (onWall && !grounded && Math.Abs(move) > 0f)
        {
            wallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else
        {
            wallSliding = false;
        }

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            jumpBufferCounter = 0f;
            grounded = false;
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(new Vector2(0f, jumpForce));
            AudioManager.Instance.PlaySFX(jumpAudioClip, gameObject.transform, 1f);
        }
        if (coyoteTimeCounter > 0f && !jump)
        {
            coyoteTimeCounter = 0f;
        }

        if (wallJumpCounter > 0f && jumpBufferCounter > 0f && !onWall)
        {
            jumpBufferCounter = 0f;
            wallSliding = false;
            rb.velocity = new Vector2(0f, 0f);
            rb.AddForce(new Vector2(wallJumpForce.x * move, wallJumpForce.y));
            AudioManager.Instance.PlaySFX(jumpAudioClip, gameObject.transform, 1f);
        }
        if (wallJumpCounter > 0f && !jump)
        {
            wallJumpCounter = 0f;
        }
    }

    private IEnumerator StopDashing()
    {
        AudioManager.Instance.PlaySFX(dashAudioClip, gameObject.transform, 1f);
        float t = 0f;
        float dur = dashTime;
        canDash = false;
        while (t < dur)
        {
            t += Time.deltaTime;


            if (Vector2.Distance(transform.position, lastImagePos) > PlayerAfterImagePool.Instance.distanceBetweenImages)
            {
                PlayerAfterImagePool.Instance.GetFromPool();
                lastImagePos = transform.position;
            }

            if (gameObject.GetComponent<PlayerMovement>().died || t >= dashTime)
            {
                if (grounded)
                {
                    canDash = true;
                }
                rb.velocity = new Vector2(0f, 0f);
                dashing = false;
            }


            yield return new WaitForEndOfFrame();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void ResetDash()
    {
        canDash = true;
    }
}