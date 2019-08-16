using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;                            
    public int currentHealth;                                   
    public Slider healthSlider;

    public GameObject winning;
    CheckForVictor checkForVictor;
    PlayerMovement playerMovement;
    PlayerHit playerHit;
    public bool isDead;

   

    void Awake()
    {
        playerHit = GetComponent<PlayerHit>();
        playerMovement = GetComponent<PlayerMovement>();
        checkForVictor = winning.GetComponent<CheckForVictor>();
        currentHealth = startingHealth;
    }


    void Update()
    {
       
    }

    public void TakeDamage(int amount)
    {

        currentHealth -= amount;

        healthSlider.value = currentHealth;

        if (currentHealth <= 0 && !isDead)
        {
            Death();
        }
    }


    void Death()
    {
        checkForVictor.deathCount++;
        playerHit.inRange = false;
        isDead = true;
        Destroy(this.gameObject);
        
    }
}
