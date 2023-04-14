using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : TacticsMove
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
        PlayerTurnF();
        EnemiesCollectiveTurn();
        EnemiesSeparateTurn();
    }

<<<<<<< Updated upstream
    private void PlayerTurn() {
        if (turn != Turn.Player) return;
=======
    private void PlayerTurnF() {
        if (!playerTurn) return;
>>>>>>> Stashed changes

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
        
        if (!isMoving){
            FindPlayer();
            CalculatePath();
            BFS();
            AStarTargetTile.targetTile = true; 
        }
        else{
            Move();
        }
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


    GameObject target;
    void CalculatePath(){ //Find where it is going to move to
        Tile targetTile = GetTargetTile(target);
        //FindPath(targetTile);

    }

    void FindPlayer(){ //finds the nearest player object, delete this. just assign target to the player tag.
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player"); //Array change to single

        GameObject nearest = null;
        float distance = Mathf.Infinity;
        
        foreach (GameObject obj in targets){ //check which obj with player tag is closest (ret)
            float d = Vector3.Distance(transform.position, obj.transform.position);

            if (d < distance){
                distance  = d; 
                nearest = obj;
            }
        }
        
        target = nearest;
    }

}