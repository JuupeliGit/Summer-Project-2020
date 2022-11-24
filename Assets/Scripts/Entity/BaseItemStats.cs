using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum ItemType
{
    None,
    Weapon,
    Consumable,
    Passive
}

[CreateAssetMenu(fileName = "New Item", menuName = "Entity/Item")]
public class BaseItemStats : ScriptableObject
{

    [Header("Item Stats")]
    [SerializeField] private AnimationCurve damageCurve = null;
    [SerializeField] private AnimationCurve healingCurve = null;
    [SerializeField] private int price = 0;

    [Header("Others")]
    [SerializeField] private ItemType type = default;
    [SerializeField] private Sprite sprite = null;
    [SerializeField] private Color color = default;
    [SerializeField] private string[] prefixes = new string[5];


    public string GetPrefix(int itemLevel)
    {
        return prefixes[itemLevel];
    }

    public int GetDamage(int itemLevel)
    {
        float n = itemLevel / 5f;
        return Mathf.FloorToInt(damageCurve.Evaluate(n));
    }

    public int GetHealAmount(int itemLevel)
    {
        float n = itemLevel / 5f;
        return Mathf.FloorToInt(healingCurve.Evaluate(n));
    }

    public Color GetColor
    {
        get { return color; }
    }
    public Sprite GetSprite
    {
        get { return sprite; }
    }

    public ItemType GetItemType
    {
        get { return type; }
    }

    public int GetBasePrice(int itemLevel)
    {
        return price + Mathf.FloorToInt(price * 0.25f * itemLevel);
    }
}
