using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalableObject : MonoBehaviour
{
    [Header("Scale")]
    public float defaultScale = 1.0f;
    public float bigScale = 1.5f;
    public float smallScale = 0.5f;
    int currentSize; // Int 0, 1, or 2
    float prevScale = 1.0f; // Float equal to either smallScale, defaultScale, or bigScale
    float currentScale = 1.0f; // Float equal to either smallScale, defaultScale, or bigScale

    // Size 0 = Shrink
    // Size 1 = Normal
    // Size 2 = Grow
    public void ChangeSize(int size)
    {
        switch (size)
        {
            case 0:
                transform.localScale = new Vector2(Mathf.Sign(transform.localScale.x) * smallScale, Mathf.Sign(transform.localScale.y) * smallScale);
                currentSize = 0;

                prevScale = currentScale;
                currentScale = smallScale;
                break;
            case 1:
                transform.localScale = new Vector2(Mathf.Sign(transform.localScale.x) * defaultScale, Mathf.Sign(transform.localScale.y) * defaultScale);
                currentSize = 1;

                prevScale = currentScale;
                currentScale = defaultScale;
                break;
            case 2:
                transform.localScale = new Vector2(Mathf.Sign(transform.localScale.x) * bigScale, Mathf.Sign(transform.localScale.y) * bigScale);
                currentSize = 2;

                prevScale = currentScale;
                currentScale = bigScale;
                break;
        }

        transform.Translate(Vector2.up * ((currentScale - prevScale) / 4f));
    }
}
