using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    private Vector3 offset;

    private void Start()
    {
        offset = new Vector3(transform.position.x - target.position.x, 0, transform.position.z - target.position.z);
        // print(offset.x + "//" + offset.z);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(target.position.x + offset.x, transform.position.y + offset.y, target.position.z + offset.z); 
    }
}

/*

Låser cam til en fast stilling efter targets placering og flytter kamera I forhold til target. 
I det her tilfælde er det playeren. 
I stedet for at gøre det til et child af det. 

*/
