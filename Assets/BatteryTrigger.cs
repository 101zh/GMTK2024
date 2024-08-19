using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BatteryTrigger : MonoBehaviour
{

    public static event UnityAction pickupBattery;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.tag.Equals("Player")) return;

        if (!other.GetComponent<PlayerController>().transform.GetChild(1).GetComponent<GunController>().canShoot)
        {
            pickupBattery.Invoke();
            Destroy(gameObject);
        }
    }

}
