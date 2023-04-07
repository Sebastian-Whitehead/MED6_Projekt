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
        player = GameObject.Find("Player").GetComponent<Unit>();
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
        }
        if (wait) return;
        player.Activate();
        Wait();
    }

    private void EnemiesCollectiveTurn()
    {
        if (turn != Turn.Enemies) return;
        if (!collectiveTurn) return;
        foreach (Enemy enemy in enemies)
        {
            enemy.Activate();
        }
    }

    private void EnemiesSeparateTurn()
    {
        if (turn != Turn.Enemies) return;
        if (collectiveTurn) return;
        
        Enemy enemy = enemies[enemyTurn];
        if (!enemy.isMoving && wait) {
            wait = false;
            enemy.Deactivate();
            if (++enemyTurn >= enemies.Length) {
                EndTurn();
                enemyTurn = 0;
                return;
            }
            return;
        }
        if (wait) return;
        enemy.Activate();
        Wait();
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