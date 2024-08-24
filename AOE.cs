using UnityEngine;

public class AOE : MonoBehaviour
{  
    public int maxHealth = 3;
    public int currentHealth;
    public health_bar_beta healthbar; 

    void Start(){
        currentHealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TakeDamage(1);
        // Debug.Log("Trigger collision detected between " + gameObject.name + " and " + other.gameObject.name);
    }

    void TakeDamage(int damage){
        currentHealth -= damage;
        healthbar.SetHealth(currentHealth);
    }
}
