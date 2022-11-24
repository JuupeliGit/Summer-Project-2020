using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Entity/Enemy")]
public class BaseEnemyStats : ScriptableObject
{

    [Header("Enemy Stats")]
    [SerializeField] private AnimationCurve hp = default;
    [SerializeField] private AnimationCurve atk = default;
    [SerializeField] private AnimationCurve spd = default;

    [Header("Others")]
    [SerializeField] private Sprite enemySprite = null;
    [SerializeField] private Color color = default;
    [SerializeField] private LootTable lootTable = null;
    [SerializeField] private string[] prefixes = new string[1];


    public Sprite GetSprite
    {
        get { return enemySprite; }
    }

    public Color GetColor
    {
        get { return color; }
    }

    public string[] GetPrefixes
    {
        get { return prefixes; }
    }

    public AnimationCurve GetHp
    {
        get { return hp; }
    }

    public AnimationCurve GetAtk
    {
        get { return atk; }
    }

    public AnimationCurve GetSpd
    {
        get { return spd; }
    }

    public LootTable GetLootTable
    {
        get { return lootTable; }
    }
}
