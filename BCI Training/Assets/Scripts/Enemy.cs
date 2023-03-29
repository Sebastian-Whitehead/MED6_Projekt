using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public int mapWidth = 10;
    public int mapHeight = 10;

    [Header("Action attributes")]
    public Action currentAction = Action.Patrole;
    public bool startTurn = false;
    private bool executing = false;
    private GameObject player;

    [Header("Character attributes")]
    public float maxHealth = 10f;
    public int moveSpeed = 2;
    public int moveDistance = 2;
    public float attackDistance = 1f;
    public float damage = 2f;
    private float health;
    
    [Header("View attributes")]
    [Range(2, 15)] public int inc = 5;
    [Range(1, 180)] public int FOV = 25;
    [Range(0f, 10f)] public float distance = 10f;
    
    [Header("Movement attributes")]
    private Vector3 location;
    private int patrolPoint = 0;
    private Vector3[] patrolPoints;
    private bool circlePatrole = false;
    Color viewColor, moveColor;

    public enum Action {
        Idle,
        Patrole,
        ChasePlayer,
        ScoutArea,
        LookAround,
    }

    // ---------------------------------------------------------------------

    public void StartTurn() {
        startTurn = true;
    }

    public void endTurn() {
        executing = false;
    }

    public void SetAction(Action nextAction) {
        currentAction = nextAction;
    }

    // ---------------------------------------------------------------------

    // Start is called before the first frame update
    void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
        health = maxHealth;
        patrolPoints = new [] {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 1),
            new Vector3(3, 0, 0),
            transform.position,
        };
        patrolPoints = GenerateRandomPath(5);
        endTurn();
    }

    // Update is called once per frame
    void Update() {
        if (health <= 0) Dead();

        Eyes();
        DrawPath();

        PickAction();
        ExecuteAction();
    }

    // ---------------------------------------------------------------------
    
    private Vector3 GenerateRandomPoint(Vector3 min, Vector3 max) {
        float randomX = UnityEngine.Random.Range(min.x, max.x);
        float randomZ = UnityEngine.Random.Range(min.z, max.z);
        Vector3 randomPoint = new Vector3(randomX, 0, randomZ);
        return randomPoint;
    }

    private Vector3[] GenerateRandomPath(int pathLength) {
        Vector3[] path = new Vector3[pathLength];
        Vector3 minPoint = new Vector3(0, 0, 0);
        Vector3 maxPoint = new Vector3(mapWidth, 0, mapHeight);
        for (int i = 0; i < pathLength; i++) {
            Vector3 randomPoint = GenerateRandomPoint(minPoint, maxPoint);
            CheckPointInsideMap(randomPoint);
            path[i] = randomPoint;
        }
        return path;
    }

    private bool CheckPointInsideMap(Vector3 point) {
        // INCOMPLETE
        return false;
    }

    private void DrawPath() {

        viewColor = Color.green;
        moveColor = Color.blue;
        if (currentAction == Action.ChasePlayer) {
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

    private void Eyes() {

        if (currentAction == Action.ChasePlayer) return;

        inc = Mathf.Max(2, inc);
        RaycastHit hit;

        for (int angle = -FOV; angle <= FOV; angle += inc) {
            Vector3 targetPos = new Vector3(0, 0, 0);
            targetPos += Quaternion.AngleAxis(angle, Vector3.up) * transform.forward * distance;

            Debug.DrawRay(transform.position, targetPos, viewColor);

            if (Physics.Raycast(transform.position, targetPos, out hit, distance)) {
                if (hit.transform.tag == "Player") {
                    Debug.Log("Player spotted!");
                    currentAction = Action.ChasePlayer;
                    endTurn();
                    return;
                }
            }
        }
    }

    // ---------------------------------------------------------------------

    private void PickAction() {
        if (!startTurn) return;

        switch (currentAction) {
            case Action.Idle:
                Idle();
                break;
            case Action.Patrole:
                Patroling();
                break;
            case Action.ChasePlayer:
                ChasePlayer();
                break;
            case Action.ScoutArea:
                ScoutArea();
                break;
            case Action.LookAround:
                LookAround();
                break;
            default:
                Debug.Log("Error: Action not found.");
                break;
        }
        startTurn = false;
        executing = true;
    }

    private void ExecuteAction() {
        if (!executing) return;

        AttackPlayer();
        MoveToLocation();
    }

    // ---------------------------------------------------------------------

    private void Idle() {
        location = transform.position;
    }

    private void Patroling() {
        location = patrolPoints[patrolPoint];
        location.y = transform.position.y;

        float distanceToPatrolPoint = Vector3.Distance(transform.position, location);
        if (++patrolPoint >= patrolPoints.Length) patrolPoint = 0;
    }

    private void ChasePlayer() {
        location = player.transform.position;
    }

    private void ScoutArea() {
        Vector3 minDistance = new Vector3(-1, 0, -1) * moveDistance;
        Vector3 maxDistance = new Vector3(1, 0, 1) * moveDistance;
        Vector3 randomPoint = GenerateRandomPoint(minDistance, maxDistance);
        Vector3 currentLocation = transform.position;
        Vector3 nextLocation = currentLocation + randomPoint;
        CheckPointInsideMap(nextLocation);
        location = nextLocation;
    }

    private void LookAround() {
        location = transform.position;
    }

    // ---------------------------------------------------------------------

    private void AttackPlayer() {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer > attackDistance) return;

        location = transform.position;
        Debug.Log("Attack!");
        endTurn();
    }

    private void MoveToLocation() {
        float distanceToLocation = Vector3.Distance(transform.position, location);
        if (distanceToLocation < 0.01f) endTurn();

        transform.LookAt(location, Vector3.up);

        float step = moveSpeed * Time.deltaTime;
        Debug.DrawLine(transform.position, location, moveColor);
        transform.position = Vector3.MoveTowards(transform.position, location, step);
    }

    // ---------------------------------------------------------------------

    private void TakeDamage(float damageTaken) {
        health -= damageTaken;
    }

    private void Dead() {
        Destroy(gameObject);
    }
}
