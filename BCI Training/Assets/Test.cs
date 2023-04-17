using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : StateMachineBehaviour
{
    public Animator Girl;
    public int playerLife;
    public bool check = true;
   

    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
    {
     if (check == true){
     playerLife = GameObject.Find("Player").GetComponent<Triggeani>().life;
     Debug.Log(playerLife);
     check = false; 
     }
     playerLife = playerLife - 1;
     if (playerLife> 0){
     GameObject.Find("Player").GetComponent<Animator>().Play("Hit Reaction Girl");
     Debug.Log("Punch is Done");
     Debug.Log(playerLife);
     }
     else { GameObject.Find("Player").GetComponent<Animator>().Play("Dying");
     Debug.Log("Player died");
     GameObject.Find("Player").GetComponent<Animator>().SetFloat("Life",playerLife);
     //Debug.Log(anim.GetFloat("Life"));
     }
    
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
