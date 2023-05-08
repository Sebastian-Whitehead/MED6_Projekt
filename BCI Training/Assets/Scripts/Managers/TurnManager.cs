using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private LoggingManager _loggingManager;
    private int PlayerTurnCount = 0;
    private string eventStr;

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
        ForceEndInput();
    }

    private void ForceEndInput()
    {
        if (Input.GetKey(KeyCode.Mouse2) && !playerTurn)
            EndTurn();
    }

    private void PlayerTurn() {
        if (!playerTurn) return;
        if (!player.Active() && !player.isMoving && wait && player.execute && waiting == true) {
            player.ResetPlayer();
            EndTurn();
            eventStr = "End Player Turn";
            logData();
            return;
        }

        if (!wait)
        {
            PlayerTurnCount += 1;
            eventStr = "Start Player Turn";
            logData();
            player.Activate();
        }
    }

    private void EnemiesCollectiveTurn() {
        if (playerTurn) return;
        if (!collectiveTurn) return;
        foreach (Enemy enemy in enemies)
        {
            eventStr = "Start Goblins Turn";
            logData();
            enemy.Activate();
        }
    }

    private void EnemiesSeparateTurn() {
        if (playerTurn) return;
        if (collectiveTurn) return;

        if (enemyTurn >= enemies.Length)
        {
            _loggingManager.Log("Game", "Event", "End Goblin Turn");
            EndTurn();
            return;
        }
        
        //print(enemyTurn + " / " + enemies.Length);
        Enemy enemy = enemies[enemyTurn];
        if (!enemy.Active() && !enemy.isMoving && wait) {
            wait = false;
            if (++enemyTurn >= enemies.Length)
            {
                EndTurn();
                _loggingManager.Log("Game", "Event", "End Goblin Turn");
                return;
            }
            return;
        }
        if (!wait) {
            if (!enemy.GetComponent<EnemyHealth>().alive) {
                enemyTurn++;
            }
            else
            {
                eventStr = "Start Goblin Turn";
                logData();
                enemy.Activate();
            }
            
        }
    }
    
    private void logData()
    {
        _loggingManager.Log("Game", new Dictionary<string, object>()
        {
            {"Nr of Turns", PlayerTurnCount},
            {"PlayerTurn", playerTurn},
            {"Event", eventStr},
            // {"State", Enum.GetName(typeof(State), state)},
        });

    }

    // ---------------------------------------------------------------------

    public void EndTurn() {
        // Debug.Log("End turn");
        enemyTurn = 0;
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