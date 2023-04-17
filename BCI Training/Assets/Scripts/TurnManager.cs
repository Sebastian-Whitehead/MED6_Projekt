using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{

    public Turn turn = Turn.Player;
    public bool collectiveTurn = false;
    private int enemyTurn = 0;
    private bool wait = false;

    public enum Turn
    {
        Idle,
        Player,
        Enemies
    }

    private Enemy[] enemies;
    private Unit player;

    void Awake()
    {
        enemies = GameObject.Find("Enemies").GetComponentsInChildren<Enemy>();
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void Update() {
        PlayerTurn();
        EnemiesCollectiveTurn();
        EnemiesSeparateTurn();
    }

    private void PlayerTurn() {
        if (turn != Turn.Player) return;

        if (!player.Active() && !player.isMoving && wait) {
            EndTurn();
            return;
        }
        if (!wait) player.Activate();
    }

    private void EnemiesCollectiveTurn() {
        if (turn != Turn.Enemies) return;
        if (!collectiveTurn) return;
        foreach (Enemy enemy in enemies) {
            enemy.Activate();
        }
    }

    private void EnemiesSeparateTurn() {
        if (turn != Turn.Enemies) return;
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

    public void EndTurn()
    {
        // TODO: Wait for seconds

        switch (turn)
        {
            case Turn.Player:
                turn = Turn.Enemies;
                break;
            case Turn.Enemies:
                turn = Turn.Player;
                break;
        }
        wait = false;
    }

    public void Wait()
    {
        wait = true;
    }
}