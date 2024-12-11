using System.Collections;
using DG.Tweening;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask setGround, setPlayer;
    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float damage;
    [SerializeField] protected float sightRange;
    [SerializeField] protected float attackRange;
    [SerializeField] protected int expAmount;
    [SerializeField] public GameObject HealthyHerb, FortressOfFur;
    public bool playerInSightRange, playerInAttackRange, isAttacked, isDead;

    // Patrolling
    public Vector3 walkPoint;
    public bool walkPointSet;
    [SerializeField] public float walkPointRange;

    protected virtual void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        isDead = false;
    }

    protected virtual void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, setPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, setPlayer);

        if (!playerInSightRange && !playerInAttackRange && !isDead)
        {
            Patroling();
        }

        if (playerInSightRange && !playerInAttackRange && !isDead)
        {
            ChasePlayer();
        }

        if (playerInAttackRange && !isDead)
        {
            AttackPlayer();
        }
    }

    protected virtual void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    protected virtual void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, setGround))
        {
            walkPointSet = true;
        }
    }

    protected virtual void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    protected abstract void AttackPlayer();

    public virtual void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        if (health <= 0 && !isDead)
        {
            DestroyEnemy();
            isDead = true;
        }
    }

    public virtual void DestroyEnemy()
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
        Destroy(gameObject);
    }
}
