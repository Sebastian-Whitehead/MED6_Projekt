using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {

    public Turn currentTurn = Turn.Idle;
    public bool collectiveTurn = false;
    private int enemyTurn = 0;
    private bool wait = false;

    public enum Turn {
        Idle,
        Player,
        Enemies
    }

    private Enemy[] enemies;
    private Player player;

    // Start is called before the first frame update
    void Awake() {
        enemies = GameObject.Find("Enemies").GetComponentsInChildren<Enemy>();
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update() {
        TurnSwitch();
    }

    void TurnSwitch() {
        switch (currentTurn) {
            case Turn.Player:
                PlayerTurn();
            break;
            case Turn.Enemies:
                if (collectiveTurn) EnemiesCollectiveTurn();
                else EnemiesSeparateTurn();
            break;

        }
    }

    void PlayerTurn() {
        if (!player.Executing()) {
            player.StartTurn();
            return;
        }
    }

    void EnemiesCollectiveTurn() {
        foreach (Enemy enemy in enemies) {
            enemy.StartTurn();
        }
        currentTurn = Turn.Idle;
    }

    void EnemiesSeparateTurn() {
        Enemy enemy = enemies[enemyTurn];
        Debug.Log(enemyTurn);
        if (!enemy.Executing() && wait) {
            wait = false;
            if (++enemyTurn >= enemies.Length) {
                currentTurn = Turn.Idle;
                enemyTurn = 0;
                return;
            }
            return;
        }
        if (wait) return;
        enemy.StartTurn();
        wait = true;
    }
}