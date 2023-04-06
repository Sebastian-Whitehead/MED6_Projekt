using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resources: MonoBehaviour
{
    public Image healthBar;
    public Image[] healthPoints;
    public float health, maxHealth = 100;

    private float lastHealth;
    private float _lerpSpeed;
    public Shake shaker;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        lastHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        HealthBarFiller();
        if (lastHealth > health)
        {
            shaker.ShakeOnce();
            Debug.Log("Shake");
            lastHealth = health;
        }else if (lastHealth < health)
        {
            lastHealth = health;
        }

        if (health > maxHealth)
            health = maxHealth;
        _lerpSpeed = 3f * Time.deltaTime;
        
        ColorChanger();
    }

    void HealthBarFiller()
    {
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, health/maxHealth, _lerpSpeed);

        for (int i = 0; i < healthPoints.Length; i++)
        {
            healthPoints[i].enabled = !DisplayHealthPoints(health, i);
        }
    }

    void ColorChanger()
    {
        Color healthColor = Color.Lerp(Color.red, Color.green, (health / maxHealth));
        healthBar.color = healthColor;
    }

    bool DisplayHealthPoints(float _health, int pointNumber)
    {
        return ((pointNumber * (maxHealth / healthPoints.Length)) >= _health);
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
}
