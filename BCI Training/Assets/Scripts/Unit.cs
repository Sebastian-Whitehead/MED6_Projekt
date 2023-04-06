using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{

    protected TurnManager turnManager;

    public enum Action { Idle, ChaseTarget };
    protected Action action;

    [Header("Character")]
    public float maxHealth = 10f;
    public float attackRange = 1f;
    public float damage = 2f;
    public float health;
    public bool alive = true;

    [Header("View")]
    [Range(2, 15)] public int inc = 5;
    [Range(1, 180)] public int FOV = 25;
    [Range(0f, 10f)] public float distance = 10f;
    public string targetName;
    protected Vector3 targetLocation;
    protected Unit target;

    [Header("Movement")]
    public int moveSpeed = 2;
    public int moveDistance = 2;

    [Header("Debug")]
    protected Color viewColor, moveColor;

    [Header("Manager")]
    public bool active = false;
    protected bool execute = false;

    public abstract void TakeDamage(Vector3 hitPosition, float damageTaken);
    protected abstract void UnitGone();
    protected abstract void ChildAwake();
    protected abstract void ChildUpdate();

    void Awake()
    {
        turnManager = GameObject.Find("GameManager").GetComponent<TurnManager>();
        target = GameObject.FindGameObjectWithTag(targetName).GetComponent<Unit>();
        health = maxHealth;
        ChildAwake();
    }

    public void Update()
    {
        Alive();
        Eyes();
        ChildUpdate();
        ExecuteAction();
    }

    private void Alive()
    {
        if (health > 0) return;
        alive = false;
    }

    protected void Eyes()
    {
        inc = Mathf.Max(2, inc);
        RaycastHit hit;

        for (int angle = -FOV; angle <= FOV; angle += inc)
        {
            Vector3 targetPos = new Vector3(0, 0, 0);
            targetPos += Quaternion.AngleAxis(angle, Vector3.up) * transform.forward * distance;

            Debug.DrawRay(transform.position, targetPos, viewColor);

            if (Physics.Raycast(transform.position, targetPos, out hit, distance))
            {
                if (hit.transform.tag == targetName)
                {
                    action = Action.ChaseTarget;
                    target = hit.transform.GetComponent<Unit>();
                    return;
                }
            }
        }
        if (action == Action.ChaseTarget)
        {
            UnitGone();
        }
    }

    protected void ExecuteAction()
    {
        if (!execute) return;
        MoveToLocation();
        AttackTarget();
    }

    protected void MoveToLocation()
    {
        float distanceToLocation = Vector3.Distance(transform.position, targetLocation);
        if (distanceToLocation < 0.01f) EndExecute();

        transform.LookAt(targetLocation, Vector3.up);

        float step = moveSpeed * Time.deltaTime;
        Debug.DrawLine(transform.position, targetLocation, moveColor);
        targetLocation.y = transform.position.y;
        transform.position = Vector3.MoveTowards(transform.position, targetLocation, step);
    }

    protected void Idle()
    {
        if (action != Action.Idle) return;
        targetLocation = transform.position;
    }

    protected void ChaseTarget()
    {
        if (action != Action.ChaseTarget) return;
        targetLocation = target.transform.position;
    }

    private void AttackTarget()
    {
        if (target == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, target.transform.position);
        if (distanceToPlayer > attackRange) return;

        Unit unit = target.GetComponentInParent<Unit>();
        unit.TakeDamage(transform.position, damage);
        Debug.Log("Attack!");
        EndExecute();
    }

    // ---------------------------------------------------------------------

    public void StartTurn()
    {
        active = true;
    }

    public void StartExecute()
    {
        execute = true;
        turnManager.Wait();
    }

    public void EndExecute()
    {
        execute = false;
    }

    public void EndTurn()
    {
        turnManager.EndTurn();
    }

    public void SetAction(Action nextAction)
    {
        action = nextAction;
    }

    public bool Executing()
    {
        return execute;
    }

    public bool Active()
    {
        return active;
    }
}