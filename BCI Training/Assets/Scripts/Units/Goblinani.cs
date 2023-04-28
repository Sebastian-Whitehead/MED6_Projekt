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
    private TacticsMove tacticsmove;
    private EnemyHealth goblin;
    private TurnManager turnManager;
    private Enemy enemy;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        shoot = GetComponent<Shoot>();
        tacticsmove= GetComponent<TacticsMove>();
        goblin = GetComponent<EnemyHealth>();
        turnManager = GameObject.Find("GameManager").GetComponent<TurnManager>();
        enemy = GetComponent<Enemy>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    { 

        // Vi skal finde der hvor modstanderen bevæger sig i stedet for der hvor playeren gør.
        Vector3 velocity = tacticsmove.velocity;
        bool moving = tacticsmove.isMoving;
        if(moving == true && velocity.x > 0.1){
            anim.SetFloat("Speed", Mathf.Abs(velocity.x));
        } else if (moving == true && velocity.z > 0.1){
                anim.SetFloat("Speed", Mathf.Abs(velocity.z));
        } else if (moving == false) {
            anim.SetFloat("Speed", 0);
//            Debug.Log("not moving");
        }
    }

       IEnumerator waiter()
    {
    
    //Wait for 4 seconds
    yield return new WaitForSeconds(1f);
    turnManager.waiting = true;
    turnManager.EndTurn();

    }
     
    void OnCollisionEnter(Collision collision){
         if (collision.gameObject.tag == "Projectile")
            {
            //If the GameObject has the same tag as specified, output this message in the console
            if(goblin.alive){
                anim.SetTrigger("Hit");
                Debug.Log("Goblin is hit");
                audioSource.PlayOneShot(impact);
                Destroy(collision.gameObject);
                enemy.TakeDamage(player.transform.position, 2);
                StartCoroutine(waiter());

                

            }
            else{ 
                anim.SetTrigger("Death");
                audioSource.PlayOneShot(Death);
                Destroy(collision.gameObject);

            }
            } 
        }
}
