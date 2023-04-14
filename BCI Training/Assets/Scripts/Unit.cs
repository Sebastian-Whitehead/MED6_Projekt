using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : PlayerMove {

    protected TurnManager turnManager;

    public enum Action {
        Idle,
        Patroling,
        Attacking,
        Searching,
        Scouting,
        Investegating
    };
    
    public Action action;
    protected bool hasAttacked = false;

    [Header("Character")]
    public float maxHealth = 10f;
    public float attackRange = 1f;
    public float damage = 2f;
    public float health;
    public bool alive = true;
    public bool offensive = false;

    [Header("View")]
    [Range(2, 15)] public int inc = 5;
    [Range(1, 180)] public int FOV = 25;
    [Range(0f, 100f)] public float distance = 10f;
    public string targetTag;
    protected Vector3 targetLocation;
    protected Unit target;

    [Header("Debug")]
    protected Color viewColor = Color.green;
    protected Color moveColor;
    protected bool execute;

    [Header("Manager")]
    public bool active = false;
    public abstract void TakeDamage(Vector3 hitPosition, float damageTaken);
    protected abstract void UnitGone();
    protected abstract void ChildAwake();
    protected abstract void ChildUpdate();
    public abstract void DecisionTree();
    protected abstract bool AttackCheck();

    void Awake() {
        turnManager = GameObject.Find("GameManager").GetComponent<TurnManager>();
        //target = GameObject.FindGameObjectWithTag(targetTag).GetComponent<Unit>();
        target = null;
        health = maxHealth;
        ChildAwake();
    }

    public void Update() {
        if (!alive) return;
        Eyes(); // Raycast see targets depended to 'offensive'
        ChildUpdate(); // Subclasses Update method
        if (!execute && tag == "Player") return; // Execute on confirm
        AttackTarget(); // Attack target, if set
        DecisionTree();
        if (!isMoving) return; // Break when not moving
        Move(); // Move to target tile
        if (path.Count <= 0) Deactivate(); // Deactivate when at location
    }

    protected void Alive() {
        health = Mathf.Max(health, 0);
        if (health > 0) return;
        alive = false;
        Destroy(gameObject);
    }

    private void Eyes() {
        if (!offensive) return;
        
        inc = Mathf.Max(2, inc);
        RaycastHit hit;

        Vector3 dir = targetLocation - transform.position;
        Debug.DrawRay(transform.position, dir, viewColor);

        for (int angle = -FOV; angle <= FOV; angle += inc) {
            Vector3 targetPos = new Vector3(0, 0, 0);
            targetPos += Quaternion.AngleAxis(angle, Vector3.up) * transform.forward * distance;
            Debug.DrawRay(transform.position, targetPos, viewColor);
            if (!Physics.Raycast(transform.position, targetPos, out hit, distance)) continue;
            if (hit.transform.tag != targetTag) continue;
            SetTarget(hit.transform);
            return;
        }
        UnitGone();
    }

    private void Idle() {
        action = Action.Idle;
        targetLocation = transform.position;
    }

    // Set target to chase/attack
    private void SetTarget(Transform tmpTarget) {
        target = tmpTarget.GetComponent<Unit>(); // Get target 'Unit' script
        targetLocation = tmpTarget.position; // Get target location
        action = Action.Attacking;
        Debug.Log(transform.name + " set target: " + target.name);
        if (tag == "Enemy") Deactivate();
    }

    // Attack target, if set and close enough
    private void AttackTarget() {
        if (hasAttacked) return;
        if (action != Action.Attacking) return;
        if (target == null) return; // Break at null target
        if (tag == "Enemy" && OutOfRange(targetLocation)) return; // Break when target not in range
        if (!offensive) return; // Break at not offensive
        if (!AttackCheck()) return; // Break attack not possible

        Debug.Log(transform.name + " attacking " + target.name);
        target.TakeDamage(transform.position, damage); // Target takes damage
        //audioManager.PlayCategory("Attack"); // Play attack sound effect
        hasAttacked = true;
        Deactivate(); // Deactivate unit
    }

    private bool OutOfRange(Vector3 targetPos) {
        Vector3 playerPos = gameObject.transform.position;
        float distance = Vector3.Distance(playerPos, targetPos);
        if (distance > attackRange) {
            Debug.Log("Out of range");
            BFS();
            MoveTo(GetTileAtPosition(targetPos)); 
            return true;
        }
        return false; 
    }

    // ---------------------------------------------------------------------

    public void Activate() {
        active = true;
        hasAttacked = false;
        steps = moveRange;
        turnManager.Wait();
    }

    public void Deactivate() {
        if ((tag == "Enemy" && (steps <= 0 || !isMoving)) || 
        (tag == "Player" && steps <= 0) || hasAttacked) {
            active = false;
            isMoving = false;
            steps = 0;
        }
    }

    public bool Active() {
        return active;
    }
}