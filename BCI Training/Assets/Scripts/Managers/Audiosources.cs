using UnityEngine;
using System.Collections;

//t ensures that the AudioSource component is automatically added to the game object when the Audiosources script is attached.
[RequireComponent(typeof(AudioSource))]
public class Audiosources : MonoBehaviour
{
    public AudioClip impact;
    public AudioClip impact2;
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter()
    {
        audioSource.PlayOneShot(impact, 0.7F);
        audioSource.PlayOneShot(impact2, 0.7F);
    }

    /*
    OnCollisionEnter() method: This method is called when a collision occurs between the game object with the Audiosources component and another collider. 
    It's triggered automatically by the Unity physics engine. 
    Inside this method, two audio clips (impact and impact2) are played using the audioSource.PlayOneShot() method. 
    The PlayOneShot() method plays the audio clip as a one-time sound with a specified volume scale (0.7F in this case).
    
    
    */
}
