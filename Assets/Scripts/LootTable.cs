using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DroppableItem
{
    public BaseItemStats item;
    public float weight;

    public BaseItemStats GetItem
    {
        get { return item; }
    }

    public float GetWeight
    {
        get { return weight; }
    }
}

[CreateAssetMenu(fileName = "New Loot Table", menuName = "Table/Loot Table")]
public class LootTable : ScriptableObject
{
    [SerializeField] DroppableItem[] loot = null;

    // Returns a random base item based on weights.
    public BaseItemStats GetRandomLoot
    {
        get
        {
            float totalWeight = 0f;
            for (int i = 0; i < loot.Length; i++)
                totalWeight += loot[i].GetWeight;

            // Random number between 0 and sum of all weights.
            float r = Random.Range(1f, totalWeight);

            float currentWeight = 0f;

            // Find an item with larger weight total than what the random number was.
            for (int i = 0; i < loot.Length; i++)
            {
                if (r < currentWeight + loot[i].GetWeight)
                    return loot[i].GetItem;
                else
                    currentWeight += loot[i].GetWeight;
            }

            return null;
        }
    }

    public BaseItemStats GetRandomLootByFloor(int floorLevel)
    {
        int floorWeight = Mathf.FloorToInt((float)floorLevel / 3.3f);    //This could be made more modular??
        floorWeight = Mathf.Clamp(floorWeight, 0, 5);

        float totalWeight = 0f;
        for (int i = floorWeight; i < floorWeight + 4; i++)
            totalWeight += loot[i].GetWeight;

        // Random number between 1 and sum of all weights.
        float r = Random.Range(1f, totalWeight);

        float currentWeight = 0f;

        // Find an item with larger weight total than what the random number was.
        for (int i = floorWeight; i < floorWeight + 4; i++)   
        {
            if (r < currentWeight + loot[i].GetWeight)
                return loot[i].GetItem;
            else
                currentWeight += loot[i].GetWeight;
        }

        return null;
    }
}
