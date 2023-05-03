using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private LoggingManager _loggingManager;
    private int PlayerTurnCount = 0;

    public bool playerTurn = true;
    public bool collectiveTurn = false;
    private int enemyTurn = 0;
    private bool wait = false;
    private Enemy[] enemies;
    private Player player;
    public bool waiting = false;
    
    

    void Awake() {
        _loggingManager = GameObject.Find("LoggingManager").GetComponent<LoggingManager>();
        enemies = GameObject.Find("Enemies").GetComponentsInChildren<Enemy>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void Update() {
        PlayerTurn();
        EnemiesCollectiveTurn();
        EnemiesSeparateTurn();
    }

    private void PlayerTurn() {
        if (!playerTurn) return;
        if (!player.Active() && !player.isMoving && wait && player.execute && waiting == true) {
            player.ResetPlayer();
            EndTurn();
            PlayerTurnCount += 1;
            _loggingManager.Log("Log", "Nr of Turns", PlayerTurnCount);
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
        
        if (enemyTurn >= enemies.Length) return;
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
        if (!wait) {
            if (!enemy.GetComponent<EnemyHealth>().alive) {
                enemyTurn++;
            } else enemy.Activate();
        }
    }

    // ---------------------------------------------------------------------

    public void EndTurn() {
        // Debug.Log("End turn");

        playerTurn = !playerTurn;
        wait = false;
        waiting = false;
    }

    public void Wait() {
        wait = true;
    }

    private void OnApplicationQuit()
    {
        _loggingManager.SaveAllLogs(clear:true);
        _loggingManager.NewFilestamp();
    }
}