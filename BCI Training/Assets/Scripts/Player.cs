using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit {

    public LayerMask PlayerLayer;

    protected override void ChildAwake() { }

    protected override void ChildUpdate() {
        CheckMouseClick();
    }

    protected override void UnitGone() {}
    public override void DecisionTree() {}

    void CheckMouseClick() {
        // if (turnManager.turn != TurnManager.Turn.Player) return;
        if (!active) return;
        if (!isMoving) BFS();
        if (steps <= 0) Deactivate();
        if (!Input.GetMouseButtonDown(0)) return;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, ~PlayerLayer)) return;
        if (hit.collider.tag == targetTag) {
            MarkTarget(hit.collider);
            action = Action.Attacking;
            return;
        } else {
            MoveToGrid(hit.collider);
        }
    }

    private void MarkTarget(Collider collider) {
        Debug.Log("Mark: " + collider.name);
        Vector3 targetPos = collider.transform.position;
        transform.LookAt(targetPos, Vector3.up);
        offensive = true;
    }

    private float RangeChance(Vector3 targetPos) {
        Vector3 playerPos = gameObject.transform.position;
        float distance = Vector3.Distance(playerPos, targetPos);
        float hitChance = 100 - Mathf.Pow(distance, 2);
        return hitChance;
    }

    public override void TakeDamage(Vector3 hitPosition, float damageTaken) {
        health -= damage;
        //action = Action.Idle;
    }

    public void Reset() {
        offensive = false;
        target = null;
    }
}
