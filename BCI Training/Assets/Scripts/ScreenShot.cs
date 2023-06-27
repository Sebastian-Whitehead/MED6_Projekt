using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ScreenShot : MonoBehaviour
{
    [SerializeField]
    private string screenName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Press W to take a Screen Capture
        if (Input.GetKeyDown(KeyCode.W))
        {
            ScreenCapture.CaptureScreenshot(Path.Combine(@"C:\Users\chilo\OneDrive\Billeder", screenName + ".png"), 3);
            Debug.Log("Screenshot Captured");
        }
    }
}