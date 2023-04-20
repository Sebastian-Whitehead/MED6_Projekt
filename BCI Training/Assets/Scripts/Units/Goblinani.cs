using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblinani : MonoBehaviour
{

    Animator anim;
    public Animator Girl;
    public int gLife = 4;
    public AudioClip impact;
    public AudioClip Death;
    AudioSource audioSource;
    private Shoot shoot;
     private float _speed = 5f;
    public AudioClip move;
    bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        shoot = GetComponent<Shoot>();
    }

    // Update is called once per frame
    void Update()
    {

       
        if (Input.GetKeyDown("q") && gLife >0)
        {
            shoot.shooting();
            anim.SetTrigger("Punch");
        }

        if (Input.GetKeyDown("e"))
        {
            anim.SetTrigger("Run");
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
                audioSource.PlayOneShot(impact);

            }
            else{ 
                anim.SetTrigger("Death");
                audioSource.PlayOneShot(Death);
            }
            } 
        }
}
