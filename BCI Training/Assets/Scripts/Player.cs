using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Unit {

    // State the player is about to execute
    public State state = State.Idle;
    public enum State {
        Idle,
        Attack,
        Move,
        Charge
    }
    public LayerMask PlayerLayer;

    private Resources res; // Health and mana
    public Button confirmBtn; // Execute action
    
    protected override void ChildAwake() {
        confirmBtn.onClick.AddListener(ConfirmAction); // Confirm action btn
        res = GetComponent<Resources>(); // Get resources
    }

    protected override void ChildUpdate() {
        res.Alive(audioManager); // Alive check
        CheckMouseClick(); // Mouse click check
        if (!execute) return; // Only on execute
        if (state == State.Charge) res.RegenMana();
        
    }

    protected override void UnitGone() {}
    public override void DecisionTree() {}
    
    // Check mouse click to move, attack
    void CheckMouseClick() {

        if (!turnManager.playerTurn) return; // Turn check
        if (!active) return; // Check activity
        if (!isMoving) BFS(); // Breath search to moveable location
        if (steps <= 0) Deactivate();
        if (!Input.GetMouseButtonDown(0)) return; // Click check
        
        target = null; // Enemy target
        execute = false; // Action execution

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, ~PlayerLayer)) return;
        SetMoveTarget(hit.collider); // Set tile to move to
        SetAttackTarget(hit.collider); // Set enemy to attack
        ActivateBtn(); // Activate confirm btn
    }

    // Activate confirm button
    private void ActivateBtn() {
        if (state == State.Idle) return;
        // Debug.Log("Activate Confirm Btn");
        confirmBtn.interactable = true;
        ConfirmBtn conBtn = confirmBtn.GetComponent<ConfirmBtn>(); // Confirm script
        conBtn.UpdateSprite(state.ToString()); // Update confirm sprite
    }

    // Confirm action
    private void ConfirmAction() {
        if (state == State.Idle) return;
        // Debug.Log("Confirm action");
        execute = true; // Execute action
        confirmBtn.interactable = false; // Deactivate confirm btn
        state = State.Idle; // Idle player
        //Deactivate(); // Deactivate player
        active = false;
    }

    // Ready moving
    void SetMoveTarget(Collider collider) {
        if (collider.tag == "Tile") {
            // Debug.Log("Set move: " + collider.name);
            state = State.Move;
            Tile t = collider.GetComponent<Tile>();
            if (t.selectable) MoveTo(t);
        }
    }

    // Ready attacking
    void SetAttackTarget(Collider collider) {
        if (collider.tag != targetTag) return;
        // Debug.Log("Set attack: " + collider.name);
        targetLocation = collider.transform.position; // Set target to enemy location
        state = State.Attack;
        transform.LookAt(targetLocation, Vector3.up);
        offensive = true; // Enable attack mode
    }

    private float RangeChance(Vector3 targetPos) {
        Vector3 playerPos = gameObject.transform.position;
        float distance = Vector3.Distance(playerPos, targetPos);
        float hitChance = 100 - Mathf.Pow(distance, 2);
        return hitChance;
    }

    // Ready mana charging
    void ReadyCharge() {
        Debug.Log("Ready: Charge");
        state = State.Charge;
    }

    void ChargeMana() {
        audioManager.PlayCategory("ManaCharge");
        res.RegenMana();
        state = State.Idle;
    }

    public override void TakeDamage(Vector3 hitPosition, float damageTaken) {
        //action = Action.Idle;
        res.Damage(damageTaken);
        audioManager.PlayCategory("TakeDamage");
    }

    public void Reset() {
        offensive = false;
        target = null;
    }

    // Check if player can attack
    protected override bool AttackCheck() {
        if (!res.ManaCheck()) return false;
        return true;
    }
}
