using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using SharedDatastructures;

public class GameMode : MonoBehaviour
{

    public Gamemode gamemode;

    public BciSlider bciSlider;
    public Player player;

    private void Start()
    {
        bciSlider.gamemode = player.gamemode = gamemode;
    }
}
