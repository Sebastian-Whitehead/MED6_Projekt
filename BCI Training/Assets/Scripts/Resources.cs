using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resources: MonoBehaviour
{
    public Image[] manaPoints;
    public Image[] healthPoints;
    public float health, maxHealth = 100;
    public float mana, maxMana = 10;
    public float manaCost = 2;

    private float lastHealth;
    private Shake shake;

    void Start()
    {
        health = maxHealth;
        lastHealth = health;
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
    }

    public void Heal(float healPoints)
    {
        if (health < maxHealth)
            health += healPoints;
    }

    public void RegenMana(float RegenPoints)
    {
        if (mana > 0)
            mana += RegenPoints;
    }

    public void Expend(float expendPoints)
    {
        if (mana < maxMana)
            mana -= expendPoints;
    }

    public bool ManaCheck() {
        return manaCost < mana;
    }
    
}
