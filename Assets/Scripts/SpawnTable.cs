using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpawnableEnemy
{
    public BaseEnemyStats enemy;
    public float weight;

    public BaseEnemyStats GetEnemy
    {
        get { return enemy; }
    }

    public float GetWeight
    {
        get { return weight; }
    }
}

[CreateAssetMenu(fileName = "New Spawn Table", menuName = "Table/Spawn Table")]
public class SpawnTable : ScriptableObject
{
    [SerializeField] SpawnableEnemy[] enemies = null;

    // Returns a random base enemy based on weights.
    public BaseEnemyStats GetRandomEnemy
    {
        get
        {
            float totalWeight = 0;
            for (int i = 0; i < enemies.Length; i++)
                totalWeight += enemies[i].GetWeight;

            // Random number between 0 and sum of all weights.
            float r = Random.Range(1f, totalWeight);

            float currentWeight = 0f;

            // Find an enemy with larger weight total than what the random number was.
            for (int i = 0; i < enemies.Length; i++)
            {
                if (r < currentWeight + enemies[i].GetWeight)
                    return enemies[i].GetEnemy;
                else
                    currentWeight += enemies[i].GetWeight;
            }

            return null;
        }
    }
}
