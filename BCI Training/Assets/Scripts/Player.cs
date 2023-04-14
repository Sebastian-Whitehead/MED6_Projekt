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

    private Resources res; // Health and mana
    public Button confirmBtn; // Execute action
    
    protected override void ChildAwake() {
        confirmBtn.onClick.AddListener(ConfirmAction); // Confirm action btn
        res = GetComponent<Resources>(); // Get resources
    }

    protected override void ChildUpdate() {
        res.Alive(audioManager); // Alive check
        CheckMouseClick(); // Mouse click check
    }

    // Check mouse click to move, attack
    void CheckMouseClick() {

        if (turnManager.turn != TurnManager.Turn.Player) return; // Turn check
        if (!active) return; // Check activity
        if (!isMoving) BFS(); // Breath search to moveable location
        if (!Input.GetMouseButtonDown(0)) return; // Click check
        
        target = null; // Enemy target
        excecute = false; // Action execution
        offensive = false; // Can attack enemies

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity)) return;
        SetMoveTarget(hit.collider); // Set tile to move to
        SetAttackTarget(hit.collider); // Set enemy to attack
        ActivateBtn(); // Activate confirm btn
    }

    private void ActivateBtn() {
        if (state == State.Idle) return;
        Debug.Log("Activate Confirm Btn");
        confirmBtn.interactable = true;
        ConfirmBtn conBtn = confirmBtn.GetComponent<ConfirmBtn>(); // Confirm script
        conBtn.UpdateSprite(state.ToString()); // Update confirm sprite
    }

    private void ConfirmAction() {
        if (state == State.Idle) return;
        Debug.Log("Confirm action");
        excecute = true; // Execute action
        confirmBtn.interactable = false; // Deactivate confirm btn
        state = State.Idle; // Idle player
        Deactivate(); // Deactivate player
    }

    void SetMoveTarget(Collider collider) {
        if (collider.tag == "Tile") {
            Debug.Log("Set move: " + collider.name);
            state = State.Move;
            Tile t = collider.GetComponent<Tile>();
            if (t.selectable) MoveTo(t);
        }
    }

    void SetAttackTarget(Collider collider) {
        if (collider.tag != targetTag) return;
        Debug.Log("Set attack: " + collider.name);
        // targetLocation = collider.transform.position; // Set target to enemy location
        state = State.Attack;
        transform.LookAt(targetLocation, Vector3.up);
        offensive = true; // Enable attack mode
    }

    protected override void UnitGone() { }
    public override void AtLocation() {}

    public override void TakeDamage(Vector3 hitPosition, float damageTaken) {
        res.Damage(damageTaken);
        audioManager.PlayCategory("TakeDamage");
    }

    protected override bool AttackCheck() {
        if (!res.ManaCheck()) return false;
        return true;
    }
}
