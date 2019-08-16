using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHit : MonoBehaviour
{
    public float attackDelay = 1f;
    public int attackDamage = 10;
    public float recoveryTime = 2f;
    public GameObject player;
    public PlayerHealth playerHealth;
    public Image ouch;
    public bool inRange;
    float timer;
    float recoveryTimer;
    public bool hitRecovery = false;


   // public GameObject spawn;
   // public GameObject ouchSpawn;
   //public Transform playerTransform;


    void Awake()
    {
        player = GameObject.FindGameObjectWithTag(gameObject.name);
        playerHealth = player.GetComponent<PlayerHealth>();
        
        //playerTransform = player.GetComponent<Transform>();
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player 1" ||
           other.gameObject.tag == "Player 2" ||
           other.gameObject.tag == "Player 3" ||
           other.gameObject.tag == "Player 4" ||
           other.gameObject.tag == "Player 5" ||
           other.gameObject.tag == "Player 6")
        {
            inRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player 1" ||
           other.gameObject.tag == "Player 2" ||
           other.gameObject.tag == "Player 3" ||
           other.gameObject.tag == "Player 4" ||
           other.gameObject.tag == "Player 5" ||
           other.gameObject.tag == "Player 6")
        {
            inRange = false;
            ouch.enabled = false;
        }
    }

    void Start()
    {
        
    }
    void Update()
    {
        timer += Time.deltaTime;

        if(hitRecovery)
        {
            recoveryTimer += Time.deltaTime;
        }

        if(recoveryTimer >= recoveryTime)
        {
            recoveryTimer = 0f;
            hitRecovery = false;
        }

        if(timer >= attackDelay && inRange && !hitRecovery)
        {
            Attack();
        }
    }

    void Attack()
    {
        timer = 0f;

        if(playerHealth.currentHealth > 0)
        {
            playerHealth.TakeDamage(attackDamage);
            ouch.enabled = true;
            //SpawnOuch();

            hitRecovery = true;
        }
    }
/*
    void SpawnOuch()
    {
       spawn =  Instantiate(ouchSpawn, playerTransform.position, Quaternion.Euler(0,0,0));
    }
  */
}
