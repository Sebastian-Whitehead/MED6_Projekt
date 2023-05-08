using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : TacticsMove {

    public LayerMask ignore;

    // Start is called before the first frame update
    void Start() {
        Init();
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
        if (!Physics.Raycast(position, -Vector3.up, out hit, 2)) {
            Debug.Log(name);
            Debug.Log(position);
            Debug.Log("Tile not found");
            return null;
        }
        if (hit.collider.tag != "Tile") {
            Debug.Log("Not tile!");
            Debug.Log(hit.collider.name);
            Debug.Log(hit.collider.transform.position);
            if (CompareTag("Enemy")) return null;
        }
        hit.collider.GetComponent<Renderer>().material.color = Color.red;
        return hit.collider.GetComponent<Tile>();
    }
}
