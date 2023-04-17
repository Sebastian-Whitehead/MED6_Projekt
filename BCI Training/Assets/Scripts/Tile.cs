using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public bool current = false;
    public bool targetTile = false; //Target posiiton
    public bool selectable = false; //The clickable tiles
    public bool isWalkable = true; 

    public List<Tile> adjacentList = new List<Tile>();  //list for identifying the neighbors.
   
   //BFS variables
   public bool visisted = false; //The tile has been processed
   
   public Tile parentTile = null; //Know the parent tile. allow us to identify which tiles are walkable through the algorithm. 
   //MOve backwards from the parent to identify the path.
   public int distance = 0; //How far each tile is from start tile.

    //EnemyMove variables 
    public float f = 0; //g+heuristic cost
    public float gCost = 0; //Cost from parent to current tile, som from the beginning
    public float heuristicCost = 0; //From processed tile to destination - estimated


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        if (current){
            GetComponent<Renderer>().material.color = Color.yellow;
        }
        else if (targetTile){
            GetComponent<Renderer>().material.color = Color.blue;
        }
        else if (selectable){
            GetComponent<Renderer>().material.color = Color.red;
        }
        else{
            GetComponent<Renderer>().material.color = Color.white;
        }
    }

    public void Reset(){ //Reset tile to original state
        adjacentList.Clear();

        current = false;
        targetTile = false; //Target posiiton
        selectable = false; //The clickable tiles
        
        visisted = false;
        parentTile = null;
        distance = 0;

        f = gCost = heuristicCost = 0;
    }

    public void IdentifyNeighbors(Tile target){
        Reset();
        
        CheckTile(Vector3.forward, target);
        CheckTile(-Vector3.forward, target);
        CheckTile(Vector3.right, target);
        CheckTile(-Vector3.right, target);

    }

    public void CheckTile(Vector3 direction, Tile target){ //Check tile forward, back, left, right, is it traversable?
        Vector3 halfExtents = new Vector3(0.25f, 1/ 2.0f, 0.25f); //Check to see if there is a tile there which is reachable.
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents); //overlapbox returns a list of colliders that is traversable.

        foreach (Collider item in colliders){ 
            Tile tile = item.GetComponent<Tile>(); //Determine if it is a tile, if it is not a tile or not walkable, meaning not traversable, we ignore it.
            if (tile != null && tile.isWalkable){
                RaycastHit hit;
                 //Test if there is something on top of the tile, making it non-walkable...
                 //We add it to the list if the raycast DOES NOT hit something.
                if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1) || (tile == target)){ //include target even if something is sitting on the tile.
                    
                    adjacentList.Add(tile);
                    //currentTile.parent = 


                }
                 
            }
        }
    
    }
}
