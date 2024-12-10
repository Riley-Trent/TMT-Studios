using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class RedFox : MonoBehaviour
{
    

    public NavMeshAgent agent;

    public Transform player;

    public Transform retreatPoint;

    public LayerMask setGround, setPlayer;

    public float health;

    public float damage = 5f;

    public float moneyStealAmount = 10f;

    public float moneyStolen;

    [SerializeField] public GameObject HealthyHerb, FortressOfFur;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange, isAttacked;

    private int expAmount = 100;

    

    void Start()
    {
        moneyStolen = 0f;
    }

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        retreatPoint = GameObject.Find("RetreatPoint").transform;
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, setPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, setPlayer);

        if(!playerInSightRange && !playerInAttackRange && !alreadyAttacked)
        {
            Patroling();
        }

        if((playerInSightRange || isAttacked) && !playerInAttackRange && !alreadyAttacked)
        {
            ChasePlayer();
        }

        if(playerInSightRange && playerInAttackRange && !alreadyAttacked)
        {
            AttackPlayer();
        }

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

        //Steal Money and run away
        if(!alreadyAttacked)
        {
            player.GetComponent<PlayerStats>().TakeDamage(damage);
            if(player.GetComponent<PlayerStats>().money <= 0)
            {
                Invoke(nameof(ResetAttack), 2f);
            }
            else if(player.GetComponent<PlayerStats>().money < 10)
            {
                moneyStolen += player.GetComponent<PlayerStats>().money;
                player.GetComponent<PlayerStats>().RemoveMoney(player.GetComponent<PlayerStats>().money);
                agent.SetDestination(retreatPoint.position);
                Invoke(nameof(DestroyEnemyNoPickup), 10f);
            }
            else
            {
                moneyStolen += moneyStealAmount;
                player.GetComponent<PlayerStats>().RemoveMoney(moneyStealAmount);
                agent.SetDestination(retreatPoint.position);
                Invoke(nameof(DestroyEnemyNoPickup), 10f);
            }
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

        if(health <= 0)
        {
            Invoke(nameof(DestroyEnemy), .1f);
        }
    }

    private void DestroyEnemy()
    {
        ExperienceManager.Instance.AddExperience(expAmount);
        float randValue = Random.value;
        if(randValue <= 0.2f)
        {
            float randValue2 = Random.value;
            if(randValue2 <= 0.5f)
            {
                Instantiate(HealthyHerb, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(FortressOfFur, transform.position, Quaternion.identity);
            }
        }
        player.GetComponent<PlayerStats>().AddMoney(moneyStolen);
        Destroy(gameObject);
    }

    private void DestroyEnemyNoPickup()
    {
        Destroy(gameObject);
    }
}
