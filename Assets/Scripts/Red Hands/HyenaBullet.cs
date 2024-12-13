using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyenaBullet : MonoBehaviour
{

    public GameObject player;
    private Rigidbody rb;
    public float force, attackRange;
    private float timer;

    public LayerMask setPlayer;

    public bool playerInAttackRange;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");

        Vector3 direction = player.transform.position - transform.position;
        rb.velocity = new Vector3(direction.x, 0, direction.z).normalized * force;

        float rot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, setPlayer);

        if(timer > 4)
        {
            Destroy(gameObject);
        }

        if(playerInAttackRange)
        {
            player.gameObject.GetComponent<PlayerStats>().TakeDamage(10f);
            Destroy(gameObject);
        }
    }
}
