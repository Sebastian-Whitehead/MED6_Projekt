using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : TacticsMove {

    // Start is called before the first frame update
    void Start() {
        Init();
    }

    void Update(){
       if (!turn){
        return;
       }
    }
    protected void MoveToGrid(Collider collider) {
        if (collider.tag == "Tile") {
            Tile t = collider.GetComponent<Tile>();
            if (t.selectable){
                MoveTo(t);
            }
        }
    }

    

    protected Tile GetTileAtPosition(Vector3 position) {
        RaycastHit hit;
        if (!Physics.Raycast(position, -Vector3.up, out hit, 1)) return null;
        return hit.collider.GetComponent<Tile>();
    }
}
