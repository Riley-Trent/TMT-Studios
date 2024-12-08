using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Hyena : MonoBehaviour
{
    

    public NavMeshAgent agent;

    public HealthBar healthBar;

    public Transform player;

    public LayerMask setGround, setPlayer;

    [SerializeField] public GameObject hyena1, hyena2, jumpPoint;

    public float health, speed;

    private float maxHealth;

    public float damage = 5f;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    public float sightRange, attackRange, inverseRange;
    public bool playerInSightRange, playerInAttackRange, isAttacked, playerInInverseRange, isDead, isRessurecting, preparingBulletBarrage;

    public MeshRenderer rend;
    AudioSource audio;

    void Start()
    {
        maxHealth = health;
    }

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        audio = GetComponent<AudioSource>();
        healthBar.SetSliderMax(health);
        rend = GetComponent<MeshRenderer>();
        rend.material.SetColor("_Color", new Vector4(0, 1, 0, 1));
        speed = agent.speed;
    }

    private void Update()
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

        if(playerInSightRange && playerInAttackRange && !alreadyAttacked && !isDead)
        {
            AttackPlayer();
        }

        if(health <= 0)
        {
            health = 0f;
            rend.material.SetColor("_Color", new Vector4(1, 0, 0, 1));
            isDead = true;
        }
        else
        {
            rend.material.SetColor("_Color", new Vector4(0, 1, 0, 1));
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

    private void Patroling()
    {
        if(!walkPointSet) SearchWalkPoint();

        if(walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if(distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        //For picking where to walk around
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 2f, setGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
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

    public void TakeDamage(float damage)
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

            }
            if(hyena2.GetComponent<Hyena>().isDead == true)
            {
                hyena2.GetComponent<Hyena>().health += (hyena2.GetComponent<Hyena>().maxHealth / 3.0f);
                hyena2.GetComponent<Hyena>().isDead = false;
                hyena2.GetComponent<Hyena>().healthBar.SetSlider(hyena2.GetComponent<Hyena>().health);
            }
        }

        isRessurecting = false;
    }

    IEnumerator HyenaBulletBarrage()
    {
        yield return new WaitForSeconds(20);

        if(!isDead)
        {
            agent.speed = 0f;
            
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
