using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class RedFox : Enemy
{

    public Transform retreatPoint;
    public float moneyStealAmount = 10f;
    public float moneyStolen;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;


    void Start()
    {
        moneyStolen = 0f;
    }
    protected override void Update()
    {
        if(!alreadyAttacked){
            base.Update();
        }
    }
    protected override void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        //Steal Money and run away
        if(!alreadyAttacked)
        {
            player.GetComponent<PlayerStats>().TakeDamage(damage);
            if(player.GetComponent<PlayerStats>().money <= 0)
            {
                Invoke(nameof(ResetAttack), 2f);
            }
            else if(player.GetComponent<PlayerStats>().money < 10)
            {
                moneyStolen += player.GetComponent<PlayerStats>().money;
                player.GetComponent<PlayerStats>().RemoveMoney(player.GetComponent<PlayerStats>().money);
                agent.SetDestination(retreatPoint.position);
                Invoke(nameof(DestroyEnemyNoPickup), 10f);
            }
            else
            {
                moneyStolen += moneyStealAmount;
                player.GetComponent<PlayerStats>().RemoveMoney(moneyStealAmount);
                agent.SetDestination(retreatPoint.position);
                Invoke(nameof(DestroyEnemyNoPickup), 10f);
            }
            alreadyAttacked = true;
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public override void DestroyEnemy()
    {
        player.GetComponent<PlayerStats>().AddMoney(moneyStolen);
        base.DestroyEnemy();
    }

    private void DestroyEnemyNoPickup()
    {
        Destroy(gameObject);
    }
}
