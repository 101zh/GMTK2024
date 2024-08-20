using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabGun : MonoBehaviour
{
    private void Start()
    {
        //FindFirstObjectByType<PlayerController>().transform.GetChild(1).gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if (collision.gameObject.tag.Equals("Player"))
        {
            collision.transform.GetChild(1).gameObject.SetActive(true);
            collision.gameObject.GetComponent<PlayerController>().hasGun = true;
            Destroy(gameObject);
        }
    }
}
