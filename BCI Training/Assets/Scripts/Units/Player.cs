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

    private PlayerFeatures res; // Health and mana
    public Button confirmBtn; // Execute action
    private ConfirmBtn conBtn; // Confirm button script
    public GameObject[] tiles;
    
    protected override void ChildAwake() {
        confirmBtn.onClick.AddListener(ConfirmAction); // Confirm action btn
        conBtn = confirmBtn.GetComponent<ConfirmBtn>(); // Confirm script
        res = GetComponent<PlayerFeatures>(); // Get resources
    }

    protected override void ChildUpdate() {
        res.Alive(audioManager); // Alive check
        if (!execute) return; // Only on execute
        if (state == State.Charge) res.RegenMana();
        
    }

    protected override void UnitGone() {}
    public override void DecisionTree() {}

    // Check mouse click to move, attack
    void LateUpdate() {

        if (execute || !active) return;
        if (!turnManager.playerTurn) return; // Turn check
        if (!isMoving) BFS(); // Breath search to moveable location
        if (!Input.GetMouseButtonDown(0)) return; // Click check
        if (state == State.Idle) ResetConfirmBtn();

        Dehighlight(); // Dehighlight all enemies
        RemoveTileHighlight();
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
        conBtn.UpdateSprite(state.ToString()); // Update confirm sprite
    }

    void ResetConfirmBtn() {
        conBtn.DisableImage();
    }

    // Confirm action
    public void ConfirmAction() {
        if (state == State.Idle) return;
        // Debug.Log("Confirm action");
        execute = true; // Execute action
        conBtn.DisableImage(); // Deactivate confirm btn
    }

    // Ready moving
    void SetMoveTarget(Collider collider) {
        isMoving = false; // Reset moving
        if (collider.tag != "Tile") return;
        // Debug.Log("Move target: " + collider.name);
        state = State.Move; // Update state to move
        Tile t = collider.GetComponent<Tile>(); // Get tile script
        if (t.selectable) MoveTo(t); // Move to selectable
        else ResetPlayer(); // Reset when not selectable
    }

    // Ready attacking
    void SetAttackTarget(Collider collider) {
        attackTarget = null; // Reset attack target
        if (collider.tag != targetTag) return;
        // Debug.Log("Attack target: " + collider.name);
        targetLocation = collider.transform.position; // Set target to enemy location
        transform.LookAt(targetLocation, Vector3.up);
        state = State.Attack; // Attack state player
        Eyes(); // Enemy visible from player (TODO: CHECK IF COLLIDER IS TARGET)
        if (attackTarget == null) ResetPlayer(); // Break at no target
        else Highlight(collider); // Highlight target
    }

    void Highlight(Collider target) {
        target.GetComponent<Highlight>().selected = true;
    }

    void Dehighlight() {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies) {
            enemy.GetComponent<Highlight>().selected = false;
        }
    }

    private float RangeChance(Vector3 targetPos) {
        Vector3 playerPos = gameObject.transform.position;
        float distance = Vector3.Distance(playerPos, targetPos);
        float hitChance = 100 - Mathf.Pow(distance, 2);
        return hitChance;
    }

    // Ready mana charging
    void ReadyCharge() {
        // Debug.Log("Ready: Charge");
        state = State.Charge;
    }

    void ChargeMana() {
        audioManager.PlayCategory("ManaCharge");
        res.RegenMana();
        state = State.Idle;
    }

    public override void TakeDamage(Vector3 hitPosition, float damageTaken) {
        res.Damage(damageTaken);
        audioManager.PlayCategory("TakeDamage");
    }

    public void ResetPlayer() {
        // Debug.Log("Reset player");
        state = State.Idle;
        execute = false;
    }

    // Check if player can attack
    protected override bool AttackCheck() {
        if (!res.ManaCheck()) {
            Debug.Log(name + " no mana");
            ResetPlayer(); // Stop execution at no mana
            return false;
        }
        res.Expend(); // Decrease mana
        return true;
    }

    protected override bool Alive() {
        return res.alive;
    }

    private void RemoveTileHighlight() {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach(var obj in tiles)
        {
            Tile tile = obj.GetComponent<Tile>();
            if (tile.targetTile)
            {
                tile.ResetTile();
                tile.selectable = true;
            }

        }
    }
}
