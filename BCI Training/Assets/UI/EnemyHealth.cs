using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int health;

    public Healthbar healthbar;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        healthbar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Damage(1);
        }
    }
    
    public void Damage(int dmg)
    {
        if (health > 0)
        {
            health -= dmg;
            healthbar.SetHealth(health);
        }
    }
}
