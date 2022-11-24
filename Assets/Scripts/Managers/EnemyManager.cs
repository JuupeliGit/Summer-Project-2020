using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using Random = UnityEngine.Random;      //UUSI ASIA

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] enemyRenderers = null;
    private Transform[] slots;

    private List<Enemy> currentEnemies = new List<Enemy>();

    [SerializeField] private GameObject deathParticle = null;
    [SerializeField] private GameObject hurtParticle = null;

    private NametagManager nametagManager;

    public static EnemyManager instance;

    public delegate void OnEndBattle();
    public event OnEndBattle onEndBattle;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        nametagManager = FindObjectOfType<NametagManager>();

        CurrentEnemyObjects = ListCurrentEnemies();
    }

    // Make a list of all the keywords from current enemies.
    private Keyword[] ListCurrentEnemies()
    {
        Keyword[] currentEnemyObjects = new Keyword[currentEnemies.Count];
        for(int i = 0; i < currentEnemies.Count; i++)
        {
            currentEnemyObjects[i] = currentEnemies[i].GetKeyword;
        }

        return currentEnemyObjects;
    }

    // Start a battle of certain difficulty level.
    public void StartBattle(int lvl, SpawnTable spawnTable)
    {
        // Decide how many enemies to spawn at random.
        int enemyAmount = Random.Range(1, 4);
        SpawnEnemies(lvl, enemyAmount, spawnTable);

        CurrentEnemyObjects = ListCurrentEnemies();

        StartCoroutine("ActionLoop");
    }

    // Spawn a set amount of enemies of given level from given spawn table.
    private void SpawnEnemies(int lvl, int amount, SpawnTable spawnTable)
    {
        // Hide enemies that are not needed. (if less than 3 enemies)
        for (int i = 0; i < enemyRenderers.Length; i++)
            enemyRenderers[i].sprite = null;

        for (int i = 0; i < amount; i++)
        {
            BaseEnemyStats baseStats = null;
            Enemy enemy = null;

            bool alreadyExists = false;

            // Try to spawn a unique enemy.
            do
            {
                baseStats = spawnTable.GetRandomEnemy;
                enemy = new Enemy(baseStats, lvl, i);

                // Check if an enemy with the same name already exists.
                alreadyExists = false;
                foreach (Enemy e in currentEnemies)
                {
                    if (e.GetKeyword.GetWord == enemy.GetKeyword.GetWord)
                        alreadyExists = true;
                }
            }
            while (alreadyExists);

            // Get nametag position for the enemy.
            Vector2 nametagPos = new Vector2(enemyRenderers[i].transform.parent.position.x, -0.5f);

            // Show nametag and reset values for health- and energybars
            nametagManager.UpdateEnemyHealthBar(enemy.GetPos, enemy.GetHealthPercent);
            nametagManager.UpdateEnemyEnergyBar(enemy.GetPos, enemy.GetEnergyPercent);
            nametagManager.ShowNametag(false, i, nametagPos, enemy.GetKeyword, -1);

            // Set the sprite for the visual enemy gameObject.
            enemyRenderers[i].sprite = baseStats.GetSprite;
            enemyRenderers[i].gameObject.SetActive(true);

            enemy.onDeath += RemoveEnemy;
            enemy.onAttack += EnemyAttack;

            currentEnemies.Add(enemy);
        }
    }

    //Ticks 4 times in 1 second.
    IEnumerator ActionLoop()
    {
        while (currentEnemies.Count > 0 && !GameOver.instance.GetGameOver)
        {
            for (int i = 0; i < currentEnemies.Count; i++)
            {
                nametagManager.UpdateEnemyEnergyBar(currentEnemies[i].GetPos, 1f - currentEnemies[i].GetEnergyPercent);
                currentEnemies[i].ReduceStepCount();
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    // Remove an enemy. This is called when an enemy dies.
    public void RemoveEnemy(Enemy enemy)
    {
        enemy.onDeath -= RemoveEnemy;

        // Get the position of the enemy's sprite.
        Vector3 pos = enemyRenderers[enemy.GetPos].transform.position;

        // Hide the enemy's sprite
        enemyRenderers[enemy.GetPos].gameObject.SetActive(false);

        // Spawn particles where the enemy stands.
        Instantiate(deathParticle, pos + new Vector3(0f, 1f), Quaternion.identity);

        // Hide it's name tag
        nametagManager.HideNametag(false, enemy.GetPos);

        // Spawn loot item if it had one.
        if(enemy.GetLoot != null)
            RoomManager.instance.SpawnItem(enemy.GetLevel / 20 - 1, enemy.GetLoot, enemy.GetPos, pos);

        // Spawn Coins
        int coinAmount = 1 + (int)(enemy.GetLevel * 0.05f) + Random.Range(-1, 1);
        StartCoroutine(RoomManager.instance.SpawnCoin(coinAmount, pos + new Vector3(0f, 1f)));

        currentEnemies.Remove(enemy);
        CurrentEnemyObjects = ListCurrentEnemies();

        //Send signal when all enemies are dead
        if (currentEnemies.Count <= 0)
        {
            currentEnemies.Clear();
            StopCoroutine("ActionLoop");

            onEndBattle();
        }
    }

    // Called when an enemy attacks.
    public void EnemyAttack(Enemy enemy, int damage)
    {
        StartCoroutine(AttackProgress(enemy, damage));
    }

    // If the enemy dies before connecting it's attack, it won't deal any damage.
    private IEnumerator AttackProgress(Enemy enemy, int damage)
    {
        enemyRenderers[enemy.GetPos].GetComponent<Animator>().SetTrigger("Attack");

        yield return new WaitForSeconds(0.25f);

        if (enemy != null && enemy.GetHealthPercent > 0)
            PlayerManager.instance.ModifyHealth(-damage);
    }


    // Modify an enemy's health.
    public void ModifyEnemyHealth(Enemy enemy, int amount)
    {
        enemy.ModifyHealth(amount);

        // Update the enemy's healthbar.
        nametagManager.UpdateEnemyHealthBar(enemy.GetPos, enemy.GetHealthPercent);

        // Play sound and animation & spawn particles if amount is less than 0.
        if (amount < 0)
        {
            Instantiate(hurtParticle, enemyRenderers[enemy.GetPos].transform.position + new Vector3(0f, 1f), Quaternion.identity);

            SoundManager.instance.PlaySound("damage", 1f, 1f, 0.5f);

            enemyRenderers[enemy.GetPos].GetComponent<Animator>().SetTrigger("Hurt");
        }
    }

    // Find an enemy with the given name.
    public Enemy GetEnemyByName(string name)
    {
        for (int i = 0; i < currentEnemies.Count; i++)
        {
            if (currentEnemies[i].GetKeyword.GetWord == name)
            {
                Enemy temp = currentEnemies[i];
                return temp;
            }
        }

        return null;
    }

    public Keyword[] CurrentEnemyObjects
    { 
        get; 
        private set; 
    }
}
