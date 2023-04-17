using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Unit {

    public State state = State.Idle;

    public enum State {
        Idle,
        Attack,
        Move,
        Charge
    }

    private Resources res;

    public Button ConfirmBtn;
    
    protected override void ChildAwake() {
        ConfirmBtn.onClick.AddListener(ConfirmAction);
        res = GetComponent<Resources>();
    }

    protected override void ChildUpdate() {
        res.Alive(audioManager);
        CheckMouseClick();
    }

    void CheckMouseClick() {

        if (!active) return;
        if (!isMoving) BFS();
        if (!Input.GetMouseButtonDown(0)) return;
        
        if (turnManager.turn != TurnManager.Turn.Player) return;
        target = null;
        excecute = false;
        offensive = false;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity)) return;
        SetMoveTarget(hit.collider);
        SetAttackTarget(hit.collider);
        ActivateBtn();
    }

    void SetMoveTarget(Collider collider) {
        if (collider.tag == "Tile") {
            state = State.Move;
            Tile t = collider.GetComponent<Tile>();
            if (t.selectable) MoveTo(t);
        }
    }

    void SetAttackTarget(Collider collider) {
        if (collider.tag != targetTag) return;
        Debug.Log("Set target: " + collider.name);
        targetLocation = collider.transform.position;
        action = Action.Chasing;
        transform.LookAt(targetLocation, Vector3.up);
        offensive = true;
    }

    private void ActivateBtn() {
        if (state == State.Idle) return;
        Debug.Log("Activate Confirm Btn");
        ConfirmBtn.interactable = true; 
    }

    private void ConfirmAction() {
        if (state == State.Idle) return;
        Debug.Log("Confirm action");
        excecute = true;
        ConfirmBtn.interactable = false;
        state = State.Idle;
        Deactivate();
    }

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
