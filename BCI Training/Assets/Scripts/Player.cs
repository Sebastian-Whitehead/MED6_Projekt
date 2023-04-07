using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit {

    public new Action action = Action.Idle;

    public new enum Action
    {
        Idle,
        Chasing
    }

    protected override void ChildAwake() {
    }

    protected override void ChildUpdate()
    {
        CheckMouseClick();
    }

    void CheckMouseClick() {

        if (!Input.GetMouseButtonDown(0) || IsMoving()) return;
        if (turnManager.turn != TurnManager.Turn.Player)
        {
            Debug.Log(turnManager.turn + " turn");
            return;
        }
        target = null;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity)) return;
        if (hit.collider.tag == "ground") StartCoroutine(GridMove(hit.point, moveSpeed));
        else if (hit.collider.tag == targetTag) SetTarget(hit.collider.transform);
    }

    void SetTarget(Transform tmpTarget)
    {
        Debug.Log("Set target: " + tmpTarget.name);
        targetLocation = tmpTarget.position;
        action = Action.Chasing;
        Activate();
    }

    protected override void UnitGone() { }
    protected override void AtLocation() {}

    public override void TakeDamage(Vector3 hitPosition, float damageTaken)
    {
        health -= damage;
        action = Action.Idle;
    }
}
