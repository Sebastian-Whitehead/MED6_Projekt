using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.UI;



public class Highlight : MonoBehaviour
{
    public Image highlightImage;
    public bool selected;
    private EnemyHealth enemyHealth;

    // Start is called before the first frame update
    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        selected = false;
        highlightImage.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!enemyHealth.alive) highlightImage.enabled = false;
        else highlightImage.enabled = selected;
    }
}
