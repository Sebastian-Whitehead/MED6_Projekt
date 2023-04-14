using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit {

    public new Action action = Action.Idle;

    public new enum Action {
        Idle,
        Chasing
    }

    private Resources res;

    protected override void ChildAwake() {
        res = GetComponent<Resources>();
    }

    protected override void ChildUpdate() {
        res.Alive(audioManager);
        CheckMouseClick();
    }

    void CheckMouseClick() {
        offensive = false;

        if (!active) return;
        if (!isMoving) BFS();
        if (!Input.GetMouseButtonDown(0)) return;
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
        if (collider.tag != targetTag) return;
        Debug.Log("Set target: " + collider.name);
        targetLocation = collider.transform.position;
        action = Action.Chasing;
        offensive = true;
    }

    protected override void UnitGone() { }
    public override void AtLocation() {}

    public override void TakeDamage(Vector3 hitPosition, float damageTaken) {
        res.Damage(damageTaken);
        audioManager.PlayCategory("TakeDamage");
        action = Action.Idle;
    }

    protected override bool AttackCheck() {
        if (!res.ManaCheck()) return false;
        return true;
    }
}
