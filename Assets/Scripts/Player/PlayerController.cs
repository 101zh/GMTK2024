using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;

    public LayerMask groundLayer;
    public Transform groundDetectionOrigin;
    bool isGrounded = false;

    public float inputThreshold = 0.05f;
    float moveInput;
    float moveInputPrev;

    [Header("Scale")]
    public float scaleFactor;

    [Header("Walking")]
    public float acceleration;
    public float topSpeed;
    public float deceleration;
    public float horizontalSpeedThreshold = 0.0f;

    [Header("Gravity")]
    public float gravity;
    public float terminalVelocity;

    [Header("Jumping")]
    bool shouldJump;
    bool jumpBuffering = false;
    public float jumpForce;
    public float jumpBufferTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        if (Physics2D.OverlapBox(groundDetectionOrigin.position, new Vector2(scaleFactor * 0.9f, 0.1f * scaleFactor), 0, groundLayer))
        {
            isGrounded = true;

            // Jump Buffer Check
            if (jumpBuffering)
            {
                StopCoroutine(JumpBuffer());
                jumpBuffering = false;
                shouldJump = true;
            }
        }
        else
            isGrounded = false;

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
    }

    private void FixedUpdate()
    {
        // -- Horizontal Movement -- //
        
        Vector2 v = rb.velocity;

        if (Mathf.Abs(moveInput) >= inputThreshold)
        {
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
                rb.velocity = new Vector2(0, v.y);
                moveInputPrev = 0;
            }
        }
    }

    void Jump()
    {
        shouldJump = false;
        jumpBuffering = false; // Just in case
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void ChangeSize(float size)
    {
        scaleFactor = size;
        transform.localScale = Vector2.one * scaleFactor;
    }

    IEnumerator JumpBuffer()
    {
        jumpBuffering = true;
        yield return new WaitForSeconds(jumpBufferTime);
        jumpBuffering = false;
    }
}
