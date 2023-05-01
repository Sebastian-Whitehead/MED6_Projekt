using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.UI;



public class Highlight : MonoBehaviour
{
    public Image highlightImage;
    public bool selected;

    // Start is called before the first frame update
    void Start()
    {
        selected = false;
        highlightImage.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        highlightImage.enabled = selected;
    }
}
