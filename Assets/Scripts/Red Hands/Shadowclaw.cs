using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class Shadowclaw : Enemy
{
    [SerializeField] private float clawDamage = 5f;  // Damage dealt by shadow claws per second
    [SerializeField] private float clawDuration = 5f;  // Duration the claws will remain active
    [SerializeField] private float clawRange = 5f;  // Range of the claws' effect
    [SerializeField] private float clawCooldown = 10f;  // Cooldown between uses
    [SerializeField] private float attackSpeedBuff = 1.5f;  // Buff to attack speed during ability
    [SerializeField] private float buffDuration = 5f;  // Duration of the attack speed buff

    private bool clawsActive = false;
    private bool canUseClaws = true;
    private bool alreadyAttacked = false;
    private float originalAttackSpeed;

    protected override void Update()
    {
        base.Update();  // Call the parent Update to handle patrolling, chasing, and default attack

        // If the player is within attack range, trigger the Shadow Claws ability
        if (playerInAttackRange && !clawsActive && canUseClaws && !isDead)
        {
            StartCoroutine(ActivateShadowClaws());
        }
    }

    // Custom AttackPlayer method that handles the teleportation and attack behavior
    protected override void AttackPlayer()
    {
        if (!clawsActive)
        {
            // Normal attack logic goes here if claws aren't active
            agent.SetDestination(transform.position);
            transform.LookAt(player);
            if (!alreadyAttacked)
            {
                player.GetComponent<PlayerStats>().TakeDamage(damage);
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), 1.5f);
            }
        }
    }

    private IEnumerator ActivateShadowClaws()
    {
        clawsActive = true;
        canUseClaws = false;

        // Store the original attack speed before buffing
        originalAttackSpeed = damage;
        damage *= attackSpeedBuff;  // Increase damage for the duration of the claws

        // Start the shadow claw damage effect (damage over time)
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, clawRange, setPlayer);  // Find all players within claw range
        foreach (var hit in hitEnemies)
        {
            if (hit.CompareTag("Player"))
            {
                StartCoroutine(ApplyClawDamage(hit.transform));
            }
        }

        // Wait for the duration of the claw attack before ending the effect
        yield return new WaitForSeconds(clawDuration);

        // End the claws' effect and reset the attack speed
        damage = originalAttackSpeed;
        clawsActive = false;

        // Wait for the cooldown before the claws can be used again
        yield return new WaitForSeconds(clawCooldown);
        canUseClaws = true;
    }

    // Coroutine that applies damage over time to an enemy
    private IEnumerator ApplyClawDamage(Transform enemy)
    {
        float elapsedTime = 0f;
        while (elapsedTime < clawDuration)
        {
            // Apply continuous damage over time
            enemy.GetComponent<PlayerStats>().TakeDamage(clawDamage * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    // You can override or add specific damage handling logic here if needed
    public override void DestroyEnemy()
    {
        base.DestroyEnemy();
    }

    // Reset the attack flag to prevent spamming attacks
    private void ResetAttack()
    {
        alreadyAttacked = false;
        Debug.Log("Nightshade attack reset");
    }
}
