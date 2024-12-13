using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dingo : Enemy
{
    [SerializeField] private float dingoSpeedReductionRange = 10f;
    [SerializeField] private float playerSpeedReduction = 0.2f; // Reduction per 5 Dingoes
    private bool alreadyAttacked;
    private static List<Dingo> activeDingoes = new List<Dingo>();
    private FPSController fpsController;

    protected override void Awake()
    {
        base.Awake();
        activeDingoes.Add(this);

        // Find the FPSController once
        fpsController = player.GetComponent<FPSController>();
        if (fpsController == null)
        {
            Debug.LogError("FPSController not found on player. Ensure the player GameObject has the FPSController script.");
        }
    }

    protected override void Update()
    {
        base.Update();
        ApplySpeedReduction();
    }

    private void ApplySpeedReduction()
    {
        if (fpsController == null) return;

        // Count nearby Dingoes
        int nearbyDingoes = 0;
        foreach (Dingo dingo in activeDingoes)
        {
            if (dingo != null && !dingo.isDead && Vector3.Distance(player.position, dingo.transform.position) <= dingoSpeedReductionRange)
            {
                nearbyDingoes++;
            }
        }

        // Calculate speed reduction for every 5 Dingoes
        float speedReductionMultiplier = Mathf.Floor(nearbyDingoes / 5f) * playerSpeedReduction;
        fpsController.walkSpeed = fpsController.baseWalkSpeed * (1f - speedReductionMultiplier);
    }

    protected override void AttackPlayer()
    {

        transform.LookAt(player);
        if(!alreadyAttacked && !isDead && player != null)
        {
            player.GetComponent<PlayerStats>().TakeDamage(damage);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), 1.5f);
        }
        
    }

    private void ResetAttack(){
        alreadyAttacked = false;
    }

    public override void DestroyEnemy()
    {
        base.DestroyEnemy();
        activeDingoes.Remove(this);
    }

    private void OnDestroy()
    {
        activeDingoes.Remove(this);
    }
}
