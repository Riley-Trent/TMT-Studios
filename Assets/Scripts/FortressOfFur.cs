using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortressOfFur : MonoBehaviour
{

    public Transform player;

    public float rotationsPerMinute;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 6.0f * rotationsPerMinute * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            player.GetComponent<PlayerStats>().fortressOfFur = true;
            player.GetComponent<PlayerStats>().powerupTimer = 15f;
            Destroy(gameObject);
        }
    }
}
