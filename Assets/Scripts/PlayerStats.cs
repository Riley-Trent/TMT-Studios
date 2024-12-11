using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public HealthBar healthBar;
    public ExperienceBar experienceBar;
    [SerializeField] public float maxHealth;
    [SerializeField] public int defense, currentExperience, maxExperience, currentLevel, experienceMultiplier;

    [SerializeField] public GameObject hpLostUI, moneyLostUI, powerupUI;
    [SerializeField] TextMeshProUGUI healthAmountText, levelText, expAmountText;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private ExperienceManager experienceManager;
     private GameObject SearchgameManager;
    
    private float health;

    public float money;

    public TextMeshProUGUI moneyText, moneyLostText, hpGainText;
    private FPSController fpsController;

    public bool fortressOfFur, fortressOfFurActive;

    public float powerupTimer;
    private bool isDead;

    AudioSource audio;

    private PlayerStats instance;
    void Awake(){
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
        
    }
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
        experienceManager.OnExperienceChange += HandleExperienceChange;
        SceneManager.sceneLoaded += OnSceneLoaded;
        
    }

    private void OnDisable()
    {
        experienceManager.OnExperienceChange -= HandleExperienceChange;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        gameManager = GameObject.FindFirstObjectByType<GameManager>();

    }
    public void TakeDamage(float amount)
    {
        if(fortressOfFur)
        {
            amount /= 2.0f;
        }
        if(amount > 0)
        {
            health -= (amount -= defense);
            hpGainText.text = "- " + amount.ToString() + " HP";
            UpdateHealthDisplay();
            hpLostUI.GetComponent<CanvasRenderer>().SetAlpha(1f);
            hpLostUI.GetComponent<TextMeshProUGUI>().color = Color.red;
            audio.Play();
            hpLostUI.GetComponent<Graphic>().CrossFadeAlpha(0f, 2f, false);
        }
    }

    private void HandleExperienceChange(int newExperience)
    {
        currentExperience += newExperience * experienceMultiplier;
        UpdateExperienceDisplay();
        
        if(currentExperience >= maxExperience)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        maxHealth += 10;
        health += 10;

        currentLevel++;
        levelText.text = currentLevel.ToString();

        currentExperience = 0;
        maxExperience += 100;
        UpdateExperienceDisplay();
        UpdateHealthDisplay();

        FindObjectOfType<CardSelectionManager>().ShowCardSelection();
        
    }

    public void HealDamage(float amount)
    {
        
        health += amount;
        if(health > maxHealth)
        {
            health = maxHealth;
        }
        healthBar.SetSlider(health);
        hpLostUI.GetComponent<CanvasRenderer>().SetAlpha(1f);
        hpLostUI.GetComponent<TextMeshProUGUI>().color = Color.green;
        UpdateHealthDisplay();
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

    public void UpdateHealthDisplay()
    {
        healthAmountText.text = health.ToString() + "/" + maxHealth.ToString();
        healthBar.SetSliderMax(maxHealth);
        healthBar.SetSlider(health);
    }

    private void UpdateExperienceDisplay()
    {
        expAmountText.text = currentExperience.ToString() + "/" + maxExperience.ToString();
        experienceBar.SetSliderMax(maxExperience);
        experienceBar.SetSlider(currentExperience);
    }
    public float Health
    {
        get => health;
        set
        {
            health = Mathf.Clamp(value, 0, maxHealth);
            UpdateHealthDisplay();
        }
    }

}
