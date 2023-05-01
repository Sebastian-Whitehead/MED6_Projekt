using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    private EnemyHealth[] enemies;
    private Canvas endScreen;
    
    // Start is called before the first frame update
    void Start()
    {
        enemies = GameObject.Find("Enemies").GetComponentsInChildren<EnemyHealth>();
        endScreen = GameObject.Find("EndScreen").GetComponent<Canvas>();
        endScreen.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        endScreen.enabled = IsEnemiesDead(); // Check if last area is complete
    }

    bool IsEnemiesDead()
    {
        foreach (EnemyHealth enemyHealth in enemies) {
            if (enemyHealth.alive) return false; // Continue when current selected enemy is dead
        }
        return true;
    }

}
