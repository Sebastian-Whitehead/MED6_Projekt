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
    //float halfHeight = 0; 
    public Vector3 velocity = new Vector3();
    Vector3 direction = new Vector3(); //heading
    GameObject target;
    public Tile AStarTargetTile;
    private bool isRotated = false;
    public float turnSpeed = 1f;

    protected void Init(){
        tiles = GameObject.FindGameObjectsWithTag("Tile"); //all tiles in 1 array, do this every frame if we add and remove tiles on the go. while playing.
        //halfHeight = GetComponent<Collider>().bounds.extents.y / 2; //Gives distance from tile to center of the player. Calculate where player is on the tile.
    }
    //find alle game objects med Tile tag i scenen.

    public void GetcurrentTile(){ //Find the tile currently under the player.
        currentTile = GetTargetTile(gameObject); //Target tile for the player.
        currentTile.current = true; //Change color from the Tile Script current variable.
    }
    /*
    The GetcurrentTile method determines the current tile
    by casting a ray from the unit's position downwards and checking for a collision with a tile. 
    The current tile is then marked as "current" using a variable in the Tile script.
    */

    public Tile GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        Tile savedTile = null;
        if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1)){ //Locate the tile
            savedTile = hit.collider.GetComponent<Tile>();
        }
        // Debug.Log("(" + name + ") savedTile: " + savedTile);
        return savedTile;
    }
    /*
    method used to get the Tile component of a given target game object by casting a ray downwards
     from its position and checking for a collision with a tile.
    */

    public void ComputeAdjency(Tile target){ //go thorugh each tile
        foreach(GameObject tile in tiles){
            Tile t = tile.GetComponent<Tile>();
            t.IdentifyNeighbors(target);
        }
    }
    /*
     iterates through all tiles and calls the IdentifyNeighbors method on each tile, 
     passing the target tile as a parameter. 
     This method is responsible for identifying the neighboring tiles for each tile.
    */

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
        // Debug.Log(name);
        if (tile == null) {
            Debug.Log("No path found");
            return;
        }
        tile.targetTile = true;
        Tile endLocation = tile; //target tile end location
        while (endLocation != null) { //when end = null, then we are at the starting tile.
            path.Push(endLocation);
            endLocation = endLocation.parentTile;
        }
    }
    /*
     It clears the path stack, sets isMoving to true, 
     and sets the target tile as the Tile parameter passed to the method. 
     It also marks the target tile as the target for visualization purposes.
    */

    public void Move() { //move from one tile to the next. - each step in the path is a tile. 
        
        // Debug.Log(name + " moving");
        // Debug.Log(name + ", " + path.Count);
        if (!isMoving) return;
        if (steps <= 0) {
            RemoveSelectableTiles();
            isMoving = false;
            isRotated = false;
        }

        if (path.Count > 0) {
            Tile t = path.Peek(); //look at the stack, dont remove anything till we reach it.
            Vector3 targetTile = t.transform.position;

            //Calcaulating the players position on top of the target tile.
            //We dont wanna walk into the tile, because then we will go into the ground, therefore we add halfheight and the tile height.. 
            //targetTile.y += t.GetComponent<Collider>().bounds.extents.y + 0.01f;

            // Check distance to target (not on y-axis upwards)
            Vector3 position = transform.position;
            targetTile.y = position.y;
            float dist = Vector3.Distance(position, targetTile);
            //Debug.Log(name + " distance: " + dist);
            if (dist >= 0.1f){

                CalculateDirection(targetTile);
                SetHorizontVelocity();

                if (!isRotated) {

                    // Rotate towards the target
                    float singleStep = turnSpeed * Time.deltaTime; // Rotation step speed
                    Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction, singleStep, 0.0f); // Rotation vector
                    transform.rotation = Quaternion.LookRotation(newDirection); // Rotate rotation vector

                    // Check if unit is looking towards target tile
                    Quaternion targetDirection = Quaternion.LookRotation(direction, Vector3.up); // Rotation towards tile
                    float angle = Quaternion.Angle(transform.rotation, targetDirection); // Angle from unit and target direction
                    
                    if (angle < 1f) isRotated = true;

                } else {
                    // Move towards target tile
                    //transform.forward = direction;
                    transform.position += velocity * Time.deltaTime;
                }


            } else {

                //Tile mid is reached
                //transform.position = targetTile;
                Tile temp = path.Pop(); // remove that tile of the path, because we have reached it. 
                //Eventually we have popped all the tiles and reached the goal.
                isRotated = false;
                velocity = Vector3.zero; 

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

    /*
    It checks if the unit is currently moving and if it has any steps remaining.
    If there are no steps remaining, it removes the selectable tiles and stops the movement.
    if there are steps remaining, it checks if there are tiles in the path stack.
    If there are, it retrieves the next tile from the stack and calculates the direction and velocity needed to move towards that tile. 
    It also handles rotation towards the target tile. If the unit has reached the target tile, it pops the tile from the stack and decreases the steps count.
    */

    protected void RemoveSelectableTiles(){ //remove selectable tiles. no longer active. Reset them. Each of the tiles that has been selected as moveable will no longer be selected
        if (currentTile != null){
            currentTile.current = false; 
            currentTile = null;
        }
        
        foreach(Tile tile in selectableTiles){ 
            tile.ResetTile();
        }
        selectableTiles.Clear();
    }
    /*
     method is responsible for resetting the selectable tiles and the current tile. 
     It iterates through the selectableTiles list and calls the ResetTile method on each tile, clearing the selection. 
     It also sets the currentTile variable to null.
    */
    public void CalculateDirection(Vector3 target){
        direction = target - transform.position; //The direction you are travelling
        direction.Normalize(); 
    }
    /*
    method calculates the direction vector needed to move towards a target position. 
    It subtracts the target position from the unit's current position and normalizes the resulting vector.
    */

    public void SetHorizontVelocity(){
        velocity = direction * moveSpeed; //define velocity vector.
    }

protected Tile FindLastTile(Tile t){ //tile in front of the one we look for and calc path at max move distance
        Stack<Tile> TempPath = new Stack<Tile>();
        
        RaycastHit hit;
        bool ocupied = true;
        if (!Physics.Raycast(t.transform.position, Vector3.up, out hit, 2) || 
        !hit.collider.GetComponent<Unit>().Alive()) {
            TempPath.Push(t);
            ocupied = false;
        }

        Tile next = t.parentTile;
        while (next != null){ //Path from the tile next to the target back to start
            TempPath.Push(next);
            next = next.parentTile;
        }

        // Debug.Log("t.parentTile: " + t.parentTile);
        // Debug.Log("t: " + t);

        if (TempPath.Count <= moveRange) {
            if (t.parentTile == null && t == null) {}
            if (ocupied) return t.parentTile;
            else return t;
        }

        int tmp = moveRange;
        Tile lastTile = null;
        for (int i = 0; i <= moveRange; i++){
            lastTile = TempPath.Pop(); //pop each tile for number of moves
            //when we pop the last one, we move to that tile
        }


        return lastTile;
    }
    /*
    This method is used to find the last tile in a path, given a target tile. 
    It starts by initializing a stack called TempPath to store the tiles in the path
    Then, it checks if the tile t is unoccupied by performing a raycast from its position upwards
    f the raycast doesn't hit any colliders or if the collider's associated unit is not alive, 
    it pushes t onto the stack. 
    Next, it iterates through the parent tiles of t until it reaches the starting tile, pushing each tile onto TempPath.
    Finally, it pops the last moveRange number of tiles from TempPath and returns the last tile as the destination tile for the enemy.
    */
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
    /*
    This method finds the tile with the lowest f cost from a given list of tiles. 
    It initializes lowest with the first tile from the list and compares the f values of each tile in the list. 
    The tile with the lowest f cost is assigned to lowest. 
    Afterward, it removes the tile from the list and returns lowest.
    */

    protected bool FindPath(Tile target){ //enemy astar
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
                return true;
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
        return false;
    }
    protected bool CalculatePath(){ //Find where it is going to move to
        Tile targetTile = GetTargetTile(target);
        return FindPath(targetTile);

    }
    /*
    This method is used to find a path to the target game object (presumably the player)
     by calling FindPath with the target tile obtained from the GetTargetTile(target) method.
    */
    protected bool CalculatePath(Tile targetTile){ //Find where it is going to move to
        return FindPath(targetTile);
    }

    protected void FindPlayer(){
        GameObject playerTarget = GameObject.FindGameObjectWithTag("Player");
        target = playerTarget;
    }
    //This method is responsible for finding the player game object and assigning it to the target variable.


}
