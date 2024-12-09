using System.Collections.Specialized;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Thumper : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask setGround, setPlayer;

    public float health;
    public float damage = 10f;

    public float slamRadius = 5f;
    public float slamHeight = 10f;
    public float slamDuration = 1f;
    public float stunDuration = 2f;

    public float timeBetweenSlams = 5f;
    private bool alreadySlammed;

    [SerializeField] public GameObject HealthyHerb, FortressOfFur;

    // Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // States
    public float sightRange;
    public bool playerInSightRange;
    private int expAmount = 100;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, setPlayer);

        if (!playerInSightRange && !alreadySlammed)
        {
            Patroling();
        }

        if (playerInSightRange && !alreadySlammed)
        {
            ChasePlayer();
        }

        if (playerInSightRange && !alreadySlammed)
        {
            PerformSlam();
        }
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        // Pick a random point within range to walk to
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, setGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void PerformSlam()
    {
        if (!alreadySlammed)
        {
            alreadySlammed = true;
            agent.SetDestination(transform.position); // Stop moving
            StartCoroutine(ExecuteSlam());
        }
    }

    private IEnumerator ExecuteSlam()
    {
        
        Vector3 originalPosition = transform.position;
        Vector3 jumpTarget = new Vector3(originalPosition.x, originalPosition.y + slamHeight, originalPosition.z);

        float halfSlamDuration = slamDuration / 2f;

        
        float elapsedTime = 0f;
        while (elapsedTime < halfSlamDuration)
        {
            transform.position = Vector3.Lerp(originalPosition, jumpTarget, elapsedTime / halfSlamDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        
        elapsedTime = 0f;
        while (elapsedTime < halfSlamDuration)
        {
            transform.position = Vector3.Lerp(jumpTarget, originalPosition, elapsedTime / halfSlamDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        
        transform.position = originalPosition; 
        Collider[] hitObjects = Physics.OverlapSphere(transform.position, slamRadius, setPlayer);

        foreach (Collider hitObject in hitObjects)
        {
            if (hitObject.CompareTag("Player"))
            {
                StunPlayer(hitObject.transform);
            }
        }

       
        Invoke(nameof(ResetSlam), timeBetweenSlams);
    }

    private void StunPlayer(Transform playerTransform)
    {
        var playerController = playerTransform.GetComponent<FPSController>();
        if (playerController != null)
        {
            playerController.walkSpeed = 0f;
            playerController.jumpForce = 0f;
            Invoke(nameof(ResetPlayerStun), stunDuration);
        }
    }

    private void ResetPlayerStun()
    {
        var playerController = player.GetComponent<FPSController>();
        if (playerController != null)
        {
            
            playerController.walkSpeed = playerController.baseWalkSpeed;
            playerController.jumpForce = playerController.baseJumpForce;
        }
    }

    private void ResetSlam()
    {
        alreadySlammed = false;
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;

        if (health <= 0)
        {
            DestroyEnemy();
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
        ResetPlayerStun();
        Destroy(gameObject);
    }
}
