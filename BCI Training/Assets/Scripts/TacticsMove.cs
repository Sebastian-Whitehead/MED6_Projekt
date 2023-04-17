using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMove : MonoBehaviour {

    [Header("Movement")]
    List<Tile> selectableTiles = new List<Tile>(); //Set back to original tile point.
    GameObject[] tiles;
    protected Stack<Tile> path = new Stack<Tile>(); //Stack getting pushed in reversable order. 
    protected Tile currentTile; //tile player is standing on

    public bool isMoving = false;
    public int moveRange = 5; //move tiles pr turn
    public int maxMoveRange = 5; //move tiles pr turn
    public float jumpHeight = 2; //drop down and jump 2 tiles
    public float moveSpeed = 2;
    public int steps = 0;
    float halfHeight = 0; 
    
    Vector3 velocity = new Vector3();
    Vector3 direction = new Vector3(); //heading

    protected void Init(){
        tiles = GameObject.FindGameObjectsWithTag("Tile"); //all tiles in 1 array, do this every frame if we add and remove tiles on the go. while playing.
        halfHeight = GetComponent<Collider>().bounds.extents.y;//Gives distance from tile to center of the player. Calculate where player is on the tile.
    }

    public void GetcurrentTile(){ //Find the tile currently under the player.
        currentTile = GetTargetTile(gameObject); //Target tile for the player.
        currentTile.current = true; //Change color from the Tile Script current variable.
    }

    public Tile GetTargetTile(GameObject target){ 
        RaycastHit hit;
        Tile savedTile = null;
        if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1)){ //Locate the tile
            savedTile = hit.collider.GetComponent<Tile>();
        }
        return savedTile;
    }

    public void ComputeAdjency(){ //go thorugh each tile
        foreach(GameObject tile in tiles){
            Tile t = tile.GetComponent<Tile>();
            t.IdentifyNeighbors(jumpHeight);
        }
    }

    public void BFS() {

        ComputeAdjency();
        GetcurrentTile();

        Queue<Tile> BFSsearch = new Queue<Tile>();

        //add currenttile to queue
        BFSsearch.Enqueue(currentTile);
        currentTile.visisted = true; //not wanna come backt to this.
        //currentTile.parentTile = null; //Find it later when backtracking.

        while (BFSsearch.Count > 0){ //Continue until empty
            Tile t = BFSsearch.Dequeue(); //process one tile, pop off the front. 
            selectableTiles.Add(t);
            t.selectable = true;
            
            int moveAble = steps;
            if (tag == "Enemy") moveAble = 100;

            if (t.distance < moveAble){
                foreach(Tile tile in t.adjacentList){ //Anything adjacent to it, will set the original tile as parent.
                    if (!tile.visisted){
                        tile.parentTile = t;
                        tile.visisted = true;
                        tile.distance = 1 + t.distance; //can add 1 everytime in an arch away from the start tile
                        BFSsearch.Enqueue(tile); //add it to the queue.
                    }
                }
            }
        }
    }

    public void MoveTo(Tile tile){
        path.Clear();
        isMoving = true;
        tile.targetTile = true;
        Tile endLocation = tile; //target tile end location
        while (endLocation != null) { //when end = null, then we are at the starting tile.
            path.Push(endLocation);
            endLocation = endLocation.parentTile;
        }
    }

    public void Move() { //move from one tile to the next. - each step in the path is a tile. 
        
        if (!isMoving) return;
        if (steps < 0) {
            RemoveSelectableTiles();
            isMoving = false;
        }

        // Debug.Log(tag + ", " + path.Count);

        if (path.Count > 0) {
            Tile t = path.Peek(); //look at the stack, dont remove anything till we reach it.
            Vector3 targetTile = t.transform.position;

            //Calcaulating the players position on top of the target tile.
            //We dont wanna walk into the tile, because then we will go into the ground, therefore we add halfheight and the tile height.. 
            targetTile.y += halfHeight + t.GetComponent<Collider>().bounds.extents.y;

            float dist = Vector3.Distance(transform.position, targetTile);
            if (dist >= 0.05f){
                CalculateDirection(targetTile);
                SetHorizontVelocity();

                transform.forward = direction;
                transform.position += velocity * Time.deltaTime;
            } else {
                
                //Tile mid is reached
                transform.position = targetTile;
                path.Pop(); // remove that tile of the path, because we have reached it. 
                //Eventually we have popped all the tiles and reached the goal.

                if (path.Count > 0) {
                    steps--;
                    // Debug.Log("Steps: " + steps + ", Path: " + path.Count);
                }
                
            }
        } else {
            isMoving = false;
            //Debug.Log("ok");
        }
    }


    protected void RemoveSelectableTiles(){ //remove selectable tiles. no longer active. Reset them. Each of the tiles that has been selected as moveable will no longer be selected
        
        if (currentTile != null){
            currentTile.current = false; 
            currentTile = null;
        }
        
        foreach(Tile tile in selectableTiles){ 
            tile.Reset();
        }
        selectableTiles.Clear();
    }

    public void CalculateDirection(Vector3 target){
        direction = target - transform.position; //The direction you are travelling
        direction.Normalize(); 
    }

    public void SetHorizontVelocity(){
        velocity = direction * moveSpeed; //define velocity vector.
    }
}
