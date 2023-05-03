using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    private EnemyHealth[] enemies;
    private Infographic infographic;
    
    // Start is called before the first frame update
    void Awake() {
        enemies = GameObject.Find("Enemies").GetComponentsInChildren<EnemyHealth>();
        infographic = GameObject.Find("Infographic").GetComponent<Infographic>();
    }

    // Update is called once per frame
    void Update() {
        IsEnemiesDead(); // Check if last area is complete
    }

    bool IsEnemiesDead() {
        foreach (EnemyHealth enemyHealth in enemies) {
            if (enemyHealth.alive) return false; // Continue when current selected enemy is dead
        }
        infographic.UpdateAndDisplay("Victory!");
        return true;
    }

}
