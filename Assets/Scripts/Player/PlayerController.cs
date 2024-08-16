using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;

    public float inputThreshold = 0.05f;
    float moveInput;
    float moveInputPrev;

    [Header("Walking")]
    public float acceleration;
    public float topSpeed;
    public float deceleration;
    public float horizontalSpeedThreshold = 0.0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(moveInput) >= inputThreshold)
            moveInputPrev = moveInput;
    }

    private void FixedUpdate()
    {
        // Horizontal Movement
        
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
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
    }
}
