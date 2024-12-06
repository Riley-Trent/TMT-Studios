using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public HealthBar healthBar;
    [SerializeField] public float maxHealth;
    private float health;

    public float money;

    public TextMeshProUGUI moneyText;

    AudioSource audio;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        moneyText.text = money.ToString() + " ¢";
        healthBar.SetSliderMax(maxHealth);
        audio = GetComponent<AudioSource>();
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        healthBar.SetSlider(health);
        audio.Play();
    }

    public void HealDamage(float amount)
    {
        health += amount;
        healthBar.SetSlider(health);
    }

    public void RemoveMoney(float amount)
    {
        money -= amount;
        if(money >= 0)
        {
            moneyText.text = money.ToString() + " ¢";
        }
    }

    public void AddMoney(float amount)
    {
        money += amount;
        moneyText.text = money.ToString() + " ¢";
    }

    void Update()
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        if (money < 0f)
        {
            money = 0f;
        }
    }

}
