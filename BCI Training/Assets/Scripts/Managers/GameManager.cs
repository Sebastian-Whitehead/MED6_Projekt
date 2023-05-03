using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    private PlayerFeatures playerFeatures;
    private Infographic infographic;

    // Start is called before the first frame update
    void Awake() {
        playerFeatures = GameObject.Find("Player").GetComponent<PlayerFeatures>();
        infographic = GameObject.Find("Infographic").GetComponent<Infographic>();
    }

    // Update is called once per frame
    void Update() {
        CheckGameOver();
    }

    void CheckGameOver() {
        if (playerFeatures.alive) return;
        infographic.UpdateAndDisplay("You died..");
        Invoke("ResetLevel", 2f);
    }

    void ResetLevel() {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
