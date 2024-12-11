using System.Collections.Specialized;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Thumper : Enemy
{

    public float slamRadius = 5f;
    public float slamHeight = 10f;
    public float slamDuration = 1f;
    public float stunDuration = 2f;

    public float timeBetweenSlams = 5f;
    private bool alreadySlammed, alreadyAttacked, playerInSlamRange;


    protected override void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, setPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, setPlayer);
        playerInSlamRange = Physics.CheckSphere(transform.position, slamRadius, setPlayer);

        if (!playerInSightRange && !alreadySlammed)
        {
            Patroling();
        }

        if (playerInSightRange && !alreadySlammed)
        {
            ChasePlayer();
        }

        if (playerInSlamRange && !alreadySlammed)
        {
            PerformSlam();
        }
        
        if (playerInAttackRange && !isDead)
        {
            AttackPlayer();
        }
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
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    protected override void AttackPlayer(){
        agent.SetDestination(transform.position);

        transform.LookAt(player);
        if(!alreadyAttacked)
        {
            player.GetComponent<PlayerStats>().TakeDamage(damage);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), 1.5f);
        }
    }
    public override void DestroyEnemy()
    {
        ResetPlayerStun();
        base.DestroyEnemy();
    }
}
