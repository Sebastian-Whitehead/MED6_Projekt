using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using SharedDatastructures;

public class BciSlider : MonoBehaviour
{
    private Image Highlight;
    private Image SuccessHighlight;
    private Image FailHighlight;

    private TextMeshProUGUI successText;
    private TextMeshProUGUI failText;

    private Slider Slider;
    private Image[] BCIAssembly;

    [NonSerialized] public Button ChargeButton;
    [NonSerialized] public Image[] ChargeButtonImg;

    public float BciPromptDuration;
    [NonSerialized] public bool StartBciPrompt;

    private float time;
    public float speed;
    public float promptSpeed;
    private float currentSpeed;
    [NonSerialized] public Shake shaker;

    private PlayerFeatures resources;
    private Player player;
    private TurnManager turnManager;

    [NonSerialized] public Gamemode gamemode;
    [NonSerialized] public bool complete;
    [NonSerialized] public bool success;
    [NonSerialized] public float currentInputValue;
    [NonSerialized] public bool showable;

    private int completedReps = 0;
    private int targetReps = 0;

//Wizard of Oz_____________
    public bool wizardOfOz = true;
    private readonly float[] successChanceLimits =  {0.65f, 0.85f};
    private float successChance;
    private bool willSucceed = false;
    private bool timeChosen = false;
    private bool forced = false;
    private float startPoint;
    private SimBCIInput simBciInput;
    private bool pressState = false;


//Logging variables_____________
    private int nrOfBCI = 0;
    private LoggingManager _loggingManager;
    private string eventStr;

    private void Awake()
    {
        ChargeButton = GameObject.Find("ChargeButton").GetComponent<Button>();
        ChargeButtonImg = GameObject.Find("ChargeButton").GetComponentsInChildren<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Highlight = GameObject.Find("Bci Highlight").GetComponent<Image>();
        simBciInput = GameObject.Find("InputManager").GetComponent<SimBCIInput>();
        SuccessHighlight = GameObject.Find("Bci Highlight (success)").GetComponent<Image>();
        FailHighlight = GameObject.Find("Bci Highlight (Fail)").GetComponent<Image>();
        failText = GameObject.Find("BCI Fail Text").GetComponent<TextMeshProUGUI>();
        successText = GameObject.Find("BCI Success Text").GetComponent<TextMeshProUGUI>();
        
        BCIAssembly = GameObject.Find("BciBackground").GetComponentsInChildren<Image>();
        Slider = GameObject.Find("Slider").GetComponent<Slider>();
        if (Camera.main != null) shaker = Camera.main.GetComponent<Shake>();
        
        _loggingManager = GameObject.Find("LoggingManager").GetComponent<LoggingManager>();
        resources = GetComponent<PlayerFeatures>();
        player = GameObject.Find("Player").GetComponent<Player>();
        turnManager = GameObject.Find("GameManager").GetComponent<TurnManager>();
        Slider.maxValue = 1;

        resources.mana = 0;
        resources.maxMana = 10;
        if (gamemode == Gamemode.Interval)
        {
            ShowChargeButton(false);
            resources.mana = resources.maxMana = 9999;
        }

        ShowAndHideBci(false);
        successChance = Random.Range(successChanceLimits[0], successChanceLimits[1]);
        print("successChance: " + successChance);
    }

    // Update is called once per frame
    void Update()
    {
        if (!StartBciPrompt)
        {
            if(gamemode == Gamemode.Battery && showable && complete) 
                ShowChargeButton(resources.maxMana !> resources.mana);
            return;
        }

        if (wizardOfOz)
        {
            if (!timeChosen)
            {
                if (SuccessRoll())
                {
                    startPoint = Random.Range(0.2f, 0.78f);
                    print("Will succeed --> Start point: " + startPoint);
                    willSucceed = true;
                }
                timeChosen = true;
            }

            //print("dibs " + !forced + " " + willSucceed + " " + (Slider.value >= startPoint));
            if (!forced && willSucceed && Slider.value >= startPoint)
            {
                ForceSuccess();
                Invoke(nameof(ForceSuccess), 3.5f);
                forced = true;
            }
        }

        time -= Time.deltaTime * currentSpeed;
        Slider.value = 1 - (time / BciPromptDuration);

        if (Slider.value >= 0.418f && StartBciPrompt == true)
        {
            Highlight.enabled = true;
            currentSpeed = promptSpeed;
            
            
            
            if (Slider.value > 0.99)
            {
                Fail();
            }
        }
    }

