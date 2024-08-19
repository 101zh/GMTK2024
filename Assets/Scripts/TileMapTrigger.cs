using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum PadType { BLUE, RED, GREEN, EMPTY, NULL };

public class TileMapTrigger : MonoBehaviour
{
    [SerializeField] private Tile usedUpPad;
    [SerializeField] private string padColor;

    private Tilemap tilemap;
    [SerializeField] private List<TileLoc> tileLocations;
    private PadType thisTileMapPadType;
    private static PadType lastPadType = PadType.NULL;

    // Use this for initialization
    void Start()
    {
        if (padColor.ToLower().Equals("blue")) { thisTileMapPadType = PadType.BLUE; }
        else if (padColor.ToLower().Equals("red")) { thisTileMapPadType = PadType.RED; }
        else if (padColor.ToLower().Equals("green")) { thisTileMapPadType = PadType.GREEN; }
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
                    tileLocations.Add(new TileLoc(localPlace, usedUpPad));
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

        if (!tileLocations[index].empty)
        {
            tilemap.SetTile(tileLocations[index].loc, tileLocations[index].replacementTile);
            tileLocations[index].empty = true;
            lastPadType = thisTileMapPadType;
        }
        else
        {
            lastPadType = PadType.EMPTY;
        }

        // Update Player
        other.GetComponent<PlayerController>().ChangeSize(lastPadType);

        Debug.Log(getLastPadTriggered());
    }

    [System.Serializable]
    private class TileLoc
    {
        public Vector3Int loc;
        public Tile replacementTile;
        public bool empty;

        public TileLoc(Vector3Int loc, Tile replacementTile)
        {
            this.loc = loc;
            empty = false;
            this.replacementTile = replacementTile;
        }

        public TileLoc(Vector3Int loc, Tile replacementTile, bool empty)
        {
            this.loc = loc;
            this.empty = empty;
            this.replacementTile = replacementTile;
        }
    }

    public static PadType getLastPadTriggered()
    {
        return lastPadType;
    }
}
