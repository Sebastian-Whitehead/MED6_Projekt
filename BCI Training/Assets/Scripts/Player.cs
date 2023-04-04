using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool startTurn = false;
    private bool executing = false;
    
    public void StartTurn() {
        startTurn = true;
    }

    public void endTurn() {
        executing = false;
    }

    public bool Executing() {
        return executing;
    }
}
