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
    public Button confirmBtn; // Execute action
    private ConfirmBtn conBtn; // Confirm button script
    public GameObject[] tiles;
    public int attackCount = 84;
    
    [NonSerialized] public Gamemode gamemode;
    public BciSlider bciPrompt;
    Animator anim;
    private Shoot shoot;
    public int chargeCount;
    
    protected override void ChildAwake() {
        confirmBtn.onClick.AddListener(ConfirmAction); // Confirm action btn
        conBtn = confirmBtn.GetComponent<ConfirmBtn>(); // Confirm script
        res = GetComponent<PlayerFeatures>(); // Get resources
        bciPrompt = GetComponent<BciSlider>();
        shoot = GetComponent<Shoot>();
    }

         protected override void ChildUpdate() {
        res.Alive(audioManager); // Alive check
        if (!execute) return; // Only on execute
        if (state == State.Charge) res.RegenMana();
        
    }
        
    

    protected override void UnitGone() {}
    public override void DecisionTree() {}

    // Check mouse click to move, attack
    void LateUpdate() {

        if (execute || !active) return;
        if (!turnManager.playerTurn) return; // Turn check
        if (!isMoving) BFS(); // Breath search to moveable location
        if (!Input.GetMouseButtonDown(0)) return; // Click check
        
        if (state == State.Idle) ResetConfirmBtn();

        Dehighlight(); // Dehighlight all enemies
        RemoveTileHighlight();
        execute = false; // Action execution

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, ~PlayerLayer)) return;
        SetMoveTarget(hit.collider); // Set tile to move to
        SetAttackTarget(hit.collider); // Set enemy to attack
        ActivateBtn(); // Activate confirm btn
    }

    // Activate confirm button
    private void ActivateBtn() {
        if (state == State.Idle) return;
        // Debug.Log("Activate Confirm Btn");
        conBtn.UpdateSprite(state.ToString()); // Update confirm sprite
    }

    void ResetConfirmBtn() {
        conBtn.DisableImage();
    }

    // Confirm action
    public void ConfirmAction() {
        // print(gamemode);
        if (state == State.Idle) return;

        if (state == State.Attack && gamemode == Gamemode.Interval)
        {
            bciPrompt.ChargeMana();
            StartCoroutine(nameof(WaitForBci));
        }
        else
        {
            // Debug.Log("Confirm action");
            execute = true;
            conBtn.DisableImage(); // Deactivate confirm btn
        }
    
        if (state == State.Attack)
            {
                attackCount += 1;
                // Debug.Log("Player has attacked " + attackCount);
            }
        
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
                        conBtn.DisableImage();
                        anim = gameObject.GetComponent<Animator>();
                        anim.SetTrigger("Shoot");
                        shoot.shooting();
                        yield break;
                    case false:
                        AttackMiss();
                        conBtn.DisableImage();
                        yield break;
                }
            }
        }
    }

    void AttackMiss()
    {
        //TODO: Attack Miss Animation
        state = State.Idle;
        execute = true;
        // Kan bruges hvis hun stadig skal lave animationen uden at skyde.
        //anim = gameObject.GetComponent<Animator>();
        //anim.SetTrigger("Shoot");
        Deactivate();

        // Debug.Log("Confirm action");
        execute = true; // Execute action
        conBtn.DisableImage(); // Deactivate confirm btn
        if (state == State.Attack){
        

        }



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

    void Dehighlight() {
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
}
