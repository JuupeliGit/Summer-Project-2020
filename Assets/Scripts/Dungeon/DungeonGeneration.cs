using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;

public enum EncounterType
{
    NULL,
    Empty,
    Battle,
    Chest,  
    Shop,
    Stairs,
}

public class DungeonGeneration : MonoBehaviour
{
    public static DungeonGeneration instance;

    [SerializeField] int sizeX = default;
    [SerializeField] int sizeY = default;

    private DungeonRoom[,] rooms;
    private Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    [SerializeField] private LootTable[] shopTable = null;
    [SerializeField] private BaseItemStats hpUpgrade = null;


    public delegate void OnGenerateDungeon();
    public event OnGenerateDungeon onGenerateDungeon;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    private void Start()
    {
        GenerateDungeon();
    }

    public void GenerateDungeon()
    {
        //Create array
        rooms = new DungeonRoom[sizeX, sizeY];

        //Create an empty room at the center. Player always spawns in this room.
        rooms[sizeX / 2, sizeY / 2] = new DungeonRoom();

        Vector2Int toBeColored = Vector2Int.zero;

        for (int k = 0; k < 10;)
        {
            int x = 0;
            int y = 0;

            // Find a random room
            do
            {
                x = Random.Range(0, sizeX);
                y = Random.Range(0, sizeY);
            }
            while (rooms[x, y] == null);

            toBeColored = new Vector2Int(x, y);
            ShuffleDirection();
            int hallwayLength = Random.Range(1, 3);

            //Tries to make a hallway the length of 1-2
            for (int j = 0; j < hallwayLength; j++)
            {
                bool addedRoom = false;

                for (int i = 0; i < directions.Length; i++)
                {
                    //Check if the tile in given direction is empty and has only 1 adjancent room
                    Vector2Int nextRoom = toBeColored + directions[i];
                    if (rooms.CheckForValue(nextRoom, null) && CheckAdjacent(nextRoom) < 2)
                    {
                        toBeColored += directions[i];
                        //Default encounter type is Battle.
                        rooms[toBeColored.x, toBeColored.y] = new DungeonRoom();
                        rooms[toBeColored.x, toBeColored.y].encounterType = EncounterType.Battle;
                        addedRoom = true;
                        break;
                    }
                }
                //Break if can't make any more rooms
                if (!addedRoom)
                    break;
                else
                    k++;
            }
        }

        for (int i = 0; i < 3; i++)
            SetChestRoom();

        SetStairRoom();
        SetShopRoom();

        for (int i = 0; i < 4; i++)
            SetMysteryRoom();

        //Pass layout to the delegate (subscribed by minimap script)
        Minimap.instance.SetDungeonLayout(rooms);
        onGenerateDungeon();
    }

    // Set stairs room somwhere not close to the player, and not in a deadend.
    private void SetStairRoom()
    {
        int x = 0;
        int y = 0;
        float dst = 0f;
        int tries = 100;

        do
        {
            x = Random.Range(0, sizeX);
            y = Random.Range(0, sizeY);
            dst = Vector2.Distance(new Vector2(x, y), new Vector2(sizeX / 2, sizeY / 2));

            tries--;
        }
        while (rooms[x, y] == null || CheckAdjacent(new Vector2(x, y)) <= 1 || (dst <= 1.5f && tries > 0));

        rooms[x, y].encounterType = EncounterType.Stairs;
    }

    // Set chest room in a random location.
    private void SetChestRoom()
    {
        int x = 0;
        int y = 0;

        do
        {
            x = Random.Range(0, sizeX);
            y = Random.Range(0, sizeY);
        }
        while ((x == 2 && y == 2) || rooms[x, y] == null);

        rooms[x, y].encounterType = EncounterType.Chest;
    }

    // Set shop room in a random location & then randomize it's items.
    private void SetShopRoom()
    {
        int x = 0;
        int y = 0;
        float dst = 0f;
        int tries = 100;

        do
        {
            x = Random.Range(0, sizeX);
            y = Random.Range(0, sizeY);
            dst = Vector2.Distance(new Vector2(x, y), new Vector2(sizeX / 2, sizeY / 2));

            tries--;
        }
        while ((x == 2 && y == 2) || rooms[x, y] == null || rooms[x,y].encounterType == EncounterType.Stairs || CheckAdjacent(new Vector2(x, y)) <= 1 || (dst <= 1.5f && tries > 0));

        rooms[x, y].encounterType = EncounterType.Shop;

        RandomizeShopItems(rooms[x, y]);
    }

    // Set random rooms to be mystery rooms.
    // They are marked as question marks in the minimap instead of their normal icon.
    // Stair rooms cannot be mystery rooms.
    private void SetMysteryRoom()
    {
        int x = 0;
        int y = 0;

        do
        {
            x = Random.Range(0, sizeX);
            y = Random.Range(0, sizeY);
        }
        while ((x == 2 && y == 2) || rooms[x, y] == null || rooms[x, y].encounterType == EncounterType.Stairs);

        rooms[x, y].isMystery = true;
    }

    // Randomize items in a shop room.
    // One of the items is always a health upgrade.
    private void RandomizeShopItems(DungeonRoom room)
    {
        for (int i = 0; i < 3; i++)
        {
            Vector2 spawnPos = new Vector2(0.5f + 1.25f * i, -1.6f - 0.3f * i);

            Item newItem = null;

            if (i == 1)
                newItem = new Item(shopTable[0].GetRandomLoot, RoomManager.instance.GetFloor / 5 + 1);

            else if (i == 2)
                newItem = new Item(shopTable[1].GetRandomLootByFloor(RoomManager.instance.GetFloor), RoomManager.instance.GetFloor / 5 + 1);

            else
                newItem = new Item(hpUpgrade, 0);

            newItem.position = spawnPos;

            room.SetItem(newItem, i);
        }
    }

    // Returns the amount of adjacent rooms.
    private int CheckAdjacent(Vector2 pos)
    {
        int a = 0;
        for (int i = 0; i < 4; i++)
        {
            int x = (int)(pos.x + directions[i].x);
            int y = (int)(pos.y + directions[i].y);
            if (!rooms.IndexOutOfRange(new Vector2Int(x, y)))
            {
                if (rooms[x, y] != null)
                    a++;
            }
        }

        return a;
    }

    // Shuffle the directions array.
    private void ShuffleDirection()
    {
        for (int i = 3; i >= 0; i--)
        {
            Vector2Int temp = directions[i];
            int r = Random.Range(0, i);
            directions[i] = directions[r];
            directions[r] = temp;
        }
    }

    public DungeonRoom[,] GetRoomLayout
    {
        get { return rooms; }
    }

    public DungeonRoom RoomAtPosition(Vector2Int pos)
    {
        return rooms[pos.x, pos.y];
    }
}
