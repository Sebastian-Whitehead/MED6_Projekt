using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmBtn : MonoBehaviour {
    public List<Sprite> sprites; // Changeable sprites
    private Dictionary<string, Sprite> confirmBtnSprites; // Name : sprite dict
    private Image image; // Image component
    private TMPro.TextMeshProUGUI text; // Text component
    private Button btn;

    void Awake() {
        btn = GetComponent<Button>();
        image = GameObject.Find("ConfirmSprite").GetComponentInChildren<Image>(); // Button image component
        text = GetComponentInChildren<TMPro.TextMeshProUGUI>(); // Button image component
        DisableImage();
        FormatSprites(); // Format sprites from list to dict
    }

    private void FormatSprites() {
        confirmBtnSprites = new Dictionary<string, Sprite>(); // Sprite dict
        foreach (Sprite sprite in sprites) { // Look sprites
            confirmBtnSprites.Add(sprite.name, sprite); // Add sprite with name
        }
    }

    // Update sprite from given name in dict
    public void UpdateSprite(string spriteName) {
        image.sprite = confirmBtnSprites[spriteName];
        image.enabled = true;
        text.SetText(spriteName);
        btn.interactable = true;
    }

    public void DisableImage() {
        image.enabled = false;
        text.SetText("");
        btn.interactable = false;
    }
}
