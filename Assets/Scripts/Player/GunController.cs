using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GunController : MonoBehaviour
{
    public Transform shootOrigin;
    public LayerMask scalableLayer;
    public GameObject hitParticles;

    public Color smallColor;
    public Color defaultColor;
    public Color largeColor;

    public Material gunBlue;
    public Material gunGreen;
    public Material gunRed;
    public Material gunNoGlow;
    public SpriteRenderer sr;
    Vector4 currentGlowColor;

    public Material OutlineMat;
    public Material SpriteLitDefaultMat;    

    PlayerController playerController;

    // Mode 0 = Shrink
    // Mode 1 = Normal
    // Mode 2 = Grow
    public int mode = 0;

    public float rayVisibleTime = 0.5f;

    bool isHovering = false;
    ScalableObject selected = null;

    Vector3 pOne = Vector3.zero;
    Vector3 pTwo = Vector3.zero;

    LineRenderer lr;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        playerController = transform.parent.GetComponent<PlayerController>();

        ModeChange(0);
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfSelectingBlock();

        if (isHovering)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StopCoroutine(Shoot());
                StartCoroutine(Shoot());
                selected.ChangeSize(mode);
            }
        }

        pOne = shootOrigin.position;
        lr.SetPosition(0, pOne);
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ModeChange(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ModeChange(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            ModeChange(2);
    }

    public void ModeChange(int m)
    {
        StopCoroutine(Shoot());

        mode = m;
        switch (mode)
        {
            case 0:
                sr.material = gunBlue;
                break;
            case 1:
                sr.material = gunGreen;
                break;
            case 2:
                sr.material = gunRed;
                break;
        }
        currentGlowColor = sr.material.color;
    }

    void LookAtSelected()
    {   
        
        var dir = selected.transform.position - transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        if (Mathf.Abs(angle) > 100) 
            transform.localScale = new Vector3(1, -1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }

    void CheckIfSelectingBlock()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 100, scalableLayer);

        if (hit.collider != null )
        {
            isHovering = true;
            if (selected != null && !hit.collider.gameObject.Equals(selected.gameObject)) { 
                selected.GetComponent<SpriteRenderer>().sortingOrder = 3;
                selected.GetComponent<SpriteRenderer>().material = SpriteLitDefaultMat;
            }
            selected = hit.collider.gameObject.GetComponent<ScalableObject>();
            selected.GetComponent<SpriteRenderer>().sortingOrder = 4;
            selected.GetComponent<SpriteRenderer>().material = OutlineMat;
            LookAtSelected();
        }
        else
        {
            isHovering = false;
            if (selected != null)
            {
                selected.GetComponent<SpriteRenderer>().sortingOrder = 3;
                selected.GetComponent<SpriteRenderer>().material = SpriteLitDefaultMat;
            }
            selected = null;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    IEnumerator Shoot()
    {
        playerController.audioManager.Play("LaserShoot");

        sr.material.color = currentGlowColor * 5;

        yield return new WaitForEndOfFrame();

        lr.enabled = true;
        sr.material.color = currentGlowColor * 10;

        pTwo = selected.gameObject.transform.position;
        lr.SetPosition(1, pTwo);

        GameObject particle = Instantiate(hitParticles, pTwo, Quaternion.Euler(0, 0, 0));
        var particleSystemMain = particle.GetComponent<ParticleSystem>().main;

        switch (mode)
        {
            case 0:
                playerController.audioManager.Play("Shrink");
                lr.startColor = smallColor;
                lr.endColor = smallColor;
                particleSystemMain.startColor = smallColor;
                particleSystemMain.startSize = 0.4f;
                break;
            case 1:
                playerController.audioManager.Play("NormalSize");
                lr.startColor = defaultColor;
                lr.endColor = defaultColor;
                particleSystemMain.startColor = defaultColor;
                particleSystemMain.startSize = 0.9f;
                break;
            case 2:
                playerController.audioManager.Play("Grow");
                lr.startColor = largeColor;
                lr.endColor = largeColor;
                particleSystemMain.startColor = largeColor;
                particleSystemMain.startSize = 1.5f;
                break;
        }

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        if (!playerController.shouldJump && !playerController.shouldMove && !playerController.isJumping)
            playerController.RayMoveFloor();

        yield return new WaitForSeconds(rayVisibleTime);
        lr.enabled = false;

        for (int i = 9; i > 0; i--)
        {
            sr.material.color = currentGlowColor * i;
            yield return new WaitForSeconds(0.02f);
        }
    }
}