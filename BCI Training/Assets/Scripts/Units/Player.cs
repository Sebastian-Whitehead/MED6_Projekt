using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SharedDatastructures;
using Unity.VisualScripting;

public class Player : Unit {

    // State the player is about to execute
    public State state = State.Idle;
    public enum State {
        Idle,
        Attack,
        Move,
        Charge
    }
    public LayerMask PlayerLayer;
    private PlayerFeatures res; // Health and mana
    [NonSerialized] public Button confirmBtn; // Execute action
    private ConfirmBtn conBtn; // Confirm button script
    [NonSerialized] public GameObject[] tiles;
    
    
    [Header("Managers")] 
    [NonSerialized] public Gamemode gamemode;
    [NonSerialized] public BciSlider bciPrompt;
    Animator anim;
    private Shoot shoot;
    private TurnManager NewturnManager; 


    [Header("Logging/status")]
    private LoggingManager _loggingManager;
    public int attack_count = 0;
    public int chargeCount;
    
    protected override void ChildAwake()
    {
        confirmBtn = GameObject.Find("ConfirmBtn").GetComponent<Button>();
        confirmBtn.onClick.AddListener(ConfirmAction); // Confirm action btn
        conBtn = confirmBtn.GetComponent<ConfirmBtn>(); // Confirm script
        res = GetComponent<PlayerFeatures>(); // Get resources
        bciPrompt = GetComponent<BciSlider>();
        shoot = GetComponent<Shoot>();
        _loggingManager = GameObject.Find("LoggingManager").GetComponent<LoggingManager>();
        NewturnManager = GameObject.Find("GameManager").GetComponent<TurnManager>();
    }

    protected override void ChildUpdate() {
        res.Alive(audioManager); // Alive check
        if (!execute) return; // Only on execute
        if (state == State.Charge) res.RegenMana();
    }
        
    public override void DecisionTree() {}

