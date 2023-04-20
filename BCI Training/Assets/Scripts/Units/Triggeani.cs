using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triggeani : MonoBehaviour
{
    Animator anim;
    private CharacterController _controller;
   // private float _speed = 5f;
    public AudioClip move;
    public AudioClip impact;
    public AudioClip death;
    AudioSource audioSource;
    AudioSource hitSource;
    bool isMoving = false;
    public int life;
    private Shoot shoot;
    private TacticsMove player;
 
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        anim = gameObject.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        shoot = GetComponent<Shoot>();
        player= GetComponent<TacticsMove>();
        
        
    }
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        if (Input.GetKey("d"))
        {
        }

        if (Input.GetKeyDown("space") && life >0)
        {
            shoot.shooting();
            anim.SetTrigger("Shoot");
        }
        bool moving = player.isMoving;
       

        

        if (moving) {
            while (audioSource.isPlaying == false){
            audioSource.PlayOneShot(move);
            anim.SetTrigger("Move");
        }

        }


        //_direction = new Vector3(0, 0, horizontalInput) * _speed;
        Vector3 velocity = player.velocity;

        //_controller.Move(velocity * Time.deltaTime);
        
       /* if (velocity.x != 0){
            isMoving = true;
        }
        else {
            isMoving = false;
            //audioSource.Stop();
        }*/
        if(moving == true && velocity.x > 0.1){
            anim.SetFloat("Speed", Mathf.Abs(velocity.x));
             } else if (moving == true && velocity.z > 0.1){
                anim.SetFloat("Speed", Mathf.Abs(velocity.z));
             } else if (moving == false) {
            
            anim.SetFloat("Speed", 0);
        }
    }

 /*   bool isPlaying(Animator anim, string stateName)
{
    if (anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        return true;
    else
        return false;
}*/

void OnCollisionEnter(Collision collision){
     if (collision.gameObject.tag == "Projectile")
        {
            life = life-1;
            if (life > 0){
                anim.SetTrigger("Hit");
                while (audioSource.isPlaying == false){
                audioSource.PlayOneShot(impact);
                }
                Debug.Log("Play hitsound");

            }
            else {
                anim.SetTrigger("Death");
                audioSource.PlayOneShot(death);
            }
        }
}


}
