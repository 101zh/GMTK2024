using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType { BLUE, RED, EMPTY, NULL };

public class TileMapTrigger : MonoBehaviour
{
    [SerializeField] private Tile usedUpTile;
    [SerializeField] private string tileColor;

    private Tilemap tilemap;
    [SerializeField] private List<TileLoc> tileLocations;
    private TileType thisTileType;
    private static TileType lastTileType = TileType.NULL;

    // Use this for initialization
    void Start()
    {
        if (tileColor.ToLower().Equals("blue")) { thisTileType = TileType.BLUE; }
        else if (tileColor.ToLower().Equals("red")) { thisTileType = TileType.RED; }
        else { throw new System.Exception("Invalid Color Tile Type"); }

        tilemap = GetComponent<Tilemap>();
        if (tileLocations.Count < 1)
        {
            tileLocations = new List<TileLoc>();
            foreach (var pos in tilemap.cellBounds.allPositionsWithin)
            {
                Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                if (tilemap.HasTile(localPlace))
                {
                    tileLocations.Add(new TileLoc(localPlace, usedUpTile));
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.tag.Equals("Player")) { return; }

        Vector3Int cellPos = tilemap.WorldToCell(other.transform.position);

        float lowestDistance = Vector3Int.Distance(cellPos, tileLocations[0].loc);
        int index = 0;

        for (int i = 1; i < tileLocations.Count; i++)
        {
            float distance = Vector3Int.Distance(cellPos, tileLocations[i].loc);
            if (distance < lowestDistance)
            {
                index = i;
                lowestDistance = distance;
            }
        }

        if (!tileLocations[index].usedUp)
        {
            tilemap.SetTile(tileLocations[index].loc, tileLocations[index].replacementTile);
            tileLocations[index].usedUp = true;
            lastTileType = thisTileType;
        }
        else
        {
            lastTileType = TileType.EMPTY;
        }

        Debug.Log(getLastTileTriggered());
    }

    [System.Serializable]
    private class TileLoc
    {
        public Vector3Int loc;
        public Tile replacementTile;
        public bool usedUp;

        public TileLoc(Vector3Int loc, Tile replacementTile)
        {
            this.loc = loc;
            usedUp = false;
            this.replacementTile = replacementTile;
        }
    }

    public static TileType getLastTileTriggered()
    {
        return lastTileType;
    }
}
