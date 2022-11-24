using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Timeline;

public class Enemy
{
    private Keyword keyword = default;

    private int maxHp;
    private int curHp;
    private int atk;
    private int spd;
    private int level;
    private int stepCount;

    private BaseItemStats loot;

    private int myPos;

    public delegate void OnDeath(Enemy enemy);
    public event OnDeath onDeath;

    public delegate void OnAttack(Enemy enemy, int damage);
    public event OnAttack onAttack;

    public Enemy(BaseEnemyStats stats, int lvl, int pos)
    {
        int index = Random.Range(0, stats.GetPrefixes.Length);

        // Get a random prefix.
        string fullName = (stats.GetPrefixes[index] == "" || Random.Range(0f, 1f) < 0.5f ? "" : stats.GetPrefixes[index] + " ") + stats.name.ToLower();
        keyword = new Keyword(fullName, stats.GetColor);

        this.level = lvl;
        myPos = pos;

        RandomizeStats(stats);
    }

    // Randomize stats when spawned.
    private void RandomizeStats(BaseEnemyStats stats)           //Enemy level is divided by 100 so the stat curves range from 0 to 1
    {
        level = level + Random.Range(-5, 6);
        maxHp = (int)stats.GetHp.Evaluate(level / 100f);
        curHp = maxHp;
        atk = (int)stats.GetAtk.Evaluate(level / 100f);
        spd = 40 / (int)stats.GetSpd.Evaluate(level / 100f);
        stepCount = spd;

        // Get a random loot item.
        loot = stats.GetLootTable.GetRandomLoot;
        
        EventLog.instance.Print(keyword.GetWord + " atk: " + atk + '\n' + "atk/s: " + 1 / ((float)spd / 4));
    }

    // Reduce stepCount by 1 each time the ActionLoop ticks (in the EnemyManager).
    public void ReduceStepCount()
    {
        // When stepCount reaches 0, this enemy should Attack.
        stepCount--;
        if (stepCount <= 0)
        {
            stepCount = spd;
            Attack();
        }
    }

    // Called when stepCount reaches 0.
    private void Attack()
    {
        onAttack(this, atk);
    }

    // Modify health. Can't go over maxHp, or lower than 0.
    public void ModifyHealth(int value)
    {
        curHp += value;

        if (curHp <= 0)
            onDeath(this);
        else if (curHp > maxHp)
            curHp = maxHp;
    }

    public Keyword GetKeyword
    {
        get { return keyword; }
    }

    public int GetPos
    {
        get { return myPos; }
    }

    public BaseItemStats GetLoot
    {
        get { return loot; }
    }

    public int GetLevel
    {
        get { return level; }
    }
    public float GetHealthPercent
    {
        get { return (float)curHp / maxHp; }
    }

    public float GetEnergyPercent
    {
        get { return (float)stepCount / spd; }
    }
}
