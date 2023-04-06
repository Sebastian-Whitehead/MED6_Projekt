using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{

    public new Action action = Action.Patrole;

    [Header("Patrole")]
    private int patrolPoint = 1;
    private Vector3[] patrolPoints;
    public bool circlePatrole = false;
    private bool clockwise = true;

    public new enum Action
    {
        Idle,
        Patrole,
        ChaseTarget,
        ScoutArea,
        LookAround,
        Investegate,
    }

    // ---------------------------------------------------------------------

    public override void TakeDamage(Vector3 hitPosition, float damageTaken)
    {
        health -= damage;
        Investegate(hitPosition);
        transform.LookAt(hitPosition, Vector3.up);
    }

    // ---------------------------------------------------------------------

    protected override void ChildAwake()
    {
        //patrolPoints = GenerateRandomPath(5);
        patrolPoints = GetManualPath();
    }

    protected override void ChildUpdate()
    {
        DrawPatrole();
    }

    public void ActionSelect()
    {
        Idle();
        ChaseTarget();
        Patroling();
        ScoutArea();
        LookAround();
        execute = true;
    }

    // ---------------------------------------------------------------------


    // https://answers.unity.com/questions/475066/how-to-get-a-random-point-on-navmesh.html
    private Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        UnityEngine.AI.NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    private Vector3[] GenerateRandomPath(int pathLength)
    {
        Vector3[] path = new Vector3[pathLength];
        for (int i = 0; i < pathLength; i++)
        {
            Vector3 nextPoint = RandomNavmeshLocation(1000f);
            path[i] = nextPoint;
        }
        return path;
    }

    private Vector3[] GetManualPath()
    {
        Transform PathObject = null;
        foreach (Transform child in transform)
        {
            if (child.name != "Path") continue;
            PathObject = child.transform;
        }
        if (PathObject == null) Debug.Log("No 'Path' found");
        Transform[] points = PathObject.GetComponentsInChildren<Transform>();
        Vector3[] path = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            Transform point = points[i];
            Vector3 position = point.transform.position;
            path[i] = position;
        }
        return path;
    }

    private void DrawPatrole()
    {

        viewColor = Color.green;
        moveColor = Color.blue;
        if (action == Action.ChaseTarget)
        {
            viewColor = Color.red;
            moveColor = Color.red;
        }

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            Vector3 startPoint = patrolPoints[i] + new Vector3(0, 1, 0);
            Vector3 endPoint;
            if (i >= patrolPoints.Length - 1) endPoint = patrolPoints[0] + new Vector3(0, 1, 0);
            else endPoint = patrolPoints[i + 1] + new Vector3(0, 1, 0);
            Vector3 difference = endPoint - startPoint;
            Debug.DrawRay(startPoint, difference, new Color(0.2F, 0.3F, 0.4F));
        }
    }

    protected override void UnitGone()
    {
        action = Action.ScoutArea;
    }

    // ---------------------------------------------------------------------

    private void Patroling()
    {
        if (action != Action.Patrole) return;
        targetLocation = patrolPoints[patrolPoint];
        if (circlePatrole)
        {
            if (clockwise)
            {
                if (++patrolPoint >= patrolPoints.Length - 1) clockwise = false;
            }
            else
            {
                if (--patrolPoint <= 0) clockwise = true;
            }
        }
        else
        {
            if (++patrolPoint >= patrolPoints.Length) patrolPoint = 0;
        }
    }

    private void Investegate(Vector3 position)
    {
        action = Action.Investegate;
        targetLocation = position;
    }

    private void ScoutArea()
    {
        if (action != Action.ScoutArea) return;
        targetLocation = RandomNavmeshLocation(moveDistance);
    }

    private void LookAround()
    {
        if (action != Action.LookAround) return;
        targetLocation = transform.position;
    }
}