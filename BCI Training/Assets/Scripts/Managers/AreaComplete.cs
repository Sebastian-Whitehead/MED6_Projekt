using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaComplete : MonoBehaviour
{
    public bool completion = false;
    private void OnTriggerEnter(Collider other) {
        if (completion) return;
        if (other.tag != "Player") return;
        completion = true;
    }
}