    public void FilterInputSuccess()
    {
        if (Slider.value >= 0.418f && StartBciPrompt == true)
        {
            Success();
        }
    }

    private bool SuccessRoll()
    {
        float roll = Random.Range(0f, 1f);
        print("roll: " + roll);
        if (roll > successChance) return false;
        return true;
    }

    private void ForceSuccess()
    {
        print( "// cPress: " + simBciInput.cPress);
        //simBciInput.vPress = pressState;
        simBciInput.cPress = !pressState;
        pressState = !pressState;
    }

    public void ShowAndHideBci(bool show)
    {
        Slider.enabled = show;
        foreach (var currentImg in BCIAssembly)
        {
            if (currentImg.name == "ChargeButton" || currentImg.name == "Mana Bottle") continue;
            currentImg.enabled = show;
        }
        ResetBci();

        if (gamemode != Gamemode.Battery) return;
        if (targetReps <= completedReps || completedReps == 0 || resources.mana >= resources.maxMana)
        {
            ShowChargeButton(!show);
        }

    }

    public void ShowChargeButton(bool state)
    {
        ChargeButton.enabled = state;
        foreach (var currentElement in ChargeButtonImg)
        {
            currentElement.enabled = state;
        }
    }

    public void ResetBci()
    {
        Slider.value = Slider.minValue;
        Highlight.enabled = SuccessHighlight.enabled = FailHighlight.enabled = 
            successText.enabled = failText.enabled = false;
        time = BciPromptDuration;
        currentSpeed = speed;
        StartBciPrompt = false;
        
        timeChosen = false;
        willSucceed = false;
        forced = false;
    }

    public void ChargeMana()
    {
        targetReps = 1;
        completedReps = 0;
        
        complete = false;
        ShowAndHideBci(true);
        StartBciPrompt = true;
    }

    public void ChargeMana(int repetitions)
    {
        targetReps = repetitions;
        completedReps = 0;
        complete = false;
        
        ShowAndHideBci(true);
        StartBciPrompt = true;
        StartCoroutine(nameof(StartBciRepeating));
    }
    
    IEnumerator StartBciRepeating()
    {
        while (true)
        {
            switch (StartBciPrompt)
            {
                case true:
                   
                    yield return null;
                    break;
                case false:
                    if (completedReps < targetReps && resources.mana < resources.maxMana)
                    {
                        yield return new WaitForSeconds(1.5f);
                        ShowAndHideBci(false);
                        yield return new WaitForSeconds(0.5f);
                        ShowAndHideBci(true);
                        StartBciPrompt = true;
                        break;
                    }
                    yield break;
            }
        }
    }

    

    // ----------------------------------------------------------------------------------- //
    //  Success and Failure Conditions
    
    public void Success()
    {
        SuccessHighlight.enabled = true;
        successText.enabled = true;
        Slider.value = 1 - (time / BciPromptDuration);
        StartBciPrompt = false;
        completedReps++;
        resources.RegenMana(2);
        nrOfBCI++;
        success = true;
        eventStr = "BciSuccess";
        logBCIData();
        //Maybe delay here
        if (gamemode == Gamemode.Battery)
        {
            if (completedReps >= targetReps || resources.mana >= resources.maxMana)
            {
                Invoke(nameof(SucceseComplete), 1.5f);
            }
        }
        else
        {
            SucceseComplete();
        }
        
    }
    
    void SucceseComplete()
    {
        complete = true;
        ShowAndHideBci(false);
        if (gamemode == Gamemode.Battery) {
            player.ResetPlayer();
            turnManager.EndTurn();
        }
    }

    
    public void Fail()
    {
        StartBciPrompt = false;
        //print("failure");
        FailHighlight.enabled = true;
        failText.enabled = true;
        shaker.ShakeOnce(0.25f);
        //completedReps++;
        
        print("failure");
        success = false;
        nrOfBCI++;
        eventStr = "BciFail";
        logBCIData();

        if (completedReps >= targetReps || gamemode == Gamemode.Interval)
        {
            print("BCI FAIL");
            ShowAndHideBci(false);
            complete = true;
        }
    }
    
    
    private void logBCIData()
    {
        _loggingManager.Log("Game", new Dictionary<string, object>()
        {
            {"BCI attempt", nrOfBCI},
            {"Event", eventStr},
            {"Success BCI", success}
            
        });

    }

    

    
}
