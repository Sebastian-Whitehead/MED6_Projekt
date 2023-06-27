using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    private LoggingManager _loggingManager;


    private EnemyHealth[] enemies;
    private Infographic infographic;
    private bool isComplete = false;
    private bool completeLog = false;

    // Start is called before the first frame update
    void Awake() {
        _loggingManager = GameObject.Find("LoggingManager").GetComponent<LoggingManager>();
        enemies = GameObject.Find("Enemies").GetComponentsInChildren<EnemyHealth>();
        infographic = GameObject.Find("Infographic").GetComponent<Infographic>();
    }

    // Update is called once per frame
    void Update() {
        isComplete = IsEnemiesDead(); // Check if last area is complete
        if (isComplete && completeLog) _loggingManager.Log("Game", "Event", "Test Complete");
    }

    bool IsEnemiesDead() {
        foreach (EnemyHealth enemyHealth in enemies) {
            if (enemyHealth.alive) return false; // Continue when current selected enemy is dead
        }
        infographic.UpdateAndDisplay("Test Complete");
        
        return true;
    }

}
//Tjekker om fjender er døde og så vinder man.
//Opdaterer text I UI'en og viser den.