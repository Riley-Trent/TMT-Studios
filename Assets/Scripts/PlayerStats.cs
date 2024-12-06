using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public HealthBar healthBar;
    [SerializeField] public float maxHealth;

    [SerializeField] public GameObject hpLostUI, moneyLostUI;
    private float health;

    public float money;

    public TextMeshProUGUI moneyText, moneyLostText, hpGainText;

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
        hpGainText.text = "- " + amount.ToString() + " HP";
        hpLostUI.GetComponent<CanvasRenderer>().SetAlpha(1f);
        hpLostUI.GetComponent<TextMeshProUGUI>().color = Color.red;
        audio.Play();
        hpLostUI.GetComponent<Graphic>().CrossFadeAlpha(0f, 2f, false);
    }

    public void HealDamage(float amount)
    {
        health += amount;
        healthBar.SetSlider(health);
        hpLostUI.GetComponent<CanvasRenderer>().SetAlpha(1f);
        hpLostUI.GetComponent<TextMeshProUGUI>().color = Color.green;
        hpGainText.text = "+ " + amount.ToString() + " HP";
        hpLostUI.GetComponent<Graphic>().CrossFadeAlpha(0f, 2f, false);
    }

    public void RemoveMoney(float amount)
    {
        money -= amount;
        if(money >= 0)
        {
            moneyLostText.text = "- " + amount.ToString() + " ¢";
            moneyLostUI.GetComponent<CanvasRenderer>().SetAlpha(1f);
            moneyLostUI.GetComponent<TextMeshProUGUI>().color = Color.red;
            moneyText.text = money.ToString() + " ¢";
            moneyLostUI.GetComponent<Graphic>().CrossFadeAlpha(0f, 2f, false);
        }
    }

    public void AddMoney(float amount)
    {
        money += amount;
        moneyLostUI.GetComponent<CanvasRenderer>().SetAlpha(1f);
        moneyLostUI.GetComponent<TextMeshProUGUI>().color = Color.green;
        moneyLostText.text = "+ " + amount.ToString() + " ¢";
        moneyText.text = money.ToString() + " ¢";
        moneyLostUI.GetComponent<Graphic>().CrossFadeAlpha(0f, 2f, false);
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
