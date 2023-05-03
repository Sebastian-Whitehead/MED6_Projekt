using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharedDatastructures;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DebugStuff;

public class TutorialManager : MonoBehaviour {

    [Header("Tutorial elements")]
    public int currentArea = 0;
    public int currentEnemy = 0;
    private static readonly List<int> enemyAreas = new List<int>() {2, 4, 5};
    private EnemyHealth[] enemies;
    public GameObject[] spawnPoints;
    private TMPro.TextMeshProUGUI tutorialUI; // Text component
    [TextArea] public string[] tutorialTexts;

    [Header("Gameplay manipulation")]
    private GameObject player;
    private TurnManager turnManager;
    private Image[] chargeButton;
    private BciSlider bciSlider;
    private GameMode gameMode;
    private bool checkComplete = false;

    void Awake() {
        tutorialUI = GameObject.Find("TutorialText").GetComponent<TMPro.TextMeshProUGUI>();
        GameObject gameManager = GameObject.Find("GameManager");
        turnManager = gameManager.GetComponent<TurnManager>();
        gameMode = gameManager.GetComponent<GameMode>();
        player = GameObject.Find("Player");
        enemies = GameObject.Find("Enemies").GetComponentsInChildren<EnemyHealth>();
        bciSlider = player.GetComponent<BciSlider>();
    }

    void Start()
    {
        bciSlider.showable = false;
        bciSlider.ShowChargeButton(false);
        UpdateArea(); // Update area to init
    }

    // Update is called once per frame
    void Update() {
        GoToLevel1();
        CheckTutorialComplete(); // Check if last area is complete
        
        CheckMovingArea1(); // Area 1, check player movement (turn end)
        CheckManaArea3(); // Area 3, check mana regain
        CheckEnemyDeath(); // Other areas, check enemy alive
    }

    void CheckMovingArea1() {
        if (currentArea != 1) return; // Guard area index
        if (turnManager.playerTurn) return; // Guard player turn
        StartCoroutine(DelayUpdateArea(1f)); // Update area if players turn is over
    }

    void CheckEnemyDeath()
    {
        if (!enemyAreas.Contains(currentArea)) return; // Guard non enemy areas
        if (currentEnemy >= enemies.Length) return; // Guard enemy index
        EnemyHealth enemyHealth = enemies[currentEnemy]; // Select current enemy
        if (enemyHealth.alive) return; // Continue when current selected enemy is dead
        currentEnemy++; // Increment currently selected enemy
        StartCoroutine(DelayUpdateArea(1.75f)); // Update area when selected enemy dies
    }

   void CheckManaArea3() {
       if (currentArea != 3) return; // Guard not area 3

        // Check if game mode is battery
        if (gameMode.gamemode != Gamemode.Battery) { // Guard other than battery
            UpdateArea(); // Skip this area
            return; // Abort method
        }
        float mana = player.GetComponent<PlayerFeatures>().mana; // Players mana
        if (mana < 4) return; // Guard not enough mana
        if (gameMode.gamemode == Gamemode.Interval) { UpdateArea(); // when player has gain mana
        } else { StartCoroutine(DelayUpdateArea(1f)); }
    }

    // Update spawn point and tutorial text
    void UpdateArea() {
        // print("Updating Area!");
        Spawn(); // Spawn player at current spawn location
        UpdateUI(); // Update tutorial UI to current text
        currentArea++; // Increment area value

        if (currentArea == 3 && gameMode.gamemode == Gamemode.Battery)
        {
            bciSlider.showable = true;
            bciSlider.ShowChargeButton(true); // Activate charge button
        }
    }

    IEnumerator DelayUpdateArea(float sec)
    {
        if (checkComplete) yield break;
        checkComplete = true;
        yield return new WaitForSeconds(sec);
        print("Delay!");
        UpdateArea();
        checkComplete = false;
    }

    void UpdateUI() {
        if (currentArea >= tutorialTexts.Length) return; // Guard index error
        tutorialUI.text = tutorialTexts[currentArea]; // Update text with tutorial text
    }

    void Spawn() {
        if (currentArea >= spawnPoints.Length) return; // Guard index error
        GameObject spawnPoint = spawnPoints[currentArea]; // Current spawn point
        player.transform.position = spawnPoint.transform.position; // Update position to spawn point
    }

    void CheckTutorialComplete() {
        if (currentArea < 6) return; // Guard index less than end
        if (checkComplete) return;
        checkComplete = true;
        Debug.Log("Tutorial done!");
        Invoke("GoToNextScene", 2f);
    }

    void GoToNextScene() {
        SceneManager.LoadScene("Level 1"); // Launch the new scene
    }

    void GoToLevel1() {
        if (!Input.GetKeyUp(KeyCode.F1)) return;
        GoToNextScene();
    }
}
