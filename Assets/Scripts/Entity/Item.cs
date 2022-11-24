using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item                           //Items receive baseitemstats when created and randomizes stats based on floor level
{
    private Keyword keyword = default;
    private Sprite sprite = null;
    private ItemType type = default;

    private int itemLevel = 0;
    private int damage = 0;
    private int healAmount = 0;
    private int price = 0;

    public Vector2 position;

    public Item(BaseItemStats stats, int baseLevel)
    {
        //Randomize item level based on floor level
        itemLevel = Mathf.Clamp(baseLevel + Random.Range(-2, 3), 0, 4);

        //Prefixes are based on itemlevel
        string prefix = stats.GetPrefix(itemLevel);

        string fullName = (prefix == "" ? "" : prefix + " ") + stats.name.ToLower();
        keyword = new Keyword(fullName.ToLower(), stats.GetColor);

        //Pass on stats
        sprite = stats.GetSprite;
        type = stats.GetItemType;

        //Check for type
        if (type == ItemType.Weapon)
            damage = stats.GetDamage(itemLevel + 1);
        else if (type == ItemType.Consumable)
            healAmount = stats.GetHealAmount(itemLevel + 1);

        //Get price
        price = stats.GetBasePrice(itemLevel);
    }

    public Keyword GetKeyword
    {
        get { return keyword; }
    }

    public Sprite GetSprite
    {
        get { return sprite; }
    }

    public ItemType GetItemType
    {
        get { return type; }
    }

    public int GetDamage
    {
        get { return damage; }
    }

    public int GetHealAmount
    {
        get { return healAmount; }
    }

    public int GetSellPrice
    {
        get { return price; }
    }

    public int GetBuyPrice
    {
        get { return price * 2; }
    }
}
