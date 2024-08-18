using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class KeyTrigger : MonoBehaviour
{
    [SerializeField] Tilemap lockedExitTileMap;
    [SerializeField] NextLevelTrigger nextLevelTrigger;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.tag.Equals("Player")) { return; }

        lockedExitTileMap.ClearAllTiles();
        nextLevelTrigger.unlocked = true;

        Destroy(gameObject);
    }
}
