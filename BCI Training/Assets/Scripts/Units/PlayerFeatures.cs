using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SharedDatastructures;

public class PlayerFeatures : MonoBehaviour
{
    public Image[] manaPoints;
    public Image[] healthPoints;
    public float health, maxHealth = 100;
    public float mana, maxMana = 10;
    public float manaCost = 2;
    public float fixedRegenPoints;

    private float lastHealth;
    private Shake shake;
    public bool alive = true;
    
    public Gamemode gamemode;
    public Image[] manaUI;

    void Start()
    {
        health = maxHealth;
        lastHealth = health;
        // print(gamemode);
        
        if (gamemode == Gamemode.Interval)
        {
            maxMana = mana = 99999;
            HideManaUI();
        }
    }

    void Awake() {
        shake = GameObject.Find("Main Camera").GetComponent<Shake>();
    }

    // Update is called once per frame
    void Update()
    {
        HealthBarFiller();
        ManaBarFiller();
        
        if (lastHealth > health)
        {
            shake.ShakeOnce();
            Debug.Log("Shake");
            lastHealth = health;
        }else if (lastHealth < health)
        {
            lastHealth = health;
        }

        if (health > maxHealth)
            health = maxHealth;
    }

    void HealthBarFiller()
    {
        for (int i = 0; i < healthPoints.Length; i++)
        {
            healthPoints[i].enabled = !DisplayHealthPoints(health, i);
        }
    }

    bool DisplayHealthPoints(float _health, int pointNumber)
    {
        return (pointNumber * (maxHealth / healthPoints.Length) >= _health);
    }


    void ManaBarFiller()
    {
        if (gamemode == Gamemode.Interval) return;
        for (int i = 0; i < manaPoints.Length; i++)
        {
            manaPoints[i].enabled = !DisplayManaPoints(mana, i);
        }
    }

    bool DisplayManaPoints(float _mana, int pointNumber)
    {
        return (pointNumber * (maxMana / manaPoints.Length) >= _mana);
    }

    public void Damage(float dmgPoints)
    {
        if (health > 0)
            health -= dmgPoints;
        HealthBarFiller();
    }

    public void Heal(float healPoints)
    {
        if (health < maxHealth)
            health += healPoints;
    }

    public void RegenMana(float RegenPoints)
    {
        if (mana <= 0) return;
        mana += RegenPoints;
    }

    public void RegenMana()
    {
        if (mana <= 0) return;
        mana += fixedRegenPoints;
    }

    public void Expend()
    {
        // Debug.Log("Decrease mana " + manaCost);
        if (mana <= maxMana) mana -= manaCost;
        ManaBarFiller();
    }

    public bool ManaCheck() {
        bool manaCheck = manaCost <= mana;
        // Debug.Log("ManaCheck: " + manaCheck);
        return manaCheck;
    }

    public void Alive(AudioManager audioManager) {
        if (health > 0) return;
        alive = false;
        audioManager.PlayCategory("Death");
    }

    public void HideManaUI()
    {
        foreach (var currentElement in manaUI)
        {
            currentElement.enabled = false;
        }
    }
    
}
