using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class Nightshade : Enemy
{
    [SerializeField] private float teleportDistance;  // Distance in front of the player where the enemy will teleport
    [SerializeField] private float attackCooldown;  // Cooldown between attacks
    [SerializeField] private float teleportCooldown;  // Cooldown between teleports
    [SerializeField] private float walkAwayDistance;  // Distance to walk away after attacking
    [SerializeField] private float walkAwayTime;  // Time the enemy walks away before returning

    private bool isAttacking = false;
    private bool canTeleport = true;  // Ensures teleport happens only after cooldown

    protected override void Update()
    {
        base.Update();  // Call the parent Update to handle patrolling, chasing, and default attack

        // If the player is within attack range, trigger teleportation attack
        if (playerInAttackRange && !isAttacking && canTeleport && !isDead)
        {
            StartCoroutine(TeleportAttack());
        }
    }

    // Custom AttackPlayer method that handles the teleportation and attack behavior
    protected override void AttackPlayer()
    {
        // This method will now be handled by the coroutine TeleportAttack
    }

    private IEnumerator TeleportAttack()
    {
        // Prevent multiple attacks and teleporting before cooldowns
        isAttacking = true;
        canTeleport = false;

        // Disable the NavMeshAgent to stop it from moving (disable pathfinding)
        // agent.enabled = false;

        // Teleport in front of the player
        Vector3 teleportPosition = player.position + player.forward * teleportDistance;
        transform.position = teleportPosition;  // Teleport the enemy to this new position

        // Inflict damage on the player immediately after teleporting
        Debug.Log("Attacking the player!");
        player.GetComponent<PlayerStats>().TakeDamage(damage);

        // Wait for attack cooldown before moving away
        yield return new WaitForSeconds(attackCooldown);

        // Walk away from the player for a brief period
        Vector3 walkAwayDirection = transform.position - player.position;  // Get the direction away from the player
        Vector3 walkAwayPosition = transform.position + walkAwayDirection.normalized * walkAwayDistance;  // Move a fixed distance away

        // Temporarily disable the NavMeshAgent to avoid it trying to chase or move
        // agent.enabled = true;  // Re-enable the agent's pathfinding for walking away
        agent.SetDestination(walkAwayPosition);  // Use NavMeshAgent to walk away

        yield return new WaitForSeconds(walkAwayTime);  // Wait for the walk-away time

        // After walking away, teleport back to the player
        teleportPosition = player.position + player.forward * teleportDistance;
        transform.position = teleportPosition;

        // Allow the enemy to attack again after the cooldown
        isAttacking = false;

        // Allow the enemy to teleport again after the cooldown
        yield return new WaitForSeconds(teleportCooldown);
        canTeleport = true;
    }
}
