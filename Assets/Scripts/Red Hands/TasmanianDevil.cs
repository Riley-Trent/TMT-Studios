using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class TasmanianDevil : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask setGround, setPlayer;

    public float health;

    public float damage = 5f;

    [SerializeField] public GameObject HealthyHerb, FortressOfFur;

    public Camera camera;
    MeshRenderer renderer;
    Plane[] cameraFrustum;
    Collider collider;

    public bool isLookedAt;

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
    public bool playerInSightRange, playerInAttackRange, playerInGrinRange, isAttacked;
    private int expAmount = 100;

    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();
    }

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
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

        if((playerInSightRange || isAttacked) && !playerInAttackRange && !alreadyAttacked)
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

        //for telling if player is looking at enemy.
        var bounds = collider.bounds;
        cameraFrustum = GeometryUtility.CalculateFrustumPlanes(camera);
        if(GeometryUtility.TestPlanesAABB(cameraFrustum, bounds))
        {
            isLookedAt = true;
        }
        else
        {
            isLookedAt = false;
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
        transform.LookAt(player);
        if(isLookedAt)
        {
            player.GetComponent<FPSController>().walkSpeed = 0f;
            player.GetComponent<FPSController>().jumpForce = 0f;
            Invoke(nameof(ResetStun), 3f);
        }
        agent.speed = agentSpeed;
        Invoke(nameof(ResetGrin), 10f);
    }

    public void ResetGrin()
    {
        alreadyGrinned = false;
    }

    public void ResetStun()
    {
        player.GetComponent<FPSController>().walkSpeed = playerBaseWalkSpeed;
        player.GetComponent<FPSController>().jumpForce = playerBaseJumpHeight;
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
        ResetStun();
        Destroy(gameObject);
    }
}
