using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{

    public float speed = 5f;
    Transform _transform; 
    bool isMoving = false;
    float distanceThresh = 0.01f;

    // Start is called before the first frame update
    void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isMoving)
		{
			RaycastHit hit; 
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
			if(Physics.Raycast(ray, out hit, Mathf.Infinity))
			{
				if (hit.collider.tag == "ground")
				{
					StartCoroutine(GridMove(hit.point));
				}
			}
		}
        
    }


    IEnumerator GridMove(Vector3 destination){
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

    IEnumerator movingTo(Vector3 destination){
        while (true){
            _transform.position = Vector3.MoveTowards(_transform.position, destination, speed * Time.deltaTime);
            
           // Debug.Log(Vector3.Distance(_transform.position, dest));
            if (Vector3.Distance(_transform.position, destination) <= distanceThresh){
                isMoving = false;
                yield break;
            }  
            else
                yield return null;
            
        }
    }
}
