using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private Transform equipSlot = null;
    [SerializeField] private Transform inventorySlots = null;
    private Transform[] slots;

    private Item equippedItem;
    private List<Item> items = new List<Item>();
    PlayerManager player;

    private void Start()
    {
        //Assign inventoryUI children into slots list
        slots = new Transform[inventorySlots.childCount];
        for (int i = 0; i < inventorySlots.childCount; i++)
        {
            slots[i] = inventorySlots.GetChild(i);
        }

        player = FindObjectOfType<PlayerManager>();

        UpdateInventoryUI();
    }

    private void UpdateInventoryUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)
            {
                slots[i].GetComponentInChildren<TMP_Text>().text = items[i].GetKeyword.GetWord;
                slots[i].GetComponentInChildren<TMP_Text>().color = items[i].GetKeyword.GetColor;
                slots[i].GetComponentInChildren<Image>().sprite = items[i].GetSprite;

                slots[i].gameObject.SetActive(true);
            }
            else
                slots[i].gameObject.SetActive(false);
        }

        if(equippedItem != null)
        {
            equipSlot.GetComponentInChildren<TMP_Text>().text = equippedItem.GetKeyword.GetWord;
            equipSlot.GetComponentInChildren<TMP_Text>().color = equippedItem.GetKeyword.GetColor;
            equipSlot.GetComponentInChildren<Image>().sprite = equippedItem.GetSprite;

            equipSlot.gameObject.SetActive(true);
        }
        else
            equipSlot.gameObject.SetActive(false);

        CurrentItemKeywords = ListCurrentItems();
    }

    private Keyword[] ListCurrentItems()
    {
        Keyword[] currentItemObjects = new Keyword[items.Count];
        for (int i = 0; i < items.Count; i++)
        {
            currentItemObjects[i] = items[i].GetKeyword;
        }

        return currentItemObjects;
    }

    public Item GetItemByName(string name)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].GetKeyword.GetWord == name)
            {
                Item temp = items[i];
                return temp;
            }
        }

        return null;
    }

    public bool EquipItem(Item item)
    {
        if(item != null)
        {
            Item temp = equippedItem;
            equippedItem = item;
            RemoveItem(item);
            AddItem(temp);

            return true;
        }

        return false;
    }

    public void UseItem(Item item)
    {
        player.ModifyHealth(item.GetHealAmount);

        RemoveItem(item);
    }

    public bool AddItem(Item item)
    {
        if (item != null && items.Count < 7)
        {
            items.Add(item);
            UpdateInventoryUI();

            return true;
        }

        return false;
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
        UpdateInventoryUI();
    }

    public void ShowItemStats(Item item)
    {
        string statsText = item.GetKeyword.GetWord + ": " + '\n';

        if (item.GetItemType == ItemType.Weapon)
        {
            int dmgDifference = (item.GetDamage - (equippedItem != null ? equippedItem.GetDamage : 0));
            string oper = dmgDifference >= 0 ? "+" : "";
            statsText += oper + dmgDifference + " damage";
        }
        else if (item.GetItemType == ItemType.Consumable)
            statsText += "+" + item.GetHealAmount + " health";

        statsText += "" + '\n' + item.GetSellPrice + " gold";

        EventLog.instance.Print(statsText);
    }

    public Item[] GetItems
    {
        get { return items.ToArray(); }
    }

    public Item GetEquippedItem
    {
        get { return equippedItem; }
    }

    public int GetItemCount
    {
        get { return items.Count; }
    }

    public Keyword[] CurrentItemKeywords 
    { 
        get; 
        private set; 
    }
}
