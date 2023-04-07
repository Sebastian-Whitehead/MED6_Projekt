using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public TextMeshProUGUI healthText;

    public void SetHealth(int health)
    {
        slider.value = health;
        UpdateHealthText();
        fill.color = gradient.Evaluate(1f);
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        healthText.text = "Health: " + slider.value + "/" + slider.maxValue;
        
        if (slider.value == 0)
        {
            healthText.enabled = false;
        }
    }
}
