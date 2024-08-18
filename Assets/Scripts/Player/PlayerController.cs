using System.Collections;
using System.Diagnostics;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    CapsuleCollider2D capCollider;

    public LayerMask groundLayer;
    public Transform groundDetectionOrigin;
    bool isGrounded = false;

    public float inputThreshold = 0.05f;
    float moveInput;
    float moveInputPrev;

    int facingDirection = 1;

    [Header("Scale")]
    public float defaultScale = 1.0f;
    public float bigScale = 1.5f;
    public float smallScale = 0.5f;
    int currentSize; // Int 0, 1, or 2
    float prevScale = 1.0f; // Float equal to either smallScale, defaultScale, or bigScale
    float currentScale = 1.0f; // Float equal to either smallScale, defaultScale, or bigScale

    [Header("Walking")]
    public float smallAcceleration;
    public float smallTopSpeed;
    public float smallDeceleration;
    [Space(10)]
    public float defaultAcceleration;
    public float defaultTopSpeed;
    public float defaultDeceleration;
    [Space(10)]
    public float bigAcceleration;
    public float bigTopSpeed;
    public float bigDeceleration;
    [Space(15)]
    public float horizontalSpeedThreshold = 0.0f;

    bool shouldMove = false;

    float acceleration;
    float topSpeed;
    float deceleration;

    [Header("Gravity")]
    public float gravity;
    public float terminalVelocity;

    [Header("Jumping")]
    public float smallJumpForce;
    public float defaultJumpForce;
    public float bigJumpForce;
    public float jumpBufferTime;
    float jumpForce;

    bool shouldJump;
    bool isJumping;
    bool jumpBuffering = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        capCollider = GetComponent<CapsuleCollider2D>();
        currentSize = 1;
        ChangeSize(1);
    }

    // Update is called once per frame
    void Update()
    {
        rb.gravityScale = gravity;
        Vector2 v = rb.velocity;

        moveInput = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(moveInput) >= inputThreshold)
            moveInputPrev = moveInput;

        // Ground Detection
        if (Physics2D.OverlapBox(groundDetectionOrigin.position, new Vector2(currentScale * 0.2f, 0.1f * currentScale), 0, groundLayer))
        {
            isGrounded = true;

            // Jump Buffer Check
            if (jumpBuffering)
            {
                StopCoroutine(JumpBuffer());
                jumpBuffering = false;
                shouldJump = true;
            }
            if (isJumping && v.y <= 0)
                isJumping = false;
        }
        else
            isGrounded = false;

        // Stop Sliding
        if (!shouldMove && !isJumping && isGrounded && rb.constraints != RigidbodyConstraints2D.FreezeAll)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            RayMoveFloor();
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
                shouldJump = true;
            else if (!jumpBuffering)
                StartCoroutine(JumpBuffer());
        }
        if (shouldJump) {
            Jump();
        }

        // Clamp Falling Speed
        if (Mathf.Abs(v.y) > terminalVelocity)
        {
            rb.velocity = new Vector2(v.x, Mathf.Sign(v.y) * terminalVelocity);
        }

        if (Input.GetKeyDown(KeyCode.Backspace)) {
            ChangeSize(2);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ChangeSize(1);
        }
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            ChangeSize(0);
        }
    }

    private void FixedUpdate()
    {
        // -- Horizontal Movement -- //
        
        Vector2 v = rb.velocity;

        if (Mathf.Abs(moveInput) >= inputThreshold)
        {
            if (!shouldMove)
            {
                rb.constraints = RigidbodyConstraints2D.None;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }

            shouldMove = true;

            float force = acceleration;

            // Apply Deceleration via Acceleration when quickly swapping directions.
            if (Mathf.Sign(moveInput) != Mathf.Sign(v.x))
                force += deceleration;

            rb.AddForce(Vector2.right * moveInput * force, ForceMode2D.Force);

            // Limit Horizontal Speed
            if (Mathf.Abs(v.x) > topSpeed)
            {
                rb.velocity = new Vector2(Mathf.Sign(v.x) * topSpeed, v.y);
            }
        }
        else
        {
            // Decelerate
            if (v.x * Mathf.Sign(moveInputPrev) > horizontalSpeedThreshold)
            {
                rb.AddForce(Vector2.right * -Mathf.Sign(v.x) * deceleration, ForceMode2D.Force);
            }
            else if (v.x != 0)
            {
                shouldMove = false;
                rb.velocity = new Vector2(0, v.y);
                moveInputPrev = 0;
            }
        }
    }

    void Jump()
    {
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        shouldJump = false;
        isJumping = true;
        jumpBuffering = false; // Just in case
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }


    // Size 0 = Shrink
    // Size 1 = Normal
    // Size 2 = Grow
    public void ChangeSize(int size)
    {
        switch (size)
        {
            case 0:
                transform.localScale = Vector2.one * smallScale;
                currentSize = 0;
                acceleration = smallAcceleration;
                topSpeed = smallTopSpeed;
                deceleration = smallDeceleration;
                jumpForce = smallJumpForce;

                prevScale = currentScale;
                currentScale = smallScale;
                break;
            case 1:
                transform.localScale = Vector2.one * defaultScale;
                currentSize = 1;
                acceleration = defaultAcceleration;
                topSpeed = defaultTopSpeed;
                deceleration = defaultDeceleration;
                jumpForce = defaultJumpForce;

                prevScale = currentScale;
                currentScale = defaultScale;
                break;
            case 2:
                transform.localScale = Vector2.one * bigScale;
                currentSize = 2;
                acceleration = bigAcceleration;
                topSpeed = bigTopSpeed;
                deceleration = bigDeceleration;
                jumpForce = bigJumpForce;

                prevScale = currentScale;
                currentScale = bigScale;
                break;
        }

        transform.Translate(Vector2.up * ((currentScale - prevScale) / 2f));
    }

    void RayMoveFloor()
    {
        // Its a mess, i know
        // But it works

        Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y - (currentScale / 2f));
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.3f, groundLayer);
        if (hit)
        {
            transform.Translate(Vector2.down * hit.distance);
        }
        else
        {
            rayOrigin = new Vector2(transform.position.x + (capCollider.size.x / 2f), transform.position.y - (currentScale / 2f));
            hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.3f, groundLayer);
            if (hit)
            {
                transform.Translate(Vector2.down * hit.distance);
            }
            else
            {
                rayOrigin = new Vector2(transform.position.x - (capCollider.size.x / 2f), transform.position.y - (currentScale / 2f));
                hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.3f, groundLayer);
                if (hit)
                {
                    transform.Translate(Vector2.down * hit.distance);
                }
            }
        }            
    }

    IEnumerator JumpBuffer()
    {
        jumpBuffering = true;
        yield return new WaitForSeconds(jumpBufferTime);
        jumpBuffering = false;
    }
}
