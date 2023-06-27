using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

 	- Finder alle audio lists på awake
	- Og spiller det passivt.

    Når man siger kør kategori finder den en tilfældig lyd inden for den kategori fx hvor de dør.
    Derfor er der AudioList.cs.
    Passiv - kører tilfældigt interval.
    
    Awake() - kaldet når audioManger er initialiseret
    
*/

public class AudioManager : MonoBehaviour {
    private Dictionary<string, List<AudioClip>> audioClipDict;
    private AudioSource audioSource;

    public Vector2 PassiveInterval;
    public bool active = true;
    private EnemyHealth enemyHealth;

    void Awake() {
        enemyHealth = GetComponent<EnemyHealth>();
        audioSource = GetComponent<AudioSource>();
        AudioList[] audioLists = GetComponents<AudioList>();
        audioClipDict = new Dictionary<string, List<AudioClip>>();
        foreach (AudioList audioList in audioLists) {
            audioClipDict.Add(audioList.Category, audioList.AudioClips);
        }
        StartCoroutine(PlayPassive());
    }
    
    private IEnumerator PlayPassive() {
        if (!active) yield return null;
        float waitTime = Random.Range(PassiveInterval.x, PassiveInterval.y);
        if (audioClipDict.ContainsKey("Passive")) {
            while (true) {
                yield return new WaitForSeconds(waitTime);
                if (!audioSource.isPlaying && enemyHealth.alive) PlayCategory("Passive");
            }
        }
    }
    /*
    PlayPassive() Coroutune - spiller passive audio clips med random intervaller.
    tjekker om aktiv flag = true.
    så vælger den random wait time mellem de to intervaller.
    Hvis passiv kateogien eksiterer i audio clip dict dictionary så går den i uendeligt loop.
    */

    
    public void PlayCategory(string category) {
        if (!active) return;
        AudioClip audioClip = GetRandomAudioClip(category);
        if (audioClip == null) return;
        PlayClip(audioClip);
    }

    /* 
        PlayCategory() method: This method is used to play an audio clip from a specific category. 
        It checks if the active flag is set to true and retrieves a random audio clip from the specified category using the GetRandomAudioClip() method. 
        If an audio clip is found, it plays it using PlayClip().
    */

    public void PlayClip(AudioClip audioClip) {
        if (!active) return;
        audioSource.PlayOneShot(audioClip);
    }

    /*
    PlayClip() method: This method plays a single audio clip using the audioSource.PlayOneShot() method.
    It checks if the active flag is set to true.
    
    */

    private AudioClip GetRandomAudioClip(string category) {
        if (!audioClipDict.ContainsKey(category)) {
            Debug.Log("AudioManager: Could not find category " + category);
            return null;
        }
        List<AudioClip> audioClips = audioClipDict[category];
        if (audioClips.Count <= 0) {
            Debug.Log("AudioManager: " + category + " is empty.");
            return null;
        }
        int randomIndex = Random.Range(0, audioClips.Count);
        AudioClip randomAudioClip = audioClips[randomIndex];
        return randomAudioClip;
    }

    /*
        GetRandomAudioClip() method: This method retrieves a random audio clip from the specified category. 
        It checks if the category exists in the audioClipDict dictionary. 
        If it does, it selects a random audio clip from the list associated with that category.
    */
}