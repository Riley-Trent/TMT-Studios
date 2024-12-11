using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class TasmanianDevil : Enemy
{
    public Camera camera;
    MeshRenderer renderer;
    Plane[] cameraFrustum;
    Collider collider;

    public bool isLookedAt;


    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    bool alreadyGrinned;

    float agentSpeed;

    float playerBaseWalkSpeed;
    float playerBaseJumpHeight;

    //States
    public float grinRange;
    public bool playerInGrinRange;

    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();
        playerBaseWalkSpeed = player.GetComponent<FPSController>().walkSpeed;
        playerBaseJumpHeight = player.GetComponent<FPSController>().jumpForce;
    }

    protected override void Update()
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

   
    protected override void AttackPlayer()
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

    public override void DestroyEnemy()
    {
        ResetStun();
        base.DestroyEnemy();
    }
}
