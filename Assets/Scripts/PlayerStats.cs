using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public HealthBar healthBar;
    [SerializeField] public float maxHealth;
    private float health;

    private float money;
    private float stars;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        money = 0f;
        stars = 0f;
        healthBar.SetSliderMax(maxHealth);
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        healthBar.SetSlider(health);
    }

    public void HealDamage(float amount)
    {
        health += amount;
        healthBar.SetSlider(health);
    }

    void Update()
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

}
