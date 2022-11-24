using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CommandExecution : MonoBehaviour
{
    InventoryManager invManager;
    DungeonGeneration dungeonGeneration;
    EnemyManager enemyManager;

    private void Start()
    {
        invManager = FindObjectOfType<InventoryManager>();
        dungeonGeneration = FindObjectOfType<DungeonGeneration>();
        enemyManager = FindObjectOfType<EnemyManager>();
    }

    public void CallCommand(string verb, string target)
    {
        // Call a function based on which verb was typed.
        if (verb == "use")
            Use(target);
        else if (verb == "take")
            Take(target);
        else if (verb == "equip")
            Equip(target);
        else if (verb == "drop")
            Drop(target);
        else if (verb == "buy")
            Buy(target);
        else if (verb == "sell")
            Sell(target);
        else if (verb == "attack")
            Attack(target);
        else if (verb == "open")
            Open(target);
        else if (verb == "walk")
            Walk(target);
        else if (verb == "inspect")
            Inspect(target);
    }

    // Use items on the floor or from inventory. Floor is checked for valid targets first.
    private void Use(string target)
    {
        Item item = RoomManager.instance.GetItemByName(target);
        if (RoomManager.instance.CurrentRoom.encounterType != EncounterType.Shop && item != null && item.GetItemType == ItemType.Consumable)
        {
            invManager.UseItem(item);
            RoomManager.instance.RemoveItem(item);
            return;
        }
        item = invManager.GetItemByName(target);
        if (item != null && item.GetItemType == ItemType.Consumable)
        {
            invManager.UseItem(item);
            return;
        }
    }

    // Equip items on the floor or from inventory. Floor is checked for valid targets first.
    private void Equip(string target)
    {
        Item item = RoomManager.instance.GetItemByName(target);
        if (invManager.GetItemCount < 7 && RoomManager.instance.CurrentRoom.encounterType != EncounterType.Shop && item != null)
        {
            if (invManager.EquipItem(item))
            {
                RoomManager.instance.RemoveItem(item);
                return;
            }
        }
        item = invManager.GetItemByName(target);
        if (invManager.EquipItem(item))
            return;
    }

    // Take items that are on the floor. The item is removed from the room and added to inventory.
    private void Take(string target)
    {
        Item item = RoomManager.instance.GetItemByName(target);
        if (RoomManager.instance.CurrentRoom.encounterType != EncounterType.Shop && item != null)
        {
            if (invManager.AddItem(item))
            {
                RoomManager.instance.RemoveItem(item);
                SoundManager.instance.PlaySound("item_pickup", 0.5f, 1f, 0.2f);

                return;
            }
        }
    }

    // Drop items from inventory. The items is deleted forever.
    private void Drop(string target)
    {
        Item item = invManager.GetItemByName(target);
        if (item != null)
            invManager.RemoveItem(item);
        else
            EventLog.instance.Print("i don't have " + target);
    }

    // Buy items on the floor in exchange for money. Only valid in the shop room.
    // Health Upgrades are used instantly after buying, they don't require any inventory space.
    private void Buy(string target)
    {
        Item item = RoomManager.instance.GetItemByName(target);
        if (item != null && RoomManager.instance.CurrentRoom.encounterType == EncounterType.Shop)
        {
            int price = (int)(item.GetBuyPrice);
            if (price <= PlayerManager.instance.CoinAmount)
            {
                if (target == "health upgrade")
                {
                    PlayerManager.instance.ModifyCoin(-price);
                    PlayerManager.instance.IncreaseMaxHp(5);
                    RoomManager.instance.RemoveItem(item);

                    SoundManager.instance.PlaySound("item_pickup", 1f, 1f, 0.2f);

                    return;
                }

                else if (invManager.AddItem(item))
                {
                    PlayerManager.instance.ModifyCoin(-price);
                    RoomManager.instance.RemoveItem(item);

                    SoundManager.instance.PlaySound("item_pickup", 1f, 1f, 0.2f);

                    return;
                }
                else
                    EventLog.instance.Print("my inventory is full!");
            }
            else
                EventLog.instance.Print("it's too expensive!");
        }
        else
            EventLog.instance.Print("can't buy that here");
    }

    // Sell items from inventory in exchange for money. Only valid in the shop room.
    // Sold items are deleted forever.
    private void Sell(string target)
    {
        Item item = invManager.GetItemByName(target);
        if (item != null  && RoomManager.instance.CurrentRoom.encounterType == EncounterType.Shop)
        {
            PlayerManager.instance.ModifyCoin(item.GetSellPrice);
            invManager.RemoveItem(item);
        }
        else
            EventLog.instance.Print("there's no one to sell to");
    }

    // Attack an enemy. Equipped item's damage value is added to the player's base damage.
    private void Attack(string target)
    {
        Enemy enemy = enemyManager.GetEnemyByName(target);
        Item equippedItem = invManager.GetEquippedItem;
        int damage = 1 + (equippedItem == null ? 0 : equippedItem.GetDamage);

        if (enemy != null)
        {
            enemyManager.ModifyEnemyHealth(enemy, -damage);
            EventLog.instance.Print(enemy.GetKeyword.GetWord + " took " + damage + " damage");
        }
        else
            EventLog.instance.Print("there's no " + target);
    }

    // Open a chest. Only valid in the chest room.
    private void Open(string target)
    {
        if (target == "chest" && RoomManager.instance.CurrentRoom.encounterType == EncounterType.Chest)
        {
            EncounterManager.instance.OpenChest();
        }
        else
            EventLog.instance.Print("there's no " + target);
    }

    // Inspect items on the floor or in inventory. Floor is checked for valid targets first.
    // The item's name & stats are printed in the Event Log.
    private void Inspect(string target)
    {
        Item item = invManager.GetItemByName(target);
        if (item != null)
        {
            invManager.ShowItemStats(item);
            return;
        }

        item = RoomManager.instance.GetItemByName(target);
        if (item != null)
        {
            invManager.ShowItemStats(item);
            return;
        }

        EventLog.instance.Print("i don't have " + target);
    }

    // Switch to another room or to the next floor.
    // Each direction is valid only if a room exists there. Walk is valid only in the stair room.
    private void Walk(string target)
    {
        if (target == "north")
            RoomManager.instance.MovePlayer(0);
        else if (target == "south")
            RoomManager.instance.MovePlayer(1);
        else if (target == "west")
            RoomManager.instance.MovePlayer(2);
        else if (target == "east")
            RoomManager.instance.MovePlayer(3);
        else if (target == "down" && RoomManager.instance.CurrentRoom.encounterType == EncounterType.Stairs)
            dungeonGeneration.GenerateDungeon();
        else
            EventLog.instance.Print("i can't walk " + target);
    }
}
