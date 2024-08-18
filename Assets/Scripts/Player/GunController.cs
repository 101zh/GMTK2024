using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform shootOrigin;
    public LayerMask scalableLayer;

    // Mode 0 = Shrink
    // Mode 1 = Normal
    // Mode 2 = Grow
    public int mode = 0;

    bool isHovering = false;
    ScalableObject selected = null;

    // Update is called once per frame
    void Update()
    {
        CheckIfSelectingBlock();

        if (isHovering)
        {
            if (Input.GetMouseButtonDown(0))
            {
                selected.ChangeSize(mode);
            }
        }
    }

    void LookAtMouse()
    {
        var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        if (Mathf.Abs(angle) > 100)
            transform.localScale = new Vector3(1, -1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);

        Debug.Log("Looking");
    }

    void CheckIfSelectingBlock()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            isHovering = true;
            selected = hit.collider.gameObject.GetComponent<ScalableObject>();
            LookAtMouse();
        }
        else
        {
            isHovering = false;
            selected = null;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}