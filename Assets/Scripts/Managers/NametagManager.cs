using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NametagManager : MonoBehaviour
{
    public static NametagManager instance;

    [SerializeField] private Camera cam = null;
    [SerializeField] private GameObject[] itemNametags = null;
    [SerializeField] private GameObject[] enemyNametags = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    // Show a nametag for either an item or an enemy at given position.
    public void ShowNametag(bool item, int index, Vector2 pos, Keyword keyword, int price)
    {
        Vector2 screenPos = cam.WorldToScreenPoint(new Vector2(pos.x, pos.y + (index - 2) * -0.5f));
        screenPos.x = Mathf.Round(screenPos.x) / Screen.width * 480;
        screenPos.y = Mathf.Round(screenPos.y) / Screen.height * 270;

        // Determine if we should show an itemNameTag, or an enemyNameTag.
        GameObject nametag = item ? itemNametags[index] : enemyNametags[index];

        // Get name and color.
        TMP_Text nameText = nametag.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        nameText.text = keyword.GetWord;
        nameText.color = keyword.GetColor;

        // if price is more than 0, then show the number on nametag (only for itemNameTags).
        if (item && price > 0)
            nametag.transform.GetChild(1).GetComponent<TMP_Text>().text = price + " g";
        else if (item)
            nametag.transform.GetChild(1).GetComponent<TMP_Text>().text = "";

        nametag.GetComponent<RectTransform>().anchoredPosition = screenPos;
    }

    // Hide a nametag.
    public void HideNametag(bool item, int index)
    {
        GameObject nametag = item ? itemNametags[index] : enemyNametags[index];

        nametag.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -100);
    }

    // Update health bar, only for enemyNameTags.
    public void UpdateEnemyHealthBar(int index, float amount)
    {
        enemyNametags[index].transform.GetChild(1).GetChild(1).GetComponent<Image>().fillAmount = amount;
    }

    // Update energy bar, only for enemyNameTags.
    public void UpdateEnemyEnergyBar(int index, float amount)
    {
        enemyNametags[index].transform.GetChild(2).GetChild(1).GetComponent<Image>().fillAmount = amount;
    }

    // Hide all nametags.
    public void HideAll()
    {
        for (int i = 0; i < itemNametags.Length; i++)
        {
            HideNametag(true, i);
            HideNametag(false, i);
        }
    }
}
