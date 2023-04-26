using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Reset rotation
        Quaternion currentRotation = transform.rotation;
        currentRotation.y = 0;
        transform.rotation = currentRotation;
    }
}
