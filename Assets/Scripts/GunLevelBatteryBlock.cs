using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GunLevelBatteryBlock : MonoBehaviour
{
    public GameObject Block;
    public Vector2 blockPos;
    public GameObject[] Batteries;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitToGive());   
    }

    IEnumerator WaitToGive()
    {
        yield return new WaitForSeconds(36);

        Instantiate(Block, blockPos, Quaternion.Euler(0, 0, 0));
        foreach (GameObject b in Batteries)
            b.SetActive(true);

        for (int i = 0; i < 10; i++)
        {
            foreach (GameObject b in Batteries)
                b.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, i / 10f);
            yield return new WaitForSeconds(0.1f);
        }

        foreach (GameObject b in Batteries)
            b.GetComponent<BoxCollider2D>().enabled = true;
    }
}
