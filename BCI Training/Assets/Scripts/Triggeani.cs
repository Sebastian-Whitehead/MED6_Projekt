using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triggeani : MonoBehaviour
{
    Animator anim;
    private CharacterController _controller;
    private float _speed = 5f;
    public AudioClip move;
    public AudioClip impact;
    public AudioClip death;
    AudioSource audioSource;
    AudioSource hitSource;
    bool isMoving = false;
    public int life;
    private Shoot shoot;
 
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        anim = gameObject.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        shoot = GetComponent<Shoot>();
        
        
    }
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        if (Input.GetKey("d"))
        {
            anim.SetTrigger("Move");
        }

        if (Input.GetKeyDown("space") && life >0)
        {
            shoot.shooting();
            anim.SetTrigger("Shoot");
        }

       

        

        if (isMoving) {
            while (audioSource.isPlaying == false){
            audioSource.PlayOneShot(move);
        }

        }

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        Vector3 direction = new Vector3(horizontalInput, 0, 0);
        //_direction = new Vector3(0, 0, horizontalInput) * _speed;
        Vector3 velocity = direction * _speed;

        //_controller.Move(velocity * Time.deltaTime);
        anim.SetFloat("Speed", Mathf.Abs(horizontalInput));
        if (velocity.x != 0){
            isMoving = true;
        }
        else {
            isMoving = false;
            //audioSource.Stop();
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
