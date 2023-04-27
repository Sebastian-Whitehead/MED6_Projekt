using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblinani : MonoBehaviour
{

    Animator anim;
    //public Animator Girl;
    public int gLife = 5;
    public AudioClip impact;
    public AudioClip Death;
    AudioSource audioSource;
    private Shoot shoot;
    // private float _speed = 5f;
    public AudioClip move;
   // bool isMoving = false;
    private TacticsMove enemy;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        shoot = GetComponent<Shoot>();
        enemy= GetComponent<TacticsMove>();
    }

    // Update is called once per frame
    void Update()
    { 

        // Vi skal finde der hvor modstanderen bevæger sig i stedet for der hvor playeren gør.
        Vector3 velocity = enemy.velocity;
        bool moving = enemy.isMoving;

        if(moving == true && velocity.x > 0.1){
            anim.SetFloat("Speed", Mathf.Abs(velocity.x));
             } else if (moving == true && velocity.z > 0.1){
                anim.SetFloat("Speed", Mathf.Abs(velocity.z));
             } else if (moving == false) {
            anim.SetFloat("Speed", 0);
//            Debug.Log("not moving");
        }
       
       /* if (Input.GetKeyDown("q") && gLife >0)
        {
            shoot.shooting();
            anim.SetTrigger("Punch");
        }

        if (Input.GetKeyDown("e"))
        {
            anim.SetTrigger("Run");
        } */
    }
     
    void OnCollisionEnter(Collision collision){
         if (collision.gameObject.tag == "Projectile")
            {
            //If the GameObject has the same tag as specified, output this message in the console
            gLife = gLife - 2;
            if(gLife>0){
                anim.SetTrigger("Hit");
                //Debug.Log("Goblin is hit");
                audioSource.PlayOneShot(impact);

            }
            else{ 
                anim.SetTrigger("Death");
                audioSource.PlayOneShot(Death);
            }
            } 
        }
}
