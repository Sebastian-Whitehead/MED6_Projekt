using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblinani : MonoBehaviour
{

    Animator anim;
    public Animator Girl;
    public int gLife = 4;
    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

       
        if (Input.GetKeyDown("q"))
        {
            anim.SetTrigger("Punch");
        }

        if (Input.GetKeyDown("e"))
        {
            anim.SetTrigger("Death");
        }
    }
     
    void OnCollisionEnter(Collision collision){
         if (collision.gameObject.tag == "Projectile")
            {
            //If the GameObject has the same tag as specified, output this message in the console
            gLife = gLife - 2;
            if(gLife>0){
                 anim.SetTrigger("Hit");
                Debug.Log("Goblin is hit");
            }
            else anim.SetTrigger("Death");
            } 
        }
}
