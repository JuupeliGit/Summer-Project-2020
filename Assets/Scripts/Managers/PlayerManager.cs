using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    [SerializeField] private GameObject player = null;

    private int maxHp = 10;
    private int curHp = 10;
    private int coins = 0;

    [SerializeField] private Image hpBar = null;
    [SerializeField] private TMP_Text hpText = null;
    [SerializeField] private TMP_Text coinText = null;

    [SerializeField] private GameObject hurtParticle = null;
    [SerializeField] private GameObject healParticle = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    // Modify player's current health.
    public void ModifyHealth(int amount)
    {
        // Take damage if amount is less than 0.
        if (amount < 0)
        {
            SoundManager.instance.PlaySound("player_hurt", 0.5f, 1f, 0.5f);
            Instantiate(hurtParticle, player.transform.position + new Vector3(0f, 1f), Quaternion.identity);
        }
        // Heal if amount if more than 0.
        else if (amount > 0)
        {
            SoundManager.instance.PlaySound("player_heal", 0.5f, 1f, 0.2f);
            Instantiate(healParticle, player.transform.position + new Vector3(0f, 1f), Quaternion.identity);
        }

        curHp += amount;

        //Prevents from overhealing
        if (curHp > maxHp)
            curHp = maxHp;

        //Player death
        else if (curHp <= 0)
        {
            curHp = 0;
            GameOver.instance.PlayerDeath();
        }

        //HpBar UI
        hpBar.fillAmount = (float)curHp / maxHp;
        hpText.text = curHp + "/" + maxHp;
    }

    // Add or remove money.
    public void ModifyCoin(int amount)
    {
        coins += amount;
        coinText.text = coins.ToString();
    }

    // Increase max health, also heal the same amount.
    public void IncreaseMaxHp(int amount)
    {
        maxHp += amount;
        ModifyHealth(amount);
    }

    public int CoinAmount
    {
        get { return coins; }
    }
}
