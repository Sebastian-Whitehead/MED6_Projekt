using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {


    [Header("Action")]
    public Action currentAction = Action.Patrole;
    public bool startTurn = false;
    private bool executing = false;
    private GameObject player;

    [Header("Character")]
    public float maxHealth = 10f;
    public float attackDistance = 1f;
    public float damage = 2f;
    private float health;
    
    [Header("View")]
    [Range(2, 15)] public int inc = 5;
    [Range(1, 180)] public int FOV = 25;
    [Range(0f, 10f)] public float distance = 10f;
    
    [Header("Movement")]
    public int moveSpeed = 2;
    public int moveDistance = 2;
    private Vector3 targetLocation;
    private int patrolPoint = 1;
    private Vector3[] patrolPoints;
    public bool circlePatrole = false;
    private bool clockwise = true;
    
    [Header("Debug")]
    Color viewColor, moveColor;

    [Header("Elements")]
    public Transform Character;
    public Transform Path;


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

    public bool Executing() {
        return executing;
    }

    public bool Active() {
        return startTurn;
    }

    // ---------------------------------------------------------------------

    // Start is called before the first frame update
    void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
        health = maxHealth;
        //patrolPoints = GenerateRandomPath(5);
        patrolPoints = GetManualPath();
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
    

    // https://answers.unity.com/questions/475066/how-to-get-a-random-point-on-navmesh.html
    public Vector3 RandomNavmeshLocation(float radius) {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        UnityEngine.AI.NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) {
            finalPosition = hit.position;            
        }
        return finalPosition;
    }

    private Vector3[] GenerateRandomPath(int pathLength) {
        Vector3[] path = new Vector3[pathLength];
        for (int i = 0; i < pathLength; i++) {
            Vector3 nextPoint = RandomNavmeshLocation(1000f);
            path[i] = nextPoint;
        }
        return path;
    }

    Vector3[] GetManualPath() {
        Transform[] points = Path.transform.GetComponentsInChildren<Transform>();
        Vector3[] path = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++) {
            Transform point = points[i];
            Vector3 position = point.transform.position;
            path[i] = position;
        }
        return path;
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

        inc = Mathf.Max(2, inc);
        RaycastHit hit;

        for (int angle = -FOV; angle <= FOV; angle += inc) {
            Vector3 targetPos = new Vector3(0, 0, 0);
            targetPos += Quaternion.AngleAxis(angle, Vector3.up) * Character.transform.forward * distance;

            Debug.DrawRay(Character.transform.position, targetPos, viewColor);

            if (Physics.Raycast(Character.transform.position, targetPos, out hit, distance)) {
                if (hit.transform.tag == "Player") {
                    currentAction = Action.ChasePlayer;
                    ChasePlayer();
                    return;
                }
            }
        }
        if (currentAction == Action.ChasePlayer) {
            currentAction = Action.ScoutArea;
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
    
    public IEnumerator EnableAfterDelay() {
        startTurn = true;
        executing = true;
        while (executing) {
            yield return null;
        }
    }

    private void ExecuteAction() {
        if (!executing) return;

        AttackPlayer();
        MoveToLocation();
    }

    // ---------------------------------------------------------------------

    private void Idle() {
        targetLocation = Character.transform.position;
    }

    private void Patroling() {
        targetLocation = patrolPoints[patrolPoint];

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

    private void ChasePlayer() {
        targetLocation = player.transform.position;
    }

    private void ScoutArea() {
        targetLocation = RandomNavmeshLocation(moveDistance);
    }

    private void LookAround() {
        targetLocation = Character.transform.position;
    }

    // ---------------------------------------------------------------------

    private void AttackPlayer() {
        float distanceToPlayer = Vector3.Distance(Character.transform.position, player.transform.position);
        if (distanceToPlayer > attackDistance) return;

        targetLocation = Character.transform.position;
        Debug.Log("Attack!");
        endTurn();
    }

    private void MoveToLocation() {
        float distanceToLocation = Vector3.Distance(Character.transform.position, targetLocation);
        if (distanceToLocation < 0.01f) endTurn();

        Character.transform.LookAt(targetLocation, Vector3.up);

        float step = moveSpeed * Time.deltaTime;
        Debug.DrawLine(Character.transform.position, targetLocation, moveColor);
        targetLocation.y = Character.transform.position.y;
        Character.transform.position = Vector3.MoveTowards(Character.transform.position, targetLocation, step);
    }

    // ---------------------------------------------------------------------

    private void TakeDamage(float damageTaken) {
        health -= damageTaken;
    }

    private void Dead() {
        Destroy(gameObject);
    }
}