    // Check mouse click to move, attack
    void LateUpdate() {

        if (state == State.Idle) ResetConfirmBtn();
        else if (!isMoving && attackTarget == null) ResetConfirmBtn();
        if (bciPrompt.StartBciPrompt) {
            ResetPlayer();
            return; // Guard BCI active
        }

        if (!NewturnManager.playerTurn) return; // Turn check
        if (execute || !active) return;
        if (!isMoving) BFS(); // Breath search to moveable location

        if (!Input.GetMouseButtonDown(0)) return; // Click check
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, ~PlayerLayer)) return;
        if (hit.collider.name == "ConfirmBtn") return;
        if (hit.collider.name == "ChargeButton") return;

        DehighlightEnemies(); // Dehighlight all enemies
        RemoveTileHighlight();
        execute = false; // Action execution
        // attackTarget = null; // Reset attacking (Can't execute atm)
        // isMoving = false; // Reset moving (Can't execute atm)

        SetMoveTarget(hit.collider); // Set tile to move to
        SetAttackTarget(hit.collider); // Set enemy to attack
        Invoke("ActivateBtn", 0.15f); // Activate confirm btn
    }

    // Activate confirm button
    private void ActivateBtn() {
        if (state == State.Idle) return;
        // Debug.Log("Activate Confirm Btn");
        conBtn.UpdateSprite(state.ToString()); // Update confirm sprite
    }

    void ResetConfirmBtn() {
        if (!conBtn.btn.IsInteractable()) return;
        conBtn.DisableImage();
    }

    // Confirm action
    public void ConfirmAction() {
        // print(gamemode);
        if (state == State.Idle) return;

        if (state == State.Attack)
        {
            if (gamemode == Gamemode.Interval){
                conBtn.DisableImage();
                bciPrompt.ChargeMana();
                StartCoroutine(nameof(WaitForBci));
            }
            else if (gamemode == Gamemode.Battery){
            
                anim = gameObject.GetComponent<Animator>();
                anim.SetTrigger("Shoot");
                shoot.shooting();
                conBtn.DisableImage();
                res.Expend();
                attack_count += 1;
                logPlayerData();
            }
        }
        else
        {
            // Debug.Log("Confirm action");
            execute = true;
            conBtn.DisableImage(); // Deactivate confirm btn
        }
        state = State.Idle;
        DehighlightEnemies(); // Dehighlight all enemies
    }

    IEnumerator WaitForBci()
    {
        while (true)
        {
            if (!bciPrompt.complete)
            {
                yield return null;
            }
            if (bciPrompt.complete)
            {
                switch (bciPrompt.success)
                {
                    case true:
                        execute = true;
                        if (state == State.Attack)
                        {
                            attack_count += 1;
                            logPlayerData();
                           
                        }
                        conBtn.DisableImage();
                        anim = gameObject.GetComponent<Animator>();
                        anim.SetTrigger("Shoot");
                        shoot.shooting();
                        yield break;
                    case false:
                        AttackMiss();
                        yield break;
                }
            }
        }
    }

    void AttackMiss()
    {
        
        state = State.Idle;
        execute = true;

        // Kan bruges hvis hun stadig skal lave animationen uden at skyde.
        anim = gameObject.GetComponent<Animator>();
        anim.SetTrigger("Shoot");
        
        Deactivate();
        NewturnManager.EndTurn();

        // Debug.Log("Confirm action");
        execute = true; // Execute action
        //conBtn.DisableImage(); // Deactivate confirm btn

    }

    // Ready moving
    void SetMoveTarget(Collider collider) {
        isMoving = false; // Reset moving
        if (collider.tag != "Tile") return;
        // Debug.Log("Move target: " + collider.name);
        state = State.Move; // Update state to move
        Tile t = collider.GetComponent<Tile>(); // Get tile script
        if (t.selectable) MoveTo(t); // Move to selectable
        else ResetPlayer(); // Reset when not selectable
    }

    // Ready attacking
    void SetAttackTarget(Collider collider) {
        attackTarget = null; // Reset attack target
        if (collider.tag != targetTag) return;
        if (gamemode == Gamemode.Battery && !res.ManaCheck()) return; // Guard no mana
        targetLocation = collider.transform.position; // Set target to enemy location
        transform.LookAt(targetLocation, Vector3.up);
        state = State.Attack; // Attack state player
        Eyes(); // Enemy visible from player
        if (!ConfirmTarget(attackTarget)) ResetPlayer(); // Confirm target 
        else Highlight(collider); // Highlight target
    }

    bool ConfirmTarget(Unit target) {
        if (attackTarget == null) return false; // Break at no target
        bool targetDead = !attackTarget.GetComponent<EnemyHealth>().alive; // Get target alive value
        if (targetDead) return false; // Check if target is still alive
        return true; // Confirm target as valid
    }

    void Highlight(Collider target) {
        target.GetComponent<Highlight>().selected = true;
    }

    void DehighlightEnemies() {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies) {
            enemy.GetComponent<Highlight>().selected = false;
        }
    }

    private float RangeChance(Vector3 targetPos) {
        Vector3 playerPos = gameObject.transform.position;
        float distance = Vector3.Distance(playerPos, targetPos);
        float hitChance = 100 - Mathf.Pow(distance, 2);
        return hitChance;
    }

    // Ready mana charging
    void ReadyCharge() {
        // Debug.Log("Ready: Charge");
        state = State.Charge;
    }

    void ChargeMana() {
        audioManager.PlayCategory("ManaCharge");
        res.RegenMana();
        state = State.Idle;
    }

    public override void TakeDamage(Vector3 hitPosition, float damageTaken) {
        res.Damage(damageTaken);
        audioManager.PlayCategory("TakeDamage");
    }

    public void ResetPlayer() {
        // Debug.Log("Reset player");
        state = State.Idle;
        execute = false;
    }

    // Check if player can attack
    protected override bool AttackCheck() {
        if (!res.ManaCheck()) {
            Debug.Log(name + " no mana");
            ResetPlayer(); // Stop execution at no mana
            return false;
        }
        res.Expend(); // Decrease mana
        return true;
    }

    public override bool Alive() {
        return res.alive;
    }

    private void RemoveTileHighlight() {
        // Debug.Log("RemoveTileHighlight");
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach(var obj in tiles)
        {
            Tile tile = obj.GetComponent<Tile>();
            if (tile.targetTile)
            {
                tile.ResetTile();
                tile.selectable = true;
            }
        }
    }

    private void logPlayerData()
    {
        _loggingManager.Log("Log", new Dictionary<string, object>()
        {
            {"Attack", attack_count},
           // {"Event", "Player Attack"},
           // {"State", Enum.GetName(typeof(State), state)},
        });

    }
    
}
