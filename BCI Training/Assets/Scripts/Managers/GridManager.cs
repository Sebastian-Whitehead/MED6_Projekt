using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
 [SerializeField] private int width;
 [SerializeField] private int height;
 [SerializeField] private Tiles tileRef;

 private Dictionary<Vector3, Tiles> tiles;

void Start(){
    MakeGrid();
}
 void MakeGrid(){
    tiles = new Dictionary<Vector3, Tiles>();
    for(int x = 0; x < width; x++){//called x/z easier to remember lol
        for(int z = 0; z < height; z++){
            var spawnTile = Instantiate(tileRef, new Vector3(x,0,z), Quaternion.identity);
            spawnTile.name = $"Tile {x},{z}";
        
        var isEven = (x % 2 == 0 && z % 2 != 0) || (x % 2 != 0 && z % 2 == 0);
        spawnTile.Init(isEven);
        
        tiles[new Vector3(x,0,z)] = spawnTile;
        }
    } 
 }
    public Tiles GetTilePos(Vector3 pos){
        if (tiles.TryGetValue(pos, out var tile)){
            return tile;
           
        }        
        return null;  
 }
}
