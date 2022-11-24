using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using ExtensionMethods;

public class Minimap : MonoBehaviour
{
    public static Minimap instance;

    [SerializeField] private Tile[] roomTiles = null;
    [SerializeField] private Tile playerTile = null;
    [SerializeField] private Tile darkTile = null;
    [SerializeField] private Tilemap[] tilemap = null;

    private DungeonRoom[,] roomLayout;
    private bool[,] seenLayout;

    Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right, Vector2Int.zero };

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        //Delegate subscriptions
        FindObjectOfType<RoomManager>().onPlayerMove += UpdateMinimap;
    }

    // Called after a new floor is generated.
    public void SetDungeonLayout(DungeonRoom[,] layout)
    {
        //Get the new dungeon layout when generating a new floor
        roomLayout = layout;
        seenLayout = new bool[layout.GetLength(0), layout.GetLength(1)];

        tilemap[0].ClearAllTiles();
    }

    // Called whenever the player switches rooms or when a room is refreshed.
    public void UpdateMinimap(Vector2Int playerPos)
    {
        if (roomLayout == null)
            return;

        tilemap[1].ClearAllTiles();

        //Check for "seen" rooms
        for (int i = 0; i < directions.Length; i++)
        {
            int x = directions[i].x + playerPos.x;
            int y = directions[i].y + playerPos.y;
            if (!roomLayout.IndexOutOfRange(new Vector2Int(x, y)) && !roomLayout.CheckForValue(new Vector2Int(x, y), null))
                seenLayout[x, y] = true;
        }
        

        //Sets all the roomtiles to correct tiles based on if the room has been "seen" or not
        for (int x = 0; x < roomLayout.GetLength(0); x++)
        {
            for (int y = 0; y < roomLayout.GetLength(1); y++)
            {
                //If the current tile in the loop has been "seen", set the corresponding tile
                if (seenLayout[x, y])
                {
                    int tileIndex = roomLayout[x,y].isMystery ? 5 : (int)roomLayout[x, y].encounterType - 1;
                    tilemap[0].SetTile(new Vector3Int(x, y, 0), roomTiles[tileIndex]);
                }
                else if (roomLayout[x, y] != null)
                    tilemap[0].SetTile(new Vector3Int(x, y, 0), darkTile);
            }
        }
        //Set the player tile on the secondary tilemap (tilemap[1])
        tilemap[1].SetTile(new Vector3Int(playerPos.x, playerPos.y, 0), playerTile);
    }
}
