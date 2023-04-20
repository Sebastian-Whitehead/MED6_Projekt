using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {
    public float maxHealth = 5;
    public float health;
    public Healthbar healthbar;

    public bool alive = true;
    
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        healthbar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        healthbar.SetHealth(health);
        //if (Input.GetKeyDown(KeyCode.Space)) Damage(1);
    }
    
    public void Damage(float dmg)
    {
        if (health > 0)
        {
            health -= dmg;
            healthbar.SetHealth(health);
        }
    }

    public void Alive(AudioManager audioManager) {
        if (health > 0) return;
        alive = false;
        audioManager.PlayCategory("Death");
    }
}
