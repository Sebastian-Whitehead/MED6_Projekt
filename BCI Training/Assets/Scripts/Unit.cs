using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : PlayerMove {

    protected TurnManager turnManager;
    public enum Action
    {
        Idle,
        Patroling,
        Chasing,
        ScoutingArea,
        LookingAround,
        Investegating,
    };

    public Action action;

    [Header("Character")]
    public float attackRange = 1f;
    public float damage = 2f;
    public bool offensive = false;

    [Header("View")]
    [Range(2, 15)] public int inc = 5;
    [Range(1, 180)] public int FOV = 25;
    [Range(0f, 10f)] public float distance = 10f;
    public string targetTag;
    protected Vector3 targetLocation;
    protected Unit target;

    [Header("Debug")]
    protected Color viewColor = Color.green;
    protected Color moveColor;

    [Header("Manager")]
    protected bool active = false;
    public bool excecute = false;
    protected AudioManager audioManager;

    public abstract void TakeDamage(Vector3 hitPosition, float damageTaken);
    protected abstract void UnitGone();
    protected abstract void ChildAwake();
    protected abstract void ChildUpdate();
    public abstract void AtLocation();
    protected abstract bool AttackCheck();


    void Awake() {
        turnManager = GameObject.Find("GameManager").GetComponent<TurnManager>();
        target = GameObject.FindGameObjectWithTag(targetTag).GetComponent<Unit>();
        ChildAwake();
        audioManager = GetComponent<AudioManager>();
    }

    public void Update() {
        Eyes();
        ChildUpdate();
        if (!excecute && tag == "Player") return;
        if (isMoving) {
            Move();
            AttackTarget();
            if (path.Count <= 0) Deactivate();
        }
    }

    protected void Eyes() {

        if (!offensive) return;
        
        inc = Mathf.Max(2, inc);
        RaycastHit hit;

        for (int angle = -FOV; angle <= FOV; angle += inc) {
            Vector3 targetPos = new Vector3(0, 0, 0);
            targetPos += Quaternion.AngleAxis(angle, Vector3.up) * transform.forward * distance;

            Debug.DrawRay(transform.position, targetPos, viewColor);

            if (Physics.Raycast(transform.position, targetPos, out hit, distance)) {
                if (hit.transform.tag == targetTag) {
                    ChaseTarget(hit.transform);
                    return;
                }
            }
        }
        UnitGone();
    }

    protected void Idle()
    {
        action = Action.Idle;
        targetLocation = transform.position;
    }

    protected void ChaseTarget(Transform tmpTarget) {
        action = Action.Chasing;
        target = tmpTarget.GetComponent<Unit>();
        if (tag == "Player") return;
        targetLocation = tmpTarget.position;
        AStarTargetTile = GetTileAtPosition(targetLocation);
        
        if (isMoving) return;
        if (turnManager.turn != TurnManager.Turn.Enemies) return;
        audioManager.PlayCategory("SpotPlayer");

        FindPlayer();
        BFS();
        //Debug.Log(currentTile);
        CalculatePath();
        AStarTargetTile.targetTile = true;

    }

    private void AttackTarget() {
        if (!offensive) return;
        if (target == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, target.transform.position);
        if (distanceToPlayer > attackRange) return;

        if (!AttackCheck()) return;

        Unit unit = target.GetComponentInParent<Unit>();
        unit.TakeDamage(transform.position, damage);
        //Debug.Log(transform.name + " attacking " + target.name);
        audioManager.PlayCategory("Attack");
        Deactivate();
    }

    // ---------------------------------------------------------------------

    public void Activate()
    {
        active = true;
        AtLocation();
        turnManager.Wait();
    }

    public void Deactivate()
    {
        active = false;
    }

    public bool Active()
    {
        return active;
    }


}