using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmBtn : MonoBehaviour {
    public List<Sprite> sprites; // Changeable sprites
    private Dictionary<string, Sprite> confirmBtnSprites; // Name, sprite dict
    private Image image; // Image component

    // Start is called before the first frame update
    void Start() {
        image = GetComponent<Image>(); // Button image component
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
    }


}
