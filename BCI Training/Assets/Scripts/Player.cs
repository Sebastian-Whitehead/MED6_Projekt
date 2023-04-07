using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit {

    public new Action action = Action.Idle;

    public new enum Action {
        Idle,
        Chasing
    }

    protected override void ChildAwake() {
    }

    protected override void ChildUpdate() {
        CheckMouseClick();
    }

    void CheckMouseClick() {

        if (!active) return;
        if (!isMoving) BFS();
        if (!Input.GetMouseButtonDown(0)) return;
        offensive = false;
        if (turnManager.turn != TurnManager.Turn.Player) return;
        target = null;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity)) return;
        MoveToGrid(hit.collider);
        SetTarget(hit.collider);
        
        Deactivate();
        
    }

    void SetTarget(Collider collider) {
        if (collider.tag == targetTag) 
        Debug.Log("Set target: " + collider.name);
        targetLocation = collider.transform.position;
        action = Action.Chasing;
        offensive = true;
        Activate();
    }

    protected override void UnitGone() { }
    public override void AtLocation() {}

    public override void TakeDamage(Vector3 hitPosition, float damageTaken) {
        health -= damage;
        action = Action.Idle;
    }
}
