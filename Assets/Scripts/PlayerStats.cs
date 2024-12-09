using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public HealthBar healthBar;
    public ExperienceBar experienceBar;
    [SerializeField] public float maxHealth;
    [SerializeField] public int currentExperience, maxExperience, currentLevel;

    [SerializeField] public GameObject hpLostUI, moneyLostUI, powerupUI;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] private GameManager gameManager;
    private float health;

    public float money;

    public TextMeshProUGUI moneyText, moneyLostText, hpGainText;
    private FPSController fpsController;

    public bool fortressOfFur, fortressOfFurActive;

    public float powerupTimer;
    private bool isDead;

    AudioSource audio;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        moneyText.text = money.ToString() + " ¢";
        healthBar.SetSliderMax(maxHealth);
        experienceBar.SetSliderMax(maxExperience);
        audio = GetComponent<AudioSource>();
        fpsController = GetComponent<FPSController>();
        powerupTimer = 0f;
    }

    private void OnEnable()
    {
        ExperienceManager.Instance.OnExperienceChange += HandleExperienceChange;
    }

    private void OnDisable()
    {
        ExperienceManager.Instance.OnExperienceChange -= HandleExperienceChange;
    }

    public void TakeDamage(float amount)
    {
        if(fortressOfFur)
        {
            amount /= 2.0f;
        }
        if(amount > 0)
        {
            health -= amount;
            healthBar.SetSlider(health);
            hpGainText.text = "- " + amount.ToString() + " HP";
            hpLostUI.GetComponent<CanvasRenderer>().SetAlpha(1f);
            hpLostUI.GetComponent<TextMeshProUGUI>().color = Color.red;
            audio.Play();
            hpLostUI.GetComponent<Graphic>().CrossFadeAlpha(0f, 2f, false);
        }
    }

    private void HandleExperienceChange(int newExperience)
    {
        currentExperience += newExperience;
        experienceBar.SetSlider(currentExperience);
        if(currentExperience >= maxExperience)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        maxHealth +=10;

        currentLevel++;
        levelText.text = currentLevel.ToString();

        currentExperience = 0;
        maxExperience += 100;
        experienceBar.SetSliderMax(maxExperience);
        experienceBar.SetSlider(currentExperience);
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
        if(amount > 0)
        {
            money += amount;
            moneyLostUI.GetComponent<CanvasRenderer>().SetAlpha(1f);
            moneyLostUI.GetComponent<TextMeshProUGUI>().color = Color.green;
            moneyLostText.text = "+ " + amount.ToString() + " ¢";
            moneyText.text = money.ToString() + " ¢";
            moneyLostUI.GetComponent<Graphic>().CrossFadeAlpha(0f, 2f, false);
        }

    }
    public void ApplyStun(float duration)
    {
        if (fpsController != null)
        {
            fpsController.walkSpeed = 0f;
            fpsController.jumpForce = 0f;

            Invoke(nameof(RemoveStun), duration);
        }
    }

    private void RemoveStun()
    {
        if (fpsController != null)
        {
            fpsController.ResetMovement();
        }
    }

    void Update()
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        if (health <= 0 && !isDead)
        {
            gameObject.SetActive(false);
            isDead = true;
            gameManager.gameOver();
        }

        if (money < 0f)
        {
            money = 0f;
        }
    

        if(fortressOfFur && !fortressOfFurActive)
        {
            fortressOfFurActive = true;
            powerupUI.SetActive(true);
        }

        if(fortressOfFurActive)
        {
            powerupTimer -= Time.deltaTime;
            if(powerupTimer <= 0f)
            {
                RemoveFortressOfFur();
            }
        }
    }

    public void RemoveFortressOfFur()
    {
        fortressOfFur = false;
        fortressOfFurActive = false;
        powerupUI.SetActive(false);
        powerupTimer = 0f;
    }

}
