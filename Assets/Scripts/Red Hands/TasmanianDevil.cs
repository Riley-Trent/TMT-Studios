using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class TasmanianDevil : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player, self;

    public LayerMask setGround, setPlayer;

    public float health;

    public float damage = 5f;

    [SerializeField] public GameObject HealthyHerb;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    bool alreadyGrinned;

    float agentSpeed;

    float playerBaseWalkSpeed;
    float playerBaseJumpHeight;

    //States
    public float sightRange, attackRange, grinRange;
    public bool playerInSightRange, playerInAttackRange, playerInGrinRange;

    void Start()
    {
    }

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        self = GameObject.Find("Devil").transform;
        agent = GetComponent<NavMeshAgent>();
        agentSpeed = agent.speed;
        playerBaseWalkSpeed = player.GetComponent<FPSController>().walkSpeed;
        playerBaseJumpHeight = player.GetComponent<FPSController>().jumpForce;
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, setPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, setPlayer);
        playerInGrinRange = Physics.CheckSphere(transform.position, grinRange, setPlayer);

        if(!playerInSightRange && !playerInAttackRange && !alreadyAttacked)
        {
            Patroling();
        }

        if(playerInSightRange && !playerInAttackRange && !alreadyAttacked)
        {
            ChasePlayer();
        }

        if(playerInSightRange && !playerInAttackRange && !alreadyAttacked && playerInGrinRange && !alreadyGrinned)
        {
            StartGrin();
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

        if(!alreadyAttacked)
        {
            player.GetComponent<PlayerStats>().TakeDamage(damage);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), 1.5f);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void StartGrin()
    {
        alreadyGrinned = true;
        transform.LookAt(player);
        agent.speed = 0f;
        Invoke(nameof(Grin), 1.5f);

    }

    public void Grin()
    {
        float absoluteSelfAngleMin, absoluteSelfAngleMax, absolutePlayerAngleMin, absolutePlayerAngleMax;

        if(self.localEulerAngles.y - 90f < 0)
        {
            absoluteSelfAngleMin = self.localEulerAngles.y - 90f + 360f;
        }
        else
        {
            absoluteSelfAngleMin = self.localEulerAngles.y - 90f;
        }

        if(self.localEulerAngles.y + 90f > 360)
        {
            absoluteSelfAngleMax = self.localEulerAngles.y + 90f - 360f;
        }
        else
        {
            absoluteSelfAngleMax = self.localEulerAngles.y + 90f;
        }

        if(player.localEulerAngles.y - 180f < 0)
        {
            absolutePlayerAngleMin = player.localEulerAngles.y - 180f + 360f;
        }
        else
        {
            absolutePlayerAngleMin = player.localEulerAngles.y - 180f;
        }

        if(player.localEulerAngles.y + 180f > 360)
        {
            absolutePlayerAngleMax = player.localEulerAngles.y + 180f - 360f;
        }
        else
        {
            absolutePlayerAngleMax = player.localEulerAngles.y + 180f;
        }


        transform.LookAt(player);
        if((absoluteSelfAngleMin <= absolutePlayerAngleMin) && (absoluteSelfAngleMax >= absolutePlayerAngleMax))
        {
            player.GetComponent<FPSController>().walkSpeed = 0f;
            player.GetComponent<FPSController>().jumpForce = 0f;
            Invoke(nameof(ResetStun), 3f);
        }
        Debug.Log("Player Min: " + absolutePlayerAngleMin + " Player Max: " + absolutePlayerAngleMax + ". Enemy Min: " + absoluteSelfAngleMin + " Enemy Max: " + absoluteSelfAngleMax);
        agent.speed = agentSpeed;
        Invoke(nameof(ResetGrin), 3f);
    }

    public void ResetGrin()
    {
        alreadyGrinned = false;
    }

    public void ResetStun()
    {
        if(player.GetComponent<FPSController>().isScurrying)
        {
            player.GetComponent<FPSController>().walkSpeed = playerBaseWalkSpeed * player.GetComponent<FPSController>().scurryMultiplier;
        }
        else
        {
            player.GetComponent<FPSController>().walkSpeed = playerBaseWalkSpeed;
        }
        player.GetComponent<FPSController>().jumpForce = playerBaseJumpHeight;

    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if(health <= 0)
        {
            Invoke(nameof(DestroyEnemy), .5f);
        }
    }

    private void DestroyEnemy()
    {
        Instantiate(HealthyHerb, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
