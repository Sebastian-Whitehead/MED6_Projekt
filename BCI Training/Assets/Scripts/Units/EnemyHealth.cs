using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour {
    public float maxHealth = 5;
    public float health;
    public Healthbar healthbar;
    Animator anim;
    public Image LOS;
    public bool alive = true;
    
    private TextMeshProUGUI alertTxt;
    private TextMeshProUGUI searchTxt;
    
    private LoggingManager _loggingManager;

    // Start is called before the first frame update
    void Start()
    {
        _loggingManager = GameObject.Find("LoggingManager").GetComponent<LoggingManager>();
        alertTxt = GameObject.Find(name + "/Enemy Billboard/AlertTxt").GetComponent<TextMeshProUGUI>();
        searchTxt = GameObject.Find(name + "/Enemy Billboard/SearchTxt").GetComponent<TextMeshProUGUI>();
        if (health == 0)
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
        anim = gameObject.GetComponent<Animator>();
        anim.SetTrigger("Death");
        LOS.enabled = alertTxt.enabled = searchTxt.enabled = false;
        GetComponent<BoxCollider>().enabled = GetComponent<CapsuleCollider>().enabled = false;
        logPlayerData();
    }
    
    private void logPlayerData()
    {
        _loggingManager.Log("Game", new Dictionary<string, object>()
        {
            {"GoblinName", this.name},
            {"Event", "Goblin Death"},
        });

    }
}
