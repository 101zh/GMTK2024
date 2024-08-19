using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    CapsuleCollider2D capCollider;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [HideInInspector]
    public AudioManager audioManager;

    [SerializeField] private bool hasGun = true;
    [HideInInspector] public bool isSelectingTarget = false;
    [SerializeField] private GameObject gun;
    private Vector3 gunScale = Vector3.one;

    public LayerMask groundLayer;
    public Transform groundDetectionOrigin;
    public bool isGrounded = false;

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
    [HideInInspector]
    public float currentScale = 1.0f; // Float equal to either smallScale, defaultScale, or bigScale

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

    [HideInInspector]
    public bool shouldMove = false;

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

    private enum AnimState { KiraIdle, KiraJump, KiraLand, KiraWalk };

    [HideInInspector]
    public bool shouldJump;
    [HideInInspector]
    public bool isJumping;
    bool jumpBuffering = false;
    private string currentAnimationState;

    private void Awake()
    {
        audioManager = GameObject.FindObjectOfType<AudioManager>();
        rb = GetComponent<Rigidbody2D>();
        capCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentSize = 1;
    }

    private void Start()
    {
        transform.localScale = Vector2.one * defaultScale;
        currentSize = 1;
        acceleration = defaultAcceleration;
        topSpeed = defaultTopSpeed;
        deceleration = defaultDeceleration;
        jumpForce = defaultJumpForce;

        prevScale = currentScale;
        currentScale = defaultScale;
        transform.Translate(Vector2.up * ((currentScale - prevScale) / 2f));
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseManager.GameIsPaused) return;

        rb.gravityScale = gravity;
        Vector2 v = rb.velocity;

        moveInput = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(moveInput) >= inputThreshold)
            moveInputPrev = moveInput;

        updateAnimation(moveInput);

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
        if (!shouldMove && !isJumping && isGrounded) // && rb.constraints != RigidbodyConstraints2D.FreezeAll
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            RayMoveFloor();
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (isGrounded)
                shouldJump = true;
            else if (!jumpBuffering)
                StartCoroutine(JumpBuffer());
        }
        if (shouldJump)
        {
            audioManager.Play("Jump");
            Jump();
        }

        // Clamp Falling Speed
        if (Mathf.Abs(v.y) > terminalVelocity)
        {
            rb.velocity = new Vector2(v.x, Mathf.Sign(v.y) * terminalVelocity);
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
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
        if (PauseManager.GameIsPaused) return;

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
    public void ChangeSize(PadType pType)
    {
        switch (pType)
        {
            case PadType.BLUE:
                ChangeSize(0);
                break;
            case PadType.RED:
                ChangeSize(2);
                break;
        }
    }

    public void ChangeSize(int size)
    {
        switch (size)
        {
            case 0:
                audioManager.Play("Shrink");
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
                audioManager.Play("NormalSize");
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
                audioManager.Play("Grow");
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

    public void RayMoveFloor()
    {
        // Its a mess, i know
        // But it works

        float yOff = (currentScale / 5f);

        Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y - (currentScale / 2f) + yOff);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 2f, groundLayer);
        if (hit)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground") || 
                hit.collider.transform.position.y < transform.position.y - (currentScale / 2f))
                transform.Translate(Vector2.down * hit.distance + Vector2.up * yOff);
        }
        else
        {
            rayOrigin = new Vector2(transform.position.x + (capCollider.size.x / 2f), transform.position.y - (currentScale / 2f) + yOff);
            hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.3f, groundLayer);
            if (hit)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground") ||
                    hit.collider.transform.position.y < transform.position.y - (currentScale / 2f))
                    transform.Translate(Vector2.down * hit.distance + Vector2.up * yOff);
            }
            else
            {
                rayOrigin = new Vector2(transform.position.x - (capCollider.size.x / 2f), transform.position.y - (currentScale / 2f));
                hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.3f, groundLayer);
                if (hit)
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground") ||
                        hit.collider.transform.position.y < transform.position.y - (currentScale / 2f))
                        transform.Translate(Vector2.down * hit.distance + Vector2.up * yOff);
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

    private void updateAnimation(float dirX)
    {
        string state;

        if (dirX > 0)
        {
            spriteRenderer.flipX = false;
            gunScale.x = 1;
            state = nameof(AnimState.KiraWalk);
        }
        else if (dirX < 0)
        {
            spriteRenderer.flipX = true;
            gunScale.x = -1;
            state = nameof(AnimState.KiraWalk);
        }
        else
        {
            state = nameof(AnimState.KiraIdle);
        }

        if (hasGun && !isSelectingTarget) gun.transform.localScale = gunScale;
        else if (hasGun) gun.transform.localScale = Vector3.one;

        if (rb.velocity.y > 0.1f)
        {
            state = nameof(AnimState.KiraJump);
        }
        else if (rb.velocity.y < -0.1f)
        {
            state = nameof(AnimState.KiraLand);
        }
        ChangeAnimationState(state);
    }

    private void ChangeAnimationState(string newState)
    {
        if (currentAnimationState == newState) return;

        animator.Play(newState);

        currentAnimationState = newState;
    }

}
