using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private LoggingManager _loggingManager;

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
            //StartCoroutine(waiter());
            return;
        }
        if (!wait) player.Activate();
    }

   /* IEnumerator waiter()
    {
    
    //Wait for 4 seconds
    yield return new WaitForSeconds(1f);
    EndTurn();
    }*/

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