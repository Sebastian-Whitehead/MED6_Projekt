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
        Investegating
    };
    
    public Action action; // Current action
    protected bool hasAttacked = false; // Has attacked this turn

    [Header("Character")] // 
    public float maxHealth = 10f;
    public float attackRange = 1f; // Range of attack
    public float damage = 2f; // Damage target on attack
    public float health;
    public bool alive = true; // Unit is alive
    public bool offensive = false; // Spot and attack targets

    [Header("View")] // Viewing propeties
    [Range(2, 15)] public int inc = 5; // Increment and rate of raycast
    [Range(1, 180)] public int FOV = 25; // Field of view [1-180]
    [Range(0f, 100f)] public float distance = 10f; // Raycast/view distance
    public string targetTag; // Tag unit will target and attack
    protected Vector3 targetLocation; // Target location to go to (check usage)
    protected Unit target; // Target to attack

    [Header("Debug")] // Gizmo propeties
    protected Color viewColor = Color.green; // Visualize eyes raycast
    protected Color moveColor; // Visualize moving 

    [Header("Manager")] // Managing variables
    public bool active = false; // Actions are possible
    protected bool execute = false; // Execution of current action
    protected AudioManager audioManager;

    // ---------------------------------------------------------------------

    // Abstrac methods
    public abstract void TakeDamage(Vector3 hitPosition, float damageTaken);
    protected abstract void UnitGone(); // Target is gone after marked
    protected abstract void ChildAwake(); // Sub-class method of 'Awake'
    protected abstract void ChildUpdate(); // Sub-class method of 'Update'
    public abstract void DecisionTree(); // Enemy decision tree to pick action
    protected abstract bool AttackCheck(); // Check possible attack


    void Awake() {
        turnManager = GameObject.Find("GameManager").GetComponent<TurnManager>();
        //target = GameObject.FindGameObjectWithTag(targetTag).GetComponent<Unit>();
        audioManager = GetComponent<AudioManager>();
        target = null; // Null set target
        health = maxHealth;
        ChildAwake(); // Sub-class method call
    }

    public void Update() {
        if (!alive) return;
        Eyes(); // Raycast see targets depended to 'offensive'
        ChildUpdate(); // Subclasses Update method
        if (!execute && tag == "Player") return; // Execute on confirm
        AttackTarget(); // Attack target, if set
        DecisionTree(); // Decision tree to pick next action
        if (tag == "Enemy" && !isMoving && steps < moveRange) Deactivate();
        if (!isMoving) return; // Break when not moving
        Move(); // Move to target tile
        if (path.Count <= 0) Deactivate(); // Deactivate when at location
    }

    // Positive and alive method
    protected void Alive() {
        if (health > 0) return; // Break at positive health
        health = Mathf.Max(health, 0); // Zero minimalize health value
        alive = false; // Disable living
        Destroy(gameObject); // Destroy game object (Temporary until animation)
    }

    // Eyes to spot game objects matching with 'target tag'
    private void Eyes() {
        if (!offensive) return; // Break at not offensive
        
        inc = Mathf.Max(2, inc); // Minimalize increments
        RaycastHit hit;

        for (int angle = -FOV; angle <= FOV; angle += inc) {
            Vector3 targetPos = new Vector3(0, 0, 0); // Intilize a zero-vector
            // Get angle from for-loop and object forward direction
            targetPos += Quaternion.AngleAxis(angle, Vector3.up) * transform.forward * distance;
            Debug.DrawRay(transform.position, targetPos, viewColor); // Visualize raycast
            if (!Physics.Raycast(transform.position, targetPos, out hit, distance)) continue;
            if (hit.transform.tag != targetTag) continue; // Only matching 'target tag'
            SetTarget(hit.transform); // Set target to hit game object
            audioManager.PlayCategory("SpotPlayer");
            return; // Break loop
        }
    }

    // Have unit go idle 
    private void Idle() {
        action = Action.Idle;
        targetLocation = transform.position; // Target location to current location
    }

    protected void SetTarget(Transform tmpTarget)
    {
        //Debug.Log(currentTile.transform.position);
        action = Action.Attacking;
        target = tmpTarget.GetComponent<Unit>();
        targetLocation = tmpTarget.position;

        if (tag == "Player") return;
        if (isMoving) return;
        if (turnManager.playerTurn) return;

        audioManager.PlayCategory("SpotPlayer");
        FindPlayer();
        BFS();
        CalculatePath();

    }

    // Attack target, if set and close enough
    private void AttackTarget() {
        if (hasAttacked) return;
        if (target == null) return; // Break at null target
        if (tag == "Enemy" && OutOfRange(targetLocation)) return; // Break when target not in range
        if (!offensive) return; // Break at not offensive
        if (!AttackCheck()) return; // Break attack not possible

        Debug.Log(transform.name + " attacking " + target.name);
        target.TakeDamage(transform.position, damage); // Target takes damage
        audioManager.PlayCategory("Attack"); // Play attack sound effect
        hasAttacked = true;
        Deactivate(); // Deactivate unit
    }

    // Check if distance to given position is matching attack range 
    private bool OutOfRange(Vector3 targetPos) {
        Vector3 playerPos = gameObject.transform.position; 
         // Calc. distance from unit to target
        float distance = Vector3.Distance(playerPos, targetPos);
        if (distance > attackRange) { // Break at distance more than range
            Debug.Log("Out of range");
            // Walk to target instead
            BFS(); // Breath search tiles
            MoveTo(GetTileAtPosition(targetPos)); // Move towards target
            return true; // Abort method
        }
        return false; // Confirm method
    }

    // ---------------------------------------------------------------------

    // Activate unit
    public void Activate() {
        active = true; // Reset active
        hasAttacked = false; // Reset attacked
        steps = moveRange; // Reset steps
        turnManager.Wait(); // TurnManager wait for unit to finish execution
    }

    // Deactivate unit 
    public void Deactivate() {
        if ((tag == "Enemy" && (steps <= 0 || !isMoving)) || 
        (tag == "Player" && steps <= 0) || hasAttacked) {
            active = false; // Disable activity
            isMoving = false; // Disable moving
            steps = 0; // Remove all steps
        }
    }

    // Return activity
    public bool Active() {
        return active;
    }


}