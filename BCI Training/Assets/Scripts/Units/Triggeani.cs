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
    private PlayerFeatures playerfeatures;
    private float lastHealth;
    private float health;
    private bool dead = false;
 
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        anim = gameObject.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        shoot = GetComponent<Shoot>();
        player= GetComponent<TacticsMove>();
        playerfeatures = GetComponent<PlayerFeatures>();
        lastHealth = playerfeatures.maxHealth;
        
        
    }
    void Update()
    {   
        health = playerfeatures.health;
        //if (Input.GetMouseButtonDown(0))
        bool moving = player.isMoving;
       

        

      /*  if (moving) {
            while (audioSource.isPlaying == false){
            audioSource.PlayOneShot(move);}

        }*/
        if (lastHealth > health && health > 0){
            anim.SetTrigger("Hit");
            while (audioSource.isPlaying == false){
                audioSource.PlayOneShot(impact);
                }
            lastHealth = health;
        }

        if (health == 0 && dead == false){
            anim.SetTrigger("Death");
            while (audioSource.isPlaying == false){
                audioSource.PlayOneShot(death);
                dead = true;
                }
            
        }

        Vector3 velocity = player.velocity;
        if(moving == true && Mathf.Abs(velocity.x) > 0.1){
            anim.SetFloat("Speed", Mathf.Abs(velocity.x));
            Debug.Log(velocity.x);
            while (audioSource.isPlaying == false){
                audioSource.PlayOneShot(move);}
        } else if (moving == true && Mathf.Abs(velocity.z) > 0.1){
            anim.SetFloat("Speed", Mathf.Abs(velocity.z));
            Debug.Log(velocity.z);
            while (audioSource.isPlaying == false){
                audioSource.PlayOneShot(move);}
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

}
