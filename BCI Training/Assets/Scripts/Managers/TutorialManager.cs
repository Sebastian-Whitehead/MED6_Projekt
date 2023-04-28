using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharedDatastructures;

public class TutorialManager : MonoBehaviour {

    [Header("Tutorial elements")]
    public int currentArea = 0;
    public int currentEnemy = 0;
    private static readonly List<int> enemyAreas = new List<int>() {2, 4, 5};
    private EnemyHealth[] enemies;
    public GameObject[] spawnPoints;
    private TMPro.TextMeshProUGUI tutorialUI; // Text component
    private string[] tutorialTexts = {
        "Select red tile. Press 'Move' to confirm.",
        "Select enemy. Confirm to attack!",
        "No mana! Charge your mana!",
        "Water tiles are unobstructed.",
        "Other tiles are obstructed."
    };

    [Header("Gameplay manipulation")]
    private GameObject player;
    private TurnManager turnManager;
    private GameObject chargeButton;
    private GameMode gameMode;
    private BciSlider bciSlider;
    private PlayerFeatures res;

    void Awake() {
        tutorialUI = GameObject.Find("TutorialText").GetComponent<TMPro.TextMeshProUGUI>();
        GameObject gameManager = GameObject.Find("GameManager");
        turnManager = gameManager.GetComponent<TurnManager>();
        gameMode = gameManager.GetComponent<GameMode>();
        player = GameObject.Find("Player");
        enemies = GameObject.Find("Enemies").GetComponentsInChildren<EnemyHealth>();
        UpdateArea(); // Update area to init

        
        chargeButton = GameObject.Find("ChargeButton"); // Get charge button element
        chargeButton.SetActive(false); // Deactivate charge button
        
    }

    // Update is called once per frame
    void Update() {
        CheckTutorialComplete(); // Check if last area is complete

        CheckMovingArea1(); // Area 1, check player movement (turn end)
        CheckManaArea3(); // Area 3, check mana regain
        CheckEnemyDeath(); // Other areas, check enemy alive
    }

    void CheckMovingArea1() {
        if (currentArea != 1) return; // Guard area index
        if (turnManager.playerTurn) return; // Guard player turn
        UpdateArea(); // Update area if players turn is over
    }

    void CheckEnemyDeath() {
        if (!enemyAreas.Contains(currentArea)) return; // Guard non enemy areas
        EnemyHealth enemyHealth = enemies[currentEnemy]; // Select current enemy
        if (enemyHealth.alive) return; // Continue when current selected enemy is dead
        currentEnemy++; // Increment currently selected enemy
        UpdateArea(); // Update area when selected enemy dies
    }

   void CheckManaArea3() {
        if (currentArea != 3) return; // Guard not area 3

        // Check if game mode is battery
        if (gameMode.gamemode != Gamemode.Battery) { // Guard other than battery
            UpdateArea(); // Skip this area
            return; // Abort method
        }
        chargeButton.SetActive(true); // Activate charge button
        float mana = player.GetComponent<PlayerFeatures>().mana; // Players mana
        if (mana <= 0) return; // Guard not enough mana
        UpdateArea(); // when player has gain mana
    }

    void CheckTutorialComplete() {
        if (currentArea < 5) return; // Guard index less than end
        Debug.Log("Tutorial done!");
    }

    // Update spawn point and tutorial text
    void UpdateArea() {
        Spawn(); // Spawn player at current spawn location
        UpdateUI(); // Update tutorial UI to current text
        currentArea++; // Increment area value
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
}
