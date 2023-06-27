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
    /*
    finder logging mananger, alle fjender og spilleren, så den kan bruge dem
    */


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
    /*Tvinger stop når man trykker på mus2 og det ik er spillerens tur*/


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

    /*
    This method is responsible for managing the player's turn
    It checks if it's currently the player's turn and if the player is active, not moving, and has executed their actions.
    if yes:
     it increments the PlayerTurnCount, resets the player, ends the turn, and logs the event.
    */


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

    /*Not used*/

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
    
    /*
    runs if not player or collective turn.
    it checks if it's the last enemy's turn. If so, it ends the turn.
    Otherwise, it activates the current enemy if it's not active, not moving, and it's the time to wait.
    
    */

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
    //end current turn. resets enemyTurn. 


    public void Wait() {
        wait = true;
    }

    private void OnApplicationQuit()
    {
        _loggingManager.SaveAllLogs(clear:true);
        _loggingManager.NewFilestamp();
    }
    
    
}