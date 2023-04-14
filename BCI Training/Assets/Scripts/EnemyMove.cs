using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : TacticsMove
{

    GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {

        Debug.DrawRay(transform.position, transform.forward);
        if (turn){
            return;
        }

        if (!isMoving){
            FindPlayer();
            CalculatePath();
            BFS();
            AStarTargetTile.targetTile = true; 
        }
        else{
            Move();
        }
    }

    void CalculatePath(){ //Find where it is going to move to
        Tile targetTile = GetTargetTile(target);
        FindPath(targetTile);

    }

    void FindPlayer(){ //finds the nearest player object, delete this. just assign target to the player tag.
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player"); //Array change to single

        GameObject nearest = null;
        float distance = Mathf.Infinity;
        
        foreach (GameObject obj in targets){ //check which obj with player tag is closest (ret)
            float d = Vector3.Distance(transform.position, obj.transform.position);

            if (d < distance){
                distance  = d; 
                nearest = obj;
            }
        }
        
        target = nearest;
    }
}
