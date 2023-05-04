using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using SharedDatastructures;

public class GameMode : MonoBehaviour
{

    public Gamemode gamemode;

    public GameObject player;
    
    private BciSlider bciSlider;
    private Player playerScript;
    private PlayerFeatures playerFeatures;
    
    

    private void Awake()
    {
        bciSlider = player.GetComponent<BciSlider>();
        playerScript = player.GetComponent<Player>();
        playerFeatures = player.GetComponent<PlayerFeatures>();
        
        bciSlider.gamemode = playerScript.gamemode = playerFeatures.gamemode = gamemode;
    }
}
