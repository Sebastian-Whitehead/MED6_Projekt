using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triggeani : MonoBehaviour
{
    Animator anim;
    private CharacterController _controller;
    private float _speed = 5f;
    public AudioClip move;
    public AudioClip hitSound;
    public AudioClip death;
    AudioSource audioSource;
    bool isMoving = false;
    public int life;
 
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        anim = gameObject.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        
    }
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        if (Input.GetKey("d"))
        {
            anim.SetTrigger("Move");
        }

        if (Input.GetKeyDown("space"))
        {
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
            audioSource.Stop();
        }

        if (isPlaying(anim, "hit reaction girl"))        {
            audioSource.PlayOneShot(hitSound);
            Debug.Log("HIT");
        }
    }

    bool isPlaying(Animator anim, string stateName)
{
    if (anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        return true;
    else
        return false;
}
}
