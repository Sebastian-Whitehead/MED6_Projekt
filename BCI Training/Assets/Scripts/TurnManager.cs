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

    void Update()
    {
        TurnSwitch();
    }

    private void TurnSwitch()
    {
        switch (turn)
        {
            case Turn.Player:
                PlayerTurn();
                break;
            case Turn.Enemies:
                if (collectiveTurn) EnemiesCollectiveTurn();
                else EnemiesSeparateTurn();
                break;
        }
    }

    private void PlayerTurn()
    {
        if (player.Active() || !wait) return;
        EndTurn();
    }

    private void EnemiesCollectiveTurn()
    {
        foreach (Enemy enemy in enemies)
        {
            enemy.Activate();
        }
    }

    private void EnemiesSeparateTurn()
    {
        Enemy enemy = enemies[enemyTurn];
        if (!enemy.Active() && wait)
        {
            wait = false;
            if (++enemyTurn >= enemies.Length)
            {
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