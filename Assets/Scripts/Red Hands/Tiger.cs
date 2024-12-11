using UnityEngine;

public class Tiger : Enemy
{

    public HealthBar healthBar;
    public Transform boss;

    public LayerMask setWall;


    float basePlayerSpeed, basePlayerJumpHeight;
    public float speed;

    public Vector3 playerPosition;
    //Attacking
    public float timeBetweenAttacks;

    public float wildChargeSetupInterval;
    public bool alreadyAttacked, shadowStrikeDone, wildChargeDone, shadowStrikeStunPossible;


    public bool isEncountered;
    //States
    public float hitWallRange;
    public bool bossHitWall;

    public bool PhaseOneActive, PhaseTwoActive, PhaseThreeActive;

    protected override void Awake()
    {
        player = GameObject.Find("Player").transform;
        boss = GameObject.Find("Da Boss").transform;
        speed = 3f;
        health = maxHealth;
        healthBar.SetSliderMax(maxHealth);

        basePlayerSpeed = player.GetComponent<FPSController>().walkSpeed;
        basePlayerJumpHeight = player.GetComponent<FPSController>().jumpForce;
        isDead = false;
    }

    protected override void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, setPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, setPlayer);
        bossHitWall = Physics.CheckSphere(transform.position, hitWallRange, setWall);

        if((playerInSightRange || isEncountered) && health > (maxHealth * 0.67f))
        {
            isEncountered = true;
            PhaseOneActive = true;
            PhaseOne();
        }
        if(health <= (maxHealth * 0.67f) && health > (maxHealth * 0.33f))
        {
            PhaseOneActive = false;
            PhaseTwoActive = true;
            PhaseTwo();
        }
        if(health <= (maxHealth * 0.33f))
        {
            PhaseTwoActive = false;
            PhaseThreeActive = true;
            PhaseThree();
        }
        if(health <= 0)
        {
            Died();
            ExperienceManager.Instance.AddExperience(expAmount);
            Destroy(gameObject);
        }
        if(playerInAttackRange)
        {
            DoDamage();
        }
    }

    private void PhaseOne()
    {
        if(PhaseOneActive)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, player.position, step);
        }
    }

    private void PhaseTwo()
    {
        if(PhaseTwoActive)
        {
            float step = (speed + 1f) * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, player.position, step);
        }
        if(PhaseTwoActive && !shadowStrikeDone)
        {
            transform.position = new Vector3(player.position.x, player.position.y + 12, player.position.z);
            shadowStrikeDone = true;
            shadowStrikeStunPossible = true;
            Invoke(nameof(ShadowStrikeStun), 2f);
            Invoke(nameof(ResetShadowStrike), 10f);
        }
    }

    private void PhaseThree()
    {
        if(PhaseThreeActive && wildChargeDone)
        {
            float step = (speed + 2f) * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, player.position, step);
        }
        if(PhaseThreeActive && !wildChargeDone)
        {
            //buggy rn
            float step = (speed + 15f) * Time.deltaTime;
            if(!bossHitWall)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.position, step);
            }
            else
            {
                wildChargeDone = true;
                Invoke(nameof(ResetWildCharge), 10f);
            }
        }
    }

    private void DoDamage()
    {
        if(!alreadyAttacked && shadowStrikeStunPossible)
        {
            player.GetComponent<FPSController>().walkSpeed = 0f;
            player.GetComponent<FPSController>().jumpForce = 0f;
            Invoke(nameof(ResetMovement), 1f);
        }
        if(!alreadyAttacked)
        {
            player.GetComponent<PlayerStats>().TakeDamage(damage);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), 1f);
        }
    }

    public override void TakeDamage(float damage)
    {
        isEncountered = true;
        health -= damage;
        healthBar.SetSlider(health);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void ResetShadowStrike()
    {
        shadowStrikeDone = false;
    }

    private void ResetWildCharge()
    {
        wildChargeDone = false;
    }

    private void ShadowStrikeStun()
    {
        shadowStrikeStunPossible = false;
    }

    private void ResetMovement()
    {
        player.GetComponent<FPSController>().walkSpeed = basePlayerSpeed;
        player.GetComponent<FPSController>().jumpForce = basePlayerJumpHeight;
    }

    public void Died()
    {
        isDead = true;
    }
    protected override void AttackPlayer()
    {
        //just added otherwise it would get mad
    }
}
