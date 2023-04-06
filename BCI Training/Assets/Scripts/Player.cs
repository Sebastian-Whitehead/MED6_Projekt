using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{

    public new Action action = Action.Idle;

    public new enum Action
    {
        Idle,
        ChaseTarget
    }

    Transform _transform;
    bool isMoving = false;
    float distanceThresh = 0.01f;

    protected override void ChildAwake()
    {
        _transform = GetComponent<Transform>();
    }

    protected override void ChildUpdate()
    {
        CheckMouseClick();
    }

    void CheckMouseClick() {

        if (!Input.GetMouseButtonDown(0) || isMoving) return;
        if (turnManager.turn != TurnManager.Turn.Player)
        {
            Debug.Log(turnManager.turn + " turn");
            return;
        }
        target = null;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity)) return;
        if (hit.collider.tag == "ground") StartCoroutine(GridMove(hit.point));
        else if (hit.collider.tag == targetName) SetTarget(hit.collider.transform);
    }

    void SetTarget(Transform tmpTarget)
    {
        Debug.Log("Set target: " + tmpTarget.name);
        targetLocation = tmpTarget.position;
        action = Action.ChaseTarget;
        StartExecute();
    }

    protected override void UnitGone() { }

    IEnumerator GridMove(Vector3 destination)
    {
        isMoving = true;

        Vector3 movementVector = destination - _transform.position;
        Vector3 target;

        //Move on the x coordinate
        target = _transform.position + new Vector3(movementVector.x, 0, 0);
        yield return StartCoroutine(movingTo(target));

        //move z when done
        target = _transform.position + new Vector3(0, 0, movementVector.z);
        yield return StartCoroutine(movingTo(target));
    }

    IEnumerator movingTo(Vector3 destination)
    {
        while (true)
        {
            _transform.position = Vector3.MoveTowards(_transform.position, destination, moveSpeed * Time.deltaTime);

            // Debug.Log(Vector3.Distance(_transform.position, dest));
            if (Vector3.Distance(_transform.position, destination) <= distanceThresh)
            {
                isMoving = false;
                yield break;
            }
            else
                yield return null;

        }
    }

    public override void TakeDamage(Vector3 hitPosition, float damageTaken)
    {
        health -= damage;
        action = Action.Idle;
    }
}
