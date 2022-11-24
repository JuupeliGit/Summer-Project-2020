using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom
{
    public EncounterType encounterType;
    public bool isMystery;

    private Item[] itemsInRoom;

    private int variationIndex;

    //Constructor
    public DungeonRoom()
    {
        encounterType = EncounterType.Empty;

        itemsInRoom = new Item[3];

        CurrentItemObjects = ListCurrentItems();

        variationIndex = Random.Range(0, 11);
    }

    // Set an item in the given slot.
    public void SetItem(Item item, int slot)
    {
        itemsInRoom[slot] = item;

        CurrentItemObjects = ListCurrentItems();
    }

    // Make a list of the current item keywords in this room.
    private Keyword[] ListCurrentItems()
    {
        if (itemsInRoom.Length > 0)
        {
            int nulllessLength = 0;
            for (int i = 0; i < itemsInRoom.Length; i++)
            {
                if (itemsInRoom[i] != null)
                    nulllessLength++;
            }

            Keyword[] currentItemObjects = new Keyword[nulllessLength];
            int j = 0;
            for (int i = 0; i < itemsInRoom.Length; i++)
            {
                if (itemsInRoom[i] != null)
                {
                    currentItemObjects[j] = itemsInRoom[i].GetKeyword;
                    j++;
                }
            }

            return currentItemObjects;
        }
        else
            return new Keyword[0];
    }

    public Item GetItem(int i)
    {
        return itemsInRoom[i];
    }

    public Keyword[] CurrentItemObjects
    {
        get;
        private set;
    }

    public int GetVariationIndex(int mod)
    {
        return variationIndex % mod;
    }
}
