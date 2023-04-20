using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BciSlider : MonoBehaviour
{
    public Image Highlight;
    public Image SucessHighlight;
    public Slider Slider;
    public Image[] BCIAssembly;
    
    public float BciPromptDuration;
    private bool StartBciPrompt;
    
    private float time;
    public float speed;
    public float promptSpeed;
    private float currentSpeed;
    
    public bool simulateBci;
    private Resources resources;
    
    [NonSerialized] public float currentInputValue;
    
    // Start is called before the first frame update
    void Start()
    {
        resources = GetComponent<Resources>();
        Slider.maxValue = 1;
        HideBci();
        ResetBci();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("w"))
        {
            ShowAndResetBci();
            StartBciPrompt = true;
        }
        
        if (!StartBciPrompt) return;
        
        time -= Time.deltaTime * currentSpeed;
        Slider.value = 1 - (time / BciPromptDuration);
        if (simulateBci){ SimulateBCI();}
        
        if (Slider.value >= 0.418f && StartBciPrompt == true)
        {
            Highlight.enabled = true;
            currentSpeed = promptSpeed;
            
            if (currentInputValue == 1f){ Success();}
            if (Slider.value >= 1f){ Fail();}
        }
    }

    public void ShowAndResetBci()
    {
        Slider.enabled = true;
        foreach (var currentImg in BCIAssembly)
        {
            currentImg.enabled = true;
        }
        ResetBci();
    }
    
    public void HideBci()
    {
        foreach (var currentImg in BCIAssembly)
        {
            currentImg.enabled = false;
        }
        Slider.enabled= false;
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

    public void Success()
    {
        SucessHighlight.enabled = true;
        Slider.value = 1 - (time / BciPromptDuration);
        StartBciPrompt = false;
        resources.mana++;
    }

    public void Fail()
    {
        StartBciPrompt = false;
        HideBci();
    }

    public void SimulateBCI()
    {
        if (Slider.value >= 0.418f)
        {
            currentInputValue = Random.Range(0.6f, 1f);
            if (currentInputValue >= 0.999f) // Simulation Threshold
            {
                currentInputValue = 1f;
                Debug.Log("THUNK!");
            }
            
            if (Input.GetKeyDown("space")) // Force Success
            {
                currentInputValue = 1f;
                Debug.Log("THUNK!");
            }
            Debug.Log(currentInputValue);
            return;
        }
        currentInputValue = Random.Range(0f, 1f);
        Debug.Log(currentInputValue);
    }
}
