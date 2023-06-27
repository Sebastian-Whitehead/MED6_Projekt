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
    Animator anim;
    private int skipTurnCounter = 0;

    // ---------------------------------------------------------------------

    public override void TakeDamage(Vector3 hitPosition, float damageTaken) {
        StartCoroutine(waiter());
        anim = gameObject.GetComponent<Animator>();
        enemyHealth.Damage(damageTaken);
        if (enemyHealth.health > 1)
        //audioManager.PlayCategory("TakeDamage");
        action = Action.Damaged;
        targetLocation = hitPosition;
        transform.LookAt(hitPosition, Vector3.up);
        //anim.SetTrigger("Hit"); 
    }

    /*
    The TakeDamage method is overridden from the base class. It is called when the enemy receives damage. 
    Inside this method, it starts a coroutine waiter() and performs actions such as reducing health, setting the enemy in a damaged state, updating the target location, and playing animations.
    */

    public override bool Alive() {
        return enemyHealth.alive;
    }
    //Checks if enemy is alive based on health.

    // ---------------------------------------------------------------------
    IEnumerator waiter()
    {
    
    //Wait for 4 seconds
    yield return new WaitForSeconds(2f);    

    }
    //coroutine waits for 2 seconds before continuing execution. It is used in the TakeDamage method to introduce a delay.

    protected override bool AttackCheck() {
        if (OutOfRange()) return false;
        if (!active) return false;
        return true;
    }
    /*
    checks if the enemy is in range to attack the player. 
    It returns true if the enemy is within the attack range and active.
    */


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
    /*
    calculates the distance between the enemy and the player. If the distance exceeds the attack range, it returns true,
    indicating that the player is out of range for an attack.
    */


    protected override void ChildAwake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        //patrolPoints = GenerateRandomPath(5);
        patrolPoints = GetManualPath();
        targetLocation = transform.position;
        DecisionTree();
    }
    /*
    Is called during initialization. 
    It sets up the enemy's health, patrol points, target location, and invokes the DecisionTree method
    to start the enemy's decision-making process.
    */

    protected override void ChildUpdate() {
        enemyHealth.Alive(audioManager);
        Eyes(); // Raycast see targets depended to 'offensive'
        DrawPatrole();
    }
    /*
    It handles updating the enemy's health, performing raycasts to detect targets,
     and drawing the patrol path.
    */

    public override void DecisionTree() {
        if (steps <= 0 || isMoving) return;
        // Debug.Log(name + " decision tree " + action);
        AtLocation();
        if (action == Action.Idle) {
            Deactivate();
            return;
        }
        if (Vector3.Distance(transform.position, targetLocation) <= 0.4f) {
            Debug.Log(name);
            Debug.Log("targetLocation: " + targetLocation);
            Debug.Log("position: " + transform.position);
            Debug.LogError("Going to self!");
        }

        int tries = 2;
        while (tries-- > 0) {
            Tile nextTile = GetTileAtPosition(targetLocation);
            if (nextTile == null) {
                Search();
                break;
            }

            AStarTargetTile = nextTile;
            // Debug.Log(name + " nextTile: " + nextTile);
            // Debug.Log("AStarTargetTile: " + AStarTargetTile);
            bool pathFound = CalculatePath(nextTile);
        
            // If no path was found, go idle
            if (pathFound) return;
            Search();
        }
        action = Action.Idle;
    }
    /*
    The DecisionTree method is overridden from the base class and contains the enemy's decision-making logic. 
    It determines the appropriate action based on the current state of the enemy and invokes corresponding methods.
    */

    private void AtLocation() {
        // Debug.Log(name + " action: " + action);
        switch (action) {
            case Action.Damaged:
                Investegate(targetLocation);
                break;
            case Action.Attacking:
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
            case Action.Idle:
                Idle();
                return;
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
    //method generates a random position on the NavMesh within a given radius

    private Vector3[] GenerateRandomPath(int pathLength) {
        Vector3[] path = new Vector3[pathLength];
        for (int i = 0; i < pathLength; i++)
        {
            Vector3 nextPoint = RandomNavmeshLocation(1000f);
            path[i] = nextPoint;
        }
        return path;
    }
    //method generates a random patrol path by calling RandomNavmeshLocation multiple times.

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
    /*
    ethod retrieves a predefined patrol path from child objects named "Path"
     and stores it as an array of positions.
    */

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
    
    // Have unit go idle 
    private void Idle() {
        action = Action.Idle;
        targetLocation = transform.position; // Target location to current location
    }

    private void Patrole() {
        // Debug.Log(name + " patrole");
        Vector3 position = transform.position;
        Vector3 targetPosition = targetLocation;
        position.y = targetPosition.y = 0;
        float dist = Vector3.Distance(position, targetPosition);
        if (dist <= 0.5f) nextPathPoint();
        targetLocation = patrolPoints[patrolPoint];
        action = Action.Patroling;
        searchTxt.enabled = false;
    }

    /*
    The Patrole method handles the enemy's patrolling behavior.
    It checks the distance between the enemy and the target patrol point and updates the target location accordingly. 
    It also updates the action to "Patrolling".
    */

    protected void nextPathPoint() {
        // Debug.Log(name + " controlePoint " + patrolPoint);
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
    //sets the enemy's action to "Investigating" and updates the target location to the specified position.

    private void Search() {
        // Debug.Log(name + " search");
        action = Action.Searching;
        targetLocation = RandomNavmeshLocation(moveRange);
        alertTxt.enabled = false;
        searchTxt.enabled = true;
        //MoveTo(nextTile);
    }

    private void LookAround() {
        action = Action.Scouting;
        targetLocation = transform.position;
    }
}