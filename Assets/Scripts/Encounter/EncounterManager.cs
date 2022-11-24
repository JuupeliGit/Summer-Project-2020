using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    public static EncounterManager instance;                //Singleton

    [SerializeField] private LootTable chestTable = null;
    [SerializeField] private GameObject chestParticle = null;

    private SpawnTable spawnTable = null;

    private void Awake()
    {
        if (instance == null)                               //Singleton
            instance = this;
        else if (instance != this)
            Destroy(this);

        FindObjectOfType<EnemyManager>().onEndBattle += FinishEncounter;
    }

    // Start an encounter.
    public void BeginEncounter()
    {
        EncounterType encounterID = RoomManager.instance.CurrentRoom.encounterType;

        switch(encounterID)
        {
            case EncounterType.Battle: // Battle
                FindObjectOfType<EnemyManager>().StartBattle(RoomManager.instance.GetFloor * 5, spawnTable);
                break;
            case EncounterType.Chest: // Chest
                RoomManager.instance.SetChest(true);
                break;
            case EncounterType.Shop: // Shop
                RoomManager.instance.SetShop(true);
                break;
            case EncounterType.Stairs: // Stairs
                RoomManager.instance.SetStairs(true);
                break;
        }

        if (encounterID != EncounterType.Battle)
            FinishEncounter();
    }

    // End encounter.
    public void FinishEncounter()
    {
        EncounterType encounterID = RoomManager.instance.CurrentRoom.encounterType;

        // Turn this room into an empty one if battle ended in this room.
        if (encounterID == EncounterType.Battle)
            RoomManager.instance.CurrentRoom.encounterType = EncounterType.Empty;

        RoomManager.instance.RefreshRoom();
    }

    // Open a chest
    public void OpenChest()
    {
        Instantiate(chestParticle, new Vector2(1.75f, -2f), Quaternion.identity);

        // Spawn a random item from the chest loot table.
        int floorLevel = RoomManager.instance.GetFloor;
        int itemLevel = floorLevel / 5 + 2;
        RoomManager.instance.SpawnItem(itemLevel, chestTable.GetRandomLootByFloor(floorLevel), 1, new Vector2(1.75f, -2.4f));

        SoundManager.instance.PlaySound("chest", 1f, 1f, 0f);
        RoomManager.instance.SetChest(false);
        RoomManager.instance.CurrentRoom.encounterType = EncounterType.Empty;

        RoomManager.instance.RefreshRoom();
    }

    public SpawnTable SetSpawnTable
    {
        set { spawnTable = value; }
    }
}
