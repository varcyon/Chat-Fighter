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
    public PlayerHealth enemyHealth;
    public Image ouch;
    public bool inRange;
    float timer;
    float recoveryTimer;
    public bool hitRecovery = false;
    public GameObject enemy;
    public Color hitFlashColor = new Color(1f, 0f, 0f, 1f);
    public Color playerColor;
    public bool damaged;



    // public GameObject spawn;
    // public GameObject ouchSpawn;
    //public Transform playerTransform;


    void Awake()
    {
        playerColor = GetComponent<Renderer>().material.color;

        //playerTransform = player.GetComponent<Transform>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player 1")
        {
            inRange = true;
            enemy = other.gameObject;
            enemyHealth = enemy.GetComponent<PlayerHealth>();
        }
        if (other.gameObject.tag == "Player 2")
        {
            inRange = true;
            enemy = other.gameObject;
            enemyHealth = enemy.GetComponent<PlayerHealth>();
        }
        if (other.gameObject.tag == "Player 3")
        {
            inRange = true;
            enemy = other.gameObject;
            enemyHealth = enemy.GetComponent<PlayerHealth>();
        }
        if (other.gameObject.tag == "Player 4")
        {
            inRange = true;
            enemy = other.gameObject;
            enemyHealth = enemy.GetComponent<PlayerHealth>();
        }
        if (other.gameObject.tag == "Player 5")
        {
            inRange = true;
            enemy = other.gameObject;
            enemyHealth = enemy.GetComponent<PlayerHealth>();
        }
        if (other.gameObject.tag == "Player 6")
        {
            inRange = true;
            enemy = other.gameObject;
            enemyHealth = enemy.GetComponent<PlayerHealth>();
        }

    }

    void OnTriggerExit(Collider other)
    {
        inRange = false;
        // enemy.GetComponent<PlayerHit>().ouch.enabled = false;
    }

    void Start()
    {

    }
    void Update()
    {
        timer += Time.deltaTime;

        if (hitRecovery)
        {
            recoveryTimer += Time.deltaTime;
        }

        if (recoveryTimer >= recoveryTime)
        {
            recoveryTimer = 0f;
            hitRecovery = false;
            ouch.enabled = false;
            damaged = false;
        }

        if (timer >= attackDelay && inRange && !hitRecovery)
        {
            Attack();
        }

        if (damaged)
        {
            GetComponent<Renderer>().material.color = Color.Lerp(hitFlashColor, playerColor, recoveryTime * Time.deltaTime);
        }
        else 
        {
            GetComponent<Renderer>().material.color = playerColor;

        }
    }

    void Attack()
    {
        timer = 0f;

        if (enemyHealth.currentHealth > 0)
        {
            enemyHealth.TakeDamage(attackDamage);
            enemy.GetComponent<PlayerHit>().ouch.enabled = true;
            //SpawnOuch();
            enemy.GetComponent<PlayerHit>().hitRecovery = true;
            enemy.GetComponent<PlayerHit>().damaged = true;
        }
        /*
            void SpawnOuch()
            {
               spawn =  Instantiate(ouchSpawn, playerTransform.position, Quaternion.Euler(0,0,0));
            }
          */
    }
}