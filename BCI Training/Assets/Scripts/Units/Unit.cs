using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Unit : PlayerMove {

    protected TurnManager turnManager;

    public enum Action { // Possible actions
        Idle,
        Patroling,
        Attacking,
        Searching,
        Scouting,
        Investegating,
        Attacked
    };
    
    public Action action; // Current action

    [Header("Character")] // 
    public float attackRange = 1f; // Range of attack
    public float damage = 2f; // Damage target on attack

    [Header("View")] // Viewing propeties
    [Range(2, 15)] public int inc = 5; // Increment and rate of raycast
    [Range(1, 180)] public int FOV = 25; // Field of view [1-180]
    [Range(0f, 100f)] public float distance = 10f; // Raycast/view distance
    public string targetTag; // Tag unit will target and attack
    protected Vector3 targetLocation; // Target location to go to (check usage)
    protected Unit attackTarget; // Target to attack

    [Header("Debug")] // Gizmo propeties
    protected Color viewColor = Color.green; // Visualize eyes raycast
    protected Color moveColor; // Visualize moving 

    [Header("Manager")] // Managing variables
    public bool active = false; // Actions are possible
    public bool execute = false; // Execution of current action
    protected AudioManager audioManager;
    public bool hasSpotted = false;
    public bool hasAttacked = false; // Has attacked this turn

    // ---------------------------------------------------------------------

    // Abstrac methods
    public abstract void TakeDamage(Vector3 hitPosition, float damageTaken);
    protected abstract void UnitGone(); // Target is gone after marked
    protected abstract void ChildAwake(); // Sub-class method of 'Awake'
    protected abstract void ChildUpdate(); // Sub-class method of 'Update'
    public abstract void DecisionTree(); // Enemy decision tree to pick action
    protected abstract bool AttackCheck(); // Check possible attack
    public abstract bool Alive(); // Check if unit is alive

    void Awake() {
        turnManager = GameObject.Find("GameManager").GetComponent<TurnManager>();
        //attackTarget = GameObject.FindGameObjectWithTag(targetTag).GetComponent<Unit>();
        audioManager = GetComponent<AudioManager>();
        attackTarget = null; // Null set attack target
        ChildAwake(); // Sub-class method call
    }

    public void Update() {
        if (!Alive() && active) Deactivate();
        if (!Alive()) return;
        ChildUpdate(); // Subclasses Update method
        if (!execute && tag == "Player") return; // Execute on confirm btn
        AttackTarget(); // Attack target, if set
        DecisionTree(); // Decision tree to pick next action
        if (steps <= 0 && active) Deactivate();
        if (!isMoving) return; // Break when not moving
        Move(); // Move to target tile
        if (path.Count <= 0 && active){ 
            Deactivate();
            turnManager.waiting = true;
        }
    }

    // Eyes to spot game objects matching with 'target tag'
    protected void Eyes() {
        
        inc = Mathf.Max(2, inc); // Minimalize increments
        RaycastHit hit;

        for (int angle = -FOV; angle <= FOV; angle += inc) {
            Vector3 targetPos = new Vector3(0, 0, 0); // Intilize a zero-vector
            // Get angle from for-loop and object forward direction
            targetPos += Quaternion.AngleAxis(angle, Vector3.up) * transform.forward * distance;
            Vector3 projection = transform.position;
            projection.y += gameObject.GetComponent<Collider>().bounds.extents.y / 2;
            Debug.DrawRay(projection, targetPos, viewColor); // Visualize raycast
            if (!Physics.Raycast(projection, targetPos, out hit, distance)) continue;
            if (hit.transform.tag != targetTag) continue; // Only matching 'target tag'
            SetTarget(hit.transform); // Set target to hit game object
            return; // Break loop
        }
    }

    protected void SetTarget(Transform tmpTarget) {
        if (hasSpotted) return;
        attackTarget = tmpTarget.GetComponent<Unit>();

        if (CompareTag("Player")) return;

        // Debug.Log(name + " isMoving: " + isMoving);
        // Debug.Log(name + " Player spotted");
        
        action = Action.Attacking;
        hasSpotted = true;
        audioManager.PlayCategory("SpotPlayer");
        targetLocation = attackTarget.transform.position;
        
        FindPlayer();
        Tile nextTile = GetTileAtPosition(targetLocation);
        AStarTargetTile = nextTile;
        //Debug.Log("AStarTargetTile: " + AStarTargetTile);
        bool pathFound = CalculatePath(nextTile);

        // If no path was found, go idle
        if (!pathFound) {
            action = Action.Idle;
        }
    }

    // Attack target, if set and close enough
    private void AttackTarget() {
        if (hasAttacked) return; // Guard already attacked
        if (attackTarget == null) return; // Break at null attack target
        if (!active) return; // Guard activity
        if (!AttackCheck()) return; // Break attack not possible
        Debug.Log(name + " attacking " + attackTarget.name);
        if (CompareTag("Enemy")) attackTarget.TakeDamage(transform.position, damage); // Target takes damage
        transform.LookAt(attackTarget.transform.position, Vector3.up);
        audioManager.PlayCategory("Attack"); // Play attack sound effect
        hasAttacked = true;
        
        //Deactivate(); // Deactivate unit
    }

    // ---------------------------------------------------------------------

    // Activate unit
    public void Activate() {
        active = true; // Reset active
        hasAttacked = false; // Reset attacked
        steps = moveRange + 1; // Reset steps plus one
        turnManager.Wait(); // TurnManager wait for unit to finish execution
        hasSpotted = false;
        execute = false;
    }

    // Deactivate unit 
    public void Deactivate() {
        // Debug.Log("Deactivate " + name);
        active = false; // Disable activity
        isMoving = false; // Disable moving
        steps = 0; // Remove all steps
        attackTarget = null;
    }

    // Return activity
    public bool Active() {
        return active;
    }


}