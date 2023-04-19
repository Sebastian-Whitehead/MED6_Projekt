using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public Rigidbody projectile;
    public Transform Spawnpoint;
    public float playerLife;
    public float projectileSpeed;
    public KeyCode UserKey;
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        /* playerLife = GameObject.Find("Player").GetComponent<Animator>().GetFloat("Life");
        if (Input.GetKeyDown(UserKey) && playerLife > 0){
           StartCoroutine(waiter());*/
         }




   

    public void shooting()
    { 
           StartCoroutine(waiter());
         }


    IEnumerator waiter()
 {
    
    //Wait for 4 seconds
    yield return new WaitForSeconds(1.4f);
       
    Rigidbody clone;
    clone = (Rigidbody)Instantiate(projectile, Spawnpoint.position, projectile.rotation);

    clone.velocity = Spawnpoint.TransformDirection(Vector3.forward*projectileSpeed);
        

 }
}
