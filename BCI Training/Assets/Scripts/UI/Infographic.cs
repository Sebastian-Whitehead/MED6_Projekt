using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infographic : MonoBehaviour {

    private TMPro.TextMeshProUGUI infoTxt; // Text component

    void Awake() {
        infoTxt = GetComponentsInChildren<TMPro.TextMeshProUGUI>()[0];
    }

    // Start is called before the first frame update
    void Start() {
        Hide();
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Display(bool display) {
        gameObject.SetActive(display);
    }

    public void UpdateAndDisplay(string info) {
        infoTxt.text = info;
        Show();
    }
}
