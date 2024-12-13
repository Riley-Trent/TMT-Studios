using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class Nightshade : Enemy
{
    [SerializeField] private float teleportDistance; 
    [SerializeField] private float attackCooldown;  
    [SerializeField] private float teleportCooldown;  
    [SerializeField] private float walkAwayDistance;  
    [SerializeField] private float walkAwayTime;  

    private bool isAttacking = false;
    private bool canTeleport = true;  

    protected override void Update()
    {
        base.Update();  

        if (playerInAttackRange && !isAttacking && canTeleport && !isDead)
        {
            StartCoroutine(TeleportAttack());
        }
    }
    
    protected override void AttackPlayer()
    {
        
    }

    private IEnumerator TeleportAttack()
    {
        
        isAttacking = true;
        canTeleport = false;

        Vector3 teleportPosition = player.position + player.forward * teleportDistance;
        transform.position = teleportPosition;  

        
        Debug.Log("Attacking the player!");
        player.GetComponent<PlayerStats>().TakeDamage(damage);

       
        yield return new WaitForSeconds(attackCooldown);

        
        Vector3 walkAwayDirection = transform.position - player.position;  
        Vector3 walkAwayPosition = transform.position + walkAwayDirection.normalized * walkAwayDistance;  

        agent.SetDestination(walkAwayPosition);  

        yield return new WaitForSeconds(walkAwayTime);  

        
        teleportPosition = player.position + player.forward * teleportDistance;
        transform.position = teleportPosition;

        isAttacking = false;

        yield return new WaitForSeconds(teleportCooldown);
        canTeleport = true;
    }
}
