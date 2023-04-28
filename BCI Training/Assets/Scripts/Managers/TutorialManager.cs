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

    void Awake() {
        tutorialUI = GameObject.Find("TutorialText").GetComponent<TMPro.TextMeshProUGUI>();
        GameObject gameManager = GameObject.Find("GameManager");
        turnManager = gameManager.GetComponent<TurnManager>();
        gameMode = gameManager.GetComponent<GameMode>();
        player = GameObject.Find("Player");
        enemies = GameObject.Find("Enemies").GetComponentsInChildren<EnemyHealth>();
        UpdateArea();
        
        chargeButton = GameObject.Find("ChargeButton");
        chargeButton.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        CheckTutorialComplete();

        CheckMovingArea1();
        CheckManaArea3();
        CheckEnemyDeath();
    }

    void CheckMovingArea1() {
        if (currentArea != 1) return; // Guard area index
        
        bool playerMoving = player.GetComponent<TacticsMove>().isMoving;
        if (turnManager.playerTurn) return; // Guard player turn
        UpdateArea();
    }

    void CheckEnemyDeath() {
        if (!enemyAreas.Contains(currentArea)) return; // Guard non enemy areas
        EnemyHealth enemyHealth = enemies[currentEnemy]; // Select current enemy
        if (enemyHealth.alive) return; // Continue when current selected enemy is dead
        currentEnemy++;
        UpdateArea();
    }

    void CheckManaArea3() {
        if (currentArea != 3) return; // Guard not area 3

        chargeButton.SetActive(true);

        // Check if game mode is battery
        if (gameMode.gamemode != Gamemode.Battery) { // Guard other than battery
            currentArea++; // Skip this area
            return;
        }

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
        if (currentArea >= tutorialTexts.Length) return;
        tutorialUI.text = tutorialTexts[currentArea]; 
    }

    void Spawn() {
        if (currentArea >= spawnPoints.Length) return;
        GameObject spawnPoint = spawnPoints[currentArea];
        player.transform.position = spawnPoint.transform.position;
    }
}
