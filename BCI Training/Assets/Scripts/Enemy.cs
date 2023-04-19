using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit {

    [Header("Patrole")]
    private int patrolPoint = 0;
    private Vector3[] patrolPoints;
    public bool circlePatrole = false;
    private bool clockwise = true;
    private EnemyHealth enemyHealth;

    // ---------------------------------------------------------------------

    public override void TakeDamage(Vector3 hitPosition, float damageTaken) {
        enemyHealth.Damage(damageTaken);
        audioManager.PlayCategory("TakeDamage");
        action = Action.Attacked;
        targetLocation = hitPosition;
        transform.LookAt(hitPosition, Vector3.up);
    }

    protected override bool Alive() {
        return enemyHealth.alive;
    }

    // ---------------------------------------------------------------------

    protected override bool AttackCheck() {
        if (OutOfRange()) return false;
        return true;
    }

    // Check if distance to given position is matching attack range 
    private bool OutOfRange() {
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 unitPos = gameObject.transform.position; 
         // Calc. distance from unit to target
        float distance = Vector3.Distance(unitPos, playerPos);
        if (distance > attackRange) { // Break at distance more than range
            return true; // Abort method
        }
        return false; // Confirm method
    }
    protected override void ChildAwake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        //patrolPoints = GenerateRandomPath(5);
        patrolPoints = GetManualPath();
        targetLocation = transform.position;
        DecisionTree();
    }

    protected override void ChildUpdate() {
        enemyHealth.Alive(audioManager);
        DrawPatrole();
    }

    public override void DecisionTree() {
        if (steps <= 0 || isMoving) return;
        Debug.Log(name + " decision tree");
        AtLocation();
        Tile nextTile = GetTileAtPosition(targetLocation);
        Debug.Log(name + " nextTile: " + nextTile);
        AStarTargetTile = nextTile;
        Debug.Log("AStarTargetTile: " + AStarTargetTile);
        CalculatePath(nextTile);
    }

    private void AtLocation() {
        Debug.Log(name + " action: " + action);
        // if (Vector3.Distance(transform.position, targetLocation) > 0.001f) return;
        switch (action) {
            case Action.Attacked:
                Investegate(targetLocation);
                break;
            case Action.Investegating:
                Search();
                break;
            case Action.Searching:
                Patrole();
                break;
            case Action.Patroling:
                Patrole();
                break;
            case Action.Attacking:
                Investegate(targetLocation);
                break;
        }
        bool inLocation = Vector3.Distance(targetLocation, transform.position) <= 0.01f;
        bool noTarget = targetLocation == null;
        if (inLocation || noTarget) Search();

    }

    protected override void UnitGone() {   
        if (action == Action.Attacking) {
            Investegate(targetLocation);
        }
    }

    // ---------------------------------------------------------------------


    // https://answers.unity.com/questions/475066/how-to-get-a-random-point-on-navmesh.html
    private Vector3 RandomNavmeshLocation(float radius) {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        UnityEngine.AI.NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    private Vector3[] GenerateRandomPath(int pathLength) {
        Vector3[] path = new Vector3[pathLength];
        for (int i = 0; i < pathLength; i++)
        {
            Vector3 nextPoint = RandomNavmeshLocation(1000f);
            path[i] = nextPoint;
        }
        return path;
    }

    private Vector3[] GetManualPath() {
        Transform PathObject = null;
        foreach (Transform child in transform) {
            if (child.name != "Path") continue;
            PathObject = child.transform;
        }
        if (PathObject == null) Debug.Log("No 'Path' found");

        Transform[] points = PathObject.GetComponentsInChildren<Transform>();
        Vector3[] path = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++) {
            Transform point = points[i];
            Vector3 position = point.transform.position;
            path[i] = position;
        }
        return path;
    }

    private void DrawPatrole() {

        viewColor = Color.green;
        moveColor = Color.blue;
        if (action == Action.Attacking) {
            viewColor = Color.red;
            moveColor = Color.red;
        }

        for (int i = 0; i < patrolPoints.Length; i++) {
            Vector3 startPoint = patrolPoints[i] + new Vector3(0, 1, 0);
            Vector3 endPoint;
            if (i >= patrolPoints.Length - 1) endPoint = patrolPoints[0] + new Vector3(0, 1, 0);
            else endPoint = patrolPoints[i + 1] + new Vector3(0, 1, 0);
            Vector3 difference = endPoint - startPoint;
            Debug.DrawRay(startPoint, difference, new Color(0.2F, 0.3F, 0.4F));
        }
    }

    // ---------------------------------------------------------------------

    private void Patrole() {
        Debug.Log(name + " patrole");
        float dist = Vector3.Distance(transform.position, targetLocation);
        if (dist <= 0.1f) nextPathPoint();
        targetLocation = patrolPoints[patrolPoint];
        action = Action.Patroling;
    }

    protected void nextPathPoint() {
        if (circlePatrole) {
            if (clockwise) {
                if (++patrolPoint >= patrolPoints.Length - 1) clockwise = false;
            } else {
                if (--patrolPoint <= 0) clockwise = true;
            }
        } else {
            if (++patrolPoint >= patrolPoints.Length) patrolPoint = 0;
        }
    }

    private void Investegate(Vector3 position) {
        Debug.Log(name + " inv.");
        action = Action.Investegating;
        targetLocation = position;
    }

    private void Search() {
        Debug.Log(name + " search");
        action = Action.Searching;
        targetLocation = RandomNavmeshLocation(moveRange);
        
        //MoveTo(nextTile);
    }

    private void LookAround() {
        action = Action.Scouting;
        targetLocation = transform.position;
    }
}