using UnityEngine;
using System.Collections;

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
}
