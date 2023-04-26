using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour {

    private GameObject player;
    public int currentArea = 0;
    private EnemyHealth[] enemies;
    public List<GameObject> spawnPoints;

    void Start() {
        player = GameObject.Find("Player");
        enemies = GameObject.Find("Enemies").GetComponentsInChildren<EnemyHealth>();
    }

    // Update is called once per frame
    void Update() {
        CheckArea();
    }

    void CheckArea() {
        if (enemies.Length - 1 <= currentArea) return;
        EnemyHealth enemyHealth = enemies[currentArea];
        if (enemyHealth.alive) return;
        Debug.Log(enemies.Length);
        Debug.Log(currentArea);
        Spawn(spawnPoints[currentArea]);
        currentArea++;
    }

    void Spawn(GameObject spawnPoint) {
        player.transform.position = spawnPoint.transform.position;
    }
}
