using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{

    public bool playerTurn = true;
    public bool collectiveTurn = false;
    private int enemyTurn = 0;
    private bool wait = false;
    private Enemy[] enemies;
    private Player player;

    void Awake() {
        enemies = GameObject.Find("Enemies").GetComponentsInChildren<Enemy>();
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void Update() {
        PlayerTurn();
        EnemiesCollectiveTurn();
        EnemiesSeparateTurn();
    }

    private void PlayerTurn() {
        if (!playerTurn) return;

        if (!player.Active() && !player.isMoving && wait && player.execute) {
            player.ResetPlayer();
            EndTurn();
            return;
        }
        if (!wait) player.Activate();
    }

    private void EnemiesCollectiveTurn() {
        if (playerTurn) return;
        if (!collectiveTurn) return;
        foreach (Enemy enemy in enemies) {
            enemy.Activate();
        }
    }

    private void EnemiesSeparateTurn() {
        if (playerTurn) return;
        if (collectiveTurn) return;
        
        Enemy enemy = enemies[enemyTurn];
        if (!enemy.Active() && !enemy.isMoving && wait) {
            wait = false;
            if (++enemyTurn >= enemies.Length) {
                enemyTurn = 0;
                EndTurn();
                return;
            }
            return;
        }
        if (!wait) enemy.Activate();
    }

    // ---------------------------------------------------------------------

    public void EndTurn() {
        // Debug.Log("End turn");
        // TODO: Wait for seconds

        playerTurn = !playerTurn;
        wait = false;
    }

    public void Wait() {
        wait = true;
    }
}