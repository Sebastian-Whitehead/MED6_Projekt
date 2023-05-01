using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SharedDatastructures;

public class PlayerFeatures : MonoBehaviour
{
    private Image[] manaPoints;
    private Image[] healthPoints;
    public float health, maxHealth = 100;
    public float mana, maxMana = 10;
    public float manaCost = 2;
    public float fixedRegenPoints;
    private float lastHealth;
    private Shake shake;
    public bool alive = true;
    
    [NonSerialized] public Gamemode gamemode;
    private Image[] manaUI;
    
    private LoggingManager _loggingManager;
    private int dmgTaken = 0;

    void Start()
    {
        _loggingManager = GameObject.Find("LoggingManager").GetComponent<LoggingManager>();
        health = maxHealth;
        lastHealth = health;
        // print(gamemode);
        
        if (gamemode == Gamemode.Interval)
        {
            maxMana = mana = 99999;
            HideManaUI();
        }
        
        manaPoints = GameObject.Find("Manapoints").GetComponentsInChildren<Image>();
        healthPoints = GameObject.Find("FillerHearts").GetComponentsInChildren<Image>();
        manaUI = GameObject.Find("Manabar").GetComponentsInChildren<Image>();
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
            shake.ShakeOnce(1f);
            lastHealth = health;
            dmgTaken++;
            _loggingManager.Log("Log", "Take Damage", dmgTaken);

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

    public void RegenMana(float regenPoints)
    {
        if (mana >= maxMana) return;
        if (mana + regenPoints <= maxMana) mana += regenPoints;
        else mana = maxMana;
    }

    public void RegenMana(){
        if (mana >= maxMana) return;
        mana += fixedRegenPoints;
    }

    public void Expend()
    {
        // Debug.Log("Decrease mana " + manaCost);
        if (mana - manaCost >= 0) mana -= manaCost;
        ManaBarFiller();
    }

    public bool ManaCheck() {
        bool manaCheck = manaCost <= mana;
        // Debug.Log("ManaCheck: " + manaCheck);
        if (!manaCheck) StartCoroutine(Blink());
        return manaCheck;
    }

    private IEnumerator Blink() {   
        Color originalColor = Color.white;
        Color blinkColor = Color.red;
        float blinkDuration = .2f;
        float alpha = 0f;
        Image manabar = GameObject.Find("Manabar").GetComponent<Image>();

        while (alpha < 1f) {
            alpha += Time.deltaTime / blinkDuration;
            manabar.color = Color.Lerp(originalColor, blinkColor, alpha);
            yield return null;
        }

        while (alpha > 0f) {
            alpha -= Time.deltaTime / blinkDuration;
            manabar.color = Color.Lerp(originalColor, blinkColor, alpha);
            yield return null;
        }

        manabar.color = originalColor;
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
