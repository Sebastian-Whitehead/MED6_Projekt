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
    public float jumpHeight = 2; //drop down and jump 2 tiles
    public float moveSpeed = 2;
    public int steps = 0;
    float halfHeight = 0; 
    public bool chasing = false;
    
    Vector3 velocity = new Vector3();
    Vector3 direction = new Vector3(); //heading

    GameObject target;


    public Tile AStarTargetTile;

    protected void Init(){
        tiles = GameObject.FindGameObjectsWithTag("Tile"); //all tiles in 1 array, do this every frame if we add and remove tiles on the go. while playing.
        halfHeight = GetComponent<Collider>().bounds.extents.y;//Gives distance from tile to center of the player. Calculate where player is on the tile.
    }

    public void GetcurrentTile(){ //Find the tile currently under the player.
        currentTile = GetTargetTile(gameObject); //Target tile for the player.
        currentTile.current = true; //Change color from the Tile Script current variable.
    }

    public Tile GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        Tile savedTile = null;
        if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1)){ //Locate the tile
            savedTile = hit.collider.GetComponent<Tile>();
        }
        return savedTile;
    }

    public void ComputeAdjency(Tile target){ //go thorugh each tile
        foreach(GameObject tile in tiles){
            Tile t = tile.GetComponent<Tile>();
            t.IdentifyNeighbors(target);
        }
    }

    public void BFS() {
        ComputeAdjency(null);
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

            if (t.distance < moveRange){
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

    public void MoveTo(Tile tile) {
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
        
        // Debug.Log(name + " moving");
        // Debug.Log(name + ", " + path.Count);
        if (!isMoving) return;
        if (steps <= 0) {
            RemoveSelectableTiles();
            isMoving = false;
        }

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
                Tile temp = path.Pop(); // remove that tile of the path, because we have reached it. 
                //Eventually we have popped all the tiles and reached the goal.

                if (path.Count > 0) {
                    steps--;
                    // Debug.Log("Steps: " + steps + ", Path: " + path.Count);
                }
                
            }
        } else {
            //RemoveSelectableTiles();
            //isMoving = false;
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

protected Tile FindLastTile(Tile t){ //tile in front of the one we look for and calc path at max move distance
        Stack<Tile> TempPath = new Stack<Tile>();
        
        RaycastHit hit;
        bool ocupied = true;
        if (!Physics.Raycast(t.transform.position, Vector3.up, out hit, 2)) {
            TempPath.Push(t);
            ocupied = false;
        }

        Tile next = t.parentTile;
        while (next != null){ //Path from the tile next to the target back to start
            TempPath.Push(next);
            next = next.parentTile;
        }

        if (TempPath.Count <= moveRange) {
            if (ocupied || t.parentTile == null) return t.parentTile;
            else return t;
        }

        int tmp = moveRange;
        Tile lastTile = null;
        for (int i = 0; i <= moveRange; i++){
            lastTile = TempPath.Pop(); //pop each tile for number of moves
            //when we pop the last one, we move to that tile
            chasing = false; 
        }

        // Debug.Log(lastTile + ", " + t);

        return lastTile;
    }

    protected Tile LowestF(List <Tile> list){ //find greatest potential of getting to where we are going.
        Tile lowest = list[0];

        foreach(Tile t in list){ //looking for the lowest f cost.
            if (t.f < lowest.f){
               lowest = t; 
            }
        }

        list.Remove(lowest);
        return lowest;

    }
    protected void FindPath(Tile target){ //enemy astar
        ComputeAdjency(target);
        GetcurrentTile();
        //Two lists, open and closed list.

        List<Tile> openList = new List<Tile>(); //any tile that has not been processed
        List<Tile> closedList = new List<Tile>(); //every tile that has been processed, not done until the target tile is in this list.
    
        openList.Add(currentTile);
        //leave the start null, to quickly find it 
        currentTile.heuristicCost = Vector3.Distance(currentTile.transform.position, target.transform.position);
        currentTile.f = currentTile.heuristicCost;

        while (openList.Count > 0){ //loop the open list. if we hit 0, without getting to the tile, we have no path
            Tile t = LowestF(openList); 

            closedList.Add(t); //add to closed list, we have found the closest route to this t.

            if (t == target){ //cannot step on target, because there is a unit, stop algorithm 1 node before end
                AStarTargetTile = FindLastTile(t);
                MoveTo(AStarTargetTile);
                return;
            }

            foreach (Tile tile in t.adjacentList){ //process all the neighbors, assumming we are not at the target tile
                //3 cases for a node, the tile is in the closed list, open list or not in any lists.
                if(closedList.Contains(tile)){
                    //do nothing

                }
                else if (openList.Contains(tile)){ //found way to a tile already in the list, meaning we found 2 or more routes to the same tile. check which is faster from prev parent
                    //compare g scores
                    float temporaryG = t.gCost + Vector3.Distance(tile.transform.position, t.transform.position);

                    if (temporaryG < tile.gCost){ //if this is the case, then it is faster
                        tile.parentTile = t;
                        tile.gCost = temporaryG;
                        tile.f = tile.gCost + tile.heuristicCost;
                    }    
                }

                else{ //add the new node, calculate cost
                    tile.parentTile = t;
                    
                    tile.gCost = t.gCost + Vector3.Distance(tile.transform.position, t.transform.position);
                    tile.heuristicCost = Vector3.Distance(tile.transform.position, target.transform.position);
                    tile.f = tile.gCost + tile.heuristicCost;

                    openList.Add(tile);
                }
            }

        }
        //problem - what to do if there is no path.
        Debug.Log("Path not found");
    }
    protected void CalculatePath(){ //Find where it is going to move to
        Tile targetTile = GetTargetTile(target);
        FindPath(targetTile);

    }
    protected void CalculatePath(Tile targetTile){ //Find where it is going to move to
        FindPath(targetTile);
    }

    protected void FindPlayer(){
        GameObject playerTarget = GameObject.FindGameObjectWithTag("Player");
        target = playerTarget;
    }


}
