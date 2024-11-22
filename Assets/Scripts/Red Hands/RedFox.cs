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

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

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

        if(playerInSightRange && !playerInAttackRange && !alreadyAttacked)
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
            player.GetComponent<PlayerStats>().RemoveMoney(moneyStealAmount);
            moneyStolen += moneyStealAmount;
            alreadyAttacked = true;
            agent.SetDestination(retreatPoint.position);
            Invoke(nameof(DestroyEnemy), 10f);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if(health <= 0)
        {
            player.GetComponent<PlayerStats>().AddMoney(moneyStolen);
            Invoke(nameof(DestroyEnemy), .5f);
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
