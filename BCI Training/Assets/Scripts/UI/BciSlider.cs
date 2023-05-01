using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using SharedDatastructures;

public class BciSlider : MonoBehaviour
{
    public Image Highlight;
    public Image SucessHighlight;
    public Slider Slider;
    public Image[] BCIAssembly;

    public Button ChargeButton;
    public Image[] ChargeButtonImg;

    public float BciPromptDuration;
    private bool StartBciPrompt;
    
    private float time;
    public float speed;
    public float promptSpeed;
    private float currentSpeed;
    public Shake shaker;
    
    private PlayerFeatures resources;

    [NonSerialized] public Gamemode gamemode;
    [NonSerialized] public bool complete;
    [NonSerialized] public bool success;
    [NonSerialized] public float currentInputValue;

    private int completedReps = 0;
    private int targetReps = 0;
    
    
    //Logging variables_____________
    private int nrOfBCI = 0;
    private LoggingManager _loggingManager;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _loggingManager = GameObject.Find("LoggingManager").GetComponent<LoggingManager>();
        resources = GetComponent<PlayerFeatures>();
        Slider.maxValue = 1;

        if (gamemode == Gamemode.Interval)
        {
            ShowChargeButton(false);
        }

        ShowAndHideBci(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!StartBciPrompt) return;
        
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

    public void ShowAndHideBci(bool show)
    {
        Slider.enabled = show;
        foreach (var currentImg in BCIAssembly)
        {
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
        Highlight.enabled = false;
        SucessHighlight.enabled = false;
        time = BciPromptDuration;
        currentSpeed = speed;
        StartBciPrompt = false;
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
        SucessHighlight.enabled = true;
        Slider.value = 1 - (time / BciPromptDuration);
        StartBciPrompt = false;
        completedReps++;
        resources.RegenMana(2);
        nrOfBCI++;
        success = true;
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
    }

    
    public void Fail()
    {
        StartBciPrompt = false;
        print("failure");
        
        shaker.ShakeOnce(0.25f);
        //completedReps++;
        
        print("failure");
        success = false;
        nrOfBCI++;
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
        _loggingManager.Log("Log", new Dictionary<string, object>()
        {
            {"BCI attempt", nrOfBCI},
          //  {"Event", "BCI Attempt"},
            {"Success BCI", success}
            
        });

    }

    

    
}
