using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunParticleHitKill : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Kill());
    }

    IEnumerator Kill()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
