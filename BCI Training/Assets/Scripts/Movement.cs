using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    Transform _transform;
    bool isMoving = false;
    float distanceThresh = 0.01f;

    float moveSpeed = 2f;

    private void Awake() {
        _transform = GetComponent<Transform>();
    }

    public void Move(Vector3 target) {
        StartCoroutine(GridMove(target));
    }

    protected IEnumerator GridMove(Vector3 destination) {
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

    protected IEnumerator movingTo(Vector3 destination) {
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

    public bool IsMoving() {
        return isMoving;
    }

    internal IEnumerator GridMove(Vector3 point, int moveSpeed)
    {
        throw new NotImplementedException();
    }
}