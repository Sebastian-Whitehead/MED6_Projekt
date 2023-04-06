using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblinani : MonoBehaviour
{

    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            anim.SetTrigger("Hit");
        }

        if (Input.GetKeyDown("e"))
        {
            anim.SetTrigger("Death");
        }
    }
}
