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
    
    private PlayerFeatures resources;

    [NonSerialized] public Gamemode gamemode;
    [NonSerialized] public bool complete;
    [NonSerialized] public bool success;
    [NonSerialized] public float currentInputValue;
    
    // Start is called before the first frame update
    void Start()
    {
        resources = GetComponent<PlayerFeatures>();
        Slider.maxValue = 1;
        ShowChargeButton(false);
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
            
            if (Slider.value >= 1f)Fail();
        }
    }

    public void FilterInputSuccess()
    {
        if (Slider.value >= 0.418f && StartBciPrompt == true) Success();
    }

    public void ShowAndHideBci(bool show)
    {
        Slider.enabled = show;
        foreach (var currentImg in BCIAssembly)
        {
            currentImg.enabled = show;
        }
        ResetBci();
        
        if(gamemode == Gamemode.Battery) ShowChargeButton(!show);
    }

    private void ShowChargeButton(bool state)
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
        complete = false;
        ShowAndHideBci(true);
        StartBciPrompt = true;
    }

    // ----------------------------------------------------------------------------------- //
    //  Success and Failure Conditions
    
    public void Success()
    {
        SucessHighlight.enabled = true;
        Slider.value = 1 - (time / BciPromptDuration);
        StartBciPrompt = false;
        resources.mana++;
        //Maybe delay here
        
        success = true;
        complete = true;
        
        ShowAndHideBci(false);
    }

    public void Fail()
    {
        StartBciPrompt = false;
        success = false;
        complete = true;
        ShowAndHideBci(false);
    }

    

    
}
