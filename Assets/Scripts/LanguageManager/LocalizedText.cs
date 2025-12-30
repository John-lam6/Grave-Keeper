using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class LocalizedText : MonoBehaviour {
    public string key;
    public TMP_Text textbox;
    public LanguageManager languageManager;

    private void Awake() {
        textbox =  GetComponent<TMP_Text>();
        languageManager = FindObjectOfType<LanguageManager>();
    }

    private void Start() {
        UpdateText();
    }

    private void OnEnable() {
        UpdateText();
    }

    private void UpdateText() {
        if (languageManager == null || languageManager.currentLanguage == null) {
            Debug.Log("Language manager or language is null.");
            return;
        }

        string localText = languageManager.currentLanguage.GetValue(key);
        if (localText != null) {
            if (textbox != null) {
                textbox.text = localText;
            }
        }
    }
    
}
