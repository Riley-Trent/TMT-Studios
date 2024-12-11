using System.Collections;
using DG.Tweening;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Hyena : Enemy
{
    

    public HealthBar healthBar;

    public Transform hyenaBulletPosition;

    [SerializeField] public GameObject hyena1, hyena2, jumpPoint, hyenaBullet;

    public float speed;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    public float inverseRange;
    public bool playerInInverseRange, isRessurecting, preparingBulletBarrage, canAttack;

    public MeshRenderer rend;
    AudioSource audio;

    void Start()
    {
        maxHealth = health;
    }

    protected override void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        audio = GetComponent<AudioSource>();
        healthBar.SetSliderMax(health);
        rend = GetComponent<MeshRenderer>();
        rend.material.SetColor("_Color", new Vector4(0, 1, 0, 1));
        speed = agent.speed;
    }

    protected override void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, setPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, setPlayer);
        playerInInverseRange = Physics.CheckSphere(transform.position, inverseRange, setPlayer);

        if(!playerInSightRange && !playerInAttackRange && !alreadyAttacked && !isDead)
        {
            Patroling();
        }

        if((playerInSightRange || isAttacked) && !playerInAttackRange && !alreadyAttacked && !isDead)
        {
            ChasePlayer();
        }

        if(playerInSightRange && playerInAttackRange && !alreadyAttacked && !isDead && canAttack)
        {
            AttackPlayer();
        }

        if(health <= 0)
        {
            health = 0f;
            rend.material.SetColor("_Color", new Vector4(1, 0, 0, 1));
            isDead = true;
            canAttack = false;
            agent.speed = 0f;
        }
        else
        {
            rend.material.SetColor("_Color", new Vector4(0, 1, 0, 1));
            canAttack = true;
        }

        if(((hyena1.GetComponent<Hyena>().isDead == true) || (hyena2.GetComponent<Hyena>().isDead == true)) && !isDead && !isRessurecting)
        {
            StartCoroutine(HyenaResurrectStart());
        }

        if(!preparingBulletBarrage)
        {
            StartCoroutine(HyenaBulletBarrage());
        }

        if(hyena1.GetComponent<Hyena>().isDead == true && hyena2.GetComponent<Hyena>().isDead == true && isDead)
        {
            Invoke(nameof(DestroyEnemy), 0f);
        }

        healthBar.SetSlider(health);

    }
    protected override void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if(!alreadyAttacked)
        {
            player.GetComponent<PlayerStats>().TakeDamage(damage);
            Invoke(nameof(ResetAttack), 2f);
            alreadyAttacked = true;
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public override void TakeDamage(float damage)
    {
        isAttacked = true;
        health -= damage;
        healthBar.SetSlider(health);

        if(health <= 0)
        {
            isDead = true;
        }
    }

    IEnumerator HyenaResurrectStart()
    {
        //play laugh sound
        isRessurecting = true;
        audio.Play();
        yield return new WaitForSeconds(5);

        if(!isDead)
        {
            if(hyena1.GetComponent<Hyena>().isDead == true)
            {
                hyena1.GetComponent<Hyena>().health += (hyena1.GetComponent<Hyena>().maxHealth / 3.0f);
                hyena1.GetComponent<Hyena>().isDead = false;
                hyena1.GetComponent<Hyena>().healthBar.SetSlider(hyena1.GetComponent<Hyena>().health);
                hyena1.GetComponent<Hyena>().agent.speed = speed;

            }
            if(hyena2.GetComponent<Hyena>().isDead == true)
            {
                hyena2.GetComponent<Hyena>().health += (hyena2.GetComponent<Hyena>().maxHealth / 3.0f);
                hyena2.GetComponent<Hyena>().isDead = false;
                hyena2.GetComponent<Hyena>().healthBar.SetSlider(hyena2.GetComponent<Hyena>().health);
                hyena2.GetComponent<Hyena>().agent.speed = speed;
            }
        }

        isRessurecting = false;
    }

    IEnumerator HyenaBulletBarrage()
    {
        preparingBulletBarrage = true;
        yield return new WaitForSeconds(20);

        if(!isDead)
        {
            agent.speed = 0f;
            transform.DOMove(jumpPoint.transform.position, 1f);
            yield return new WaitForSeconds(1);
            for(int i = 0; i < 12; i++)
            {
                if(!isDead)
                {
                    transform.LookAt(player);
                    Instantiate(hyenaBullet, hyenaBulletPosition.position, Quaternion.identity);
                    yield return new WaitForSeconds(0.5f);
                }
            }
            agent.speed = speed;
        }
        preparingBulletBarrage = false;
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
