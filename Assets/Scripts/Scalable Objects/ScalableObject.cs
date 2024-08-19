using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ScalableObject : MonoBehaviour
{
    [Header("Scale")]
    public float defaultScale = 1.0f;
    public float bigScale = 1.5f;
    public float smallScale = 0.5f;
    int currentSize; // Int 0, 1, or 2
    float prevScale = 1.0f; // Float equal to either smallScale, defaultScale, or bigScale
    float currentScale = 1.0f; // Float equal to either smallScale, defaultScale, or bigScale'

    Vector2 startPos;
    Quaternion startRotation;

    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startRotation = transform.rotation;
        startPos = transform.position;
    }

    private void Update()
    {
        if (transform.position.magnitude > 30f)
        {
            rb.velocity = Vector2.zero;
            transform.rotation = startRotation;
            transform.position = startPos;
        }
    }

    // Size 0 = Shrink
    // Size 1 = Normal
    // Size 2 = Grow
    public void ChangeSize(int size)
    {
        float scaleDiff;

        switch (size)
        {
            case 0:
                scaleDiff = smallScale - currentScale;
                transform.Translate(Vector2.up * (scaleDiff / 4f) + Vector2.up * 0.01f);
                transform.localScale = new Vector2(Mathf.Sign(transform.localScale.x) * smallScale, Mathf.Sign(transform.localScale.y) * smallScale);

                currentSize = 0;

                prevScale = currentScale;
                currentScale = smallScale;

                rb.mass = smallScale;
                break;
            case 1:
                scaleDiff = defaultScale - currentScale;
                transform.Translate(Vector2.up * (scaleDiff / 4f) + Vector2.up * 0.01f);
                transform.localScale = new Vector2(Mathf.Sign(transform.localScale.x) * defaultScale, Mathf.Sign(transform.localScale.y) * defaultScale);

                currentSize = 1;

                prevScale = currentScale;
                currentScale = defaultScale;

                rb.mass = defaultScale;
                break;
            case 2:
                scaleDiff = bigScale - currentScale;
                transform.Translate(Vector2.up * (scaleDiff / 4f) + Vector2.up * 0.01f);

                transform.localScale = new Vector2(Mathf.Sign(transform.localScale.x) * bigScale, Mathf.Sign(transform.localScale.y) * bigScale);

                currentSize = 2;

                prevScale = currentScale;
                currentScale = bigScale;

                rb.mass = bigScale;
                break;
        }
    }
}
